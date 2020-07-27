using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
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

        public const string RecommendationsCollectionName = "bets";

        public async Task ResetAsync()
        {
            await _db.Value.DropCollectionAsync(RecommendationsCollectionName);
        }

        private IMongoCollection<Recommendation> Recommendations => _db.Value.GetCollection<Recommendation>(RecommendationsCollectionName);

        public Task InsertRecommendationAsync(IRecommendation bet)
        {
            _logger.Debug("Create Recommendation - RecommendationId:{RecommendationId}", bet?.RecommendationId);

            if (bet == null || !(bet is Recommendation rec))
                throw new ArgumentNullException(nameof(bet));

            return Recommendations.InsertOneAsync(rec);
        }

        public Task InsertRecommendationsAsync(IEnumerable<IRecommendation> bets)
        {
            if (bets == null)
                throw new ArgumentNullException(nameof(bets));

            var recs = bets.Cast<Recommendation>();

            return Recommendations.InsertManyAsync(recs);
        }

        public IEnumerable<IRecommendation> GetAllRecommendations() => Recommendations.AsQueryable();

        public IEnumerable<IRecommendation> QueryRecommendationById(params string[] betId)
        {
            return Recommendations.AsQueryable().Where(x => betId.Contains(x.RecommendationId));
        }

        public IRecommendation CreateNewRecommendation() => new Recommendation();

        public IRecommendation[] DeserializeRecommendation(string jsonStream) => JsonSerializer.Deserialize<Recommendation[]>(jsonStream);
    }
}
