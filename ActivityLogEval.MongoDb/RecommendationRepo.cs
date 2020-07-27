using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Serilog;
using ActivityLogEval.Abstractions;

namespace ActivityLogEval.MongoDb
{
    public class RecommendationRepo : IRecommendationRepo
    {
        private readonly Lazy<IMongoDatabase> _db;
        private readonly ILogger _logger;

        public RecommendationRepo(
            Lazy<IMongoDatabase> db,
            ILogger logger)
        {
            _db = db;
            _logger = logger;
        }

        public const string RecommendationsCollectionName = "recommendations";

        public Task ResetAsync() => _db.Value.DropCollectionAsync(RecommendationsCollectionName);

        private IMongoCollection<Recommendation> Recommendations => _db.Value.GetCollection<Recommendation>(RecommendationsCollectionName);

        public Task InsertRecommendationAsync(IRecommendation recommendation)
        {
            _logger.Debug("Create Recommendation - RecommendationId:{RecommendationId}", recommendation?.RecommendationId);

            if (recommendation == null || !(recommendation is Recommendation rec))
                throw new ArgumentException(nameof(recommendation));

            return Recommendations.InsertOneAsync(rec);
        }

        public Task InsertRecommendationsAsync(IEnumerable<IRecommendation> recommendations)
        {
            if (recommendations == null)
                throw new ArgumentNullException(nameof(recommendations));

            var recs = recommendations.Cast<Recommendation>();

            return Recommendations.InsertManyAsync(recs);
        }

        public IEnumerable<IRecommendation> GetAllRecommendations() => Recommendations.AsQueryable();

        public IEnumerable<IRecommendation> QueryRecommendationById(params string[] recId)
        {
            return Recommendations.AsQueryable().Where(x => recId.Contains(x.RecommendationId));
        }

        public IRecommendation CreateNewRecommendation() => new Recommendation();

        public IRecommendation[] DeserializeRecommendation(string jsonStream)
        {
            var options = new JsonSerializerOptions { };
            options.Converters.Add(new JsonStringEnumConverter());

            return JsonSerializer.Deserialize<Recommendation[]>(jsonStream, options);
        }
    }
}
