using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Serilog;
using ActivityLogEval.Abstractions;
using System.Threading.Tasks;

namespace ActivityLogEval.MongoDb
{
    public class MongoDbRepo : IRepo
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly ILogger _logger;

        private readonly Lazy<IMongoDatabase> _db;

        public MongoDbRepo(
            IDbConnectionFactory dbConnectionFactory,
            ILogger logger)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _logger = logger;

            _db = new Lazy<IMongoDatabase>(() => _dbConnectionFactory.GetDatabase());
        }

        public const string BetsCollectionName = "bets";

        public async Task ResetAsync()
        {
            await _db.Value.DropCollectionAsync(BetsCollectionName);
        }

        private IMongoCollection<Bet> Bets => _db.Value.GetCollection<Bet>(BetsCollectionName);

        public Task InsertBetAsync(Bet bet)
        {
            _logger.Debug("Create Bet - BetId:{BetId}", bet?.BetId);

            if (bet == null)
                throw new ArgumentNullException(nameof(bet));

            return Bets.InsertOneAsync(bet);
        }

        public Task InsertBetsAsync(IEnumerable<Bet> bets)
        {
            if (bets == null)
                throw new ArgumentNullException(nameof(bets));

            return Bets.InsertManyAsync(bets);
        }

        public IEnumerable<Bet> QueryBetById(params string[] betId)
        {
            return Bets.AsQueryable().Where(x => betId.Contains(x.BetId));
        }
    }
}
