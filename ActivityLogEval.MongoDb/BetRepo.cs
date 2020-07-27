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
    public class BetRepo : IBetRepo
    {
        private readonly Lazy<IMongoDatabase> _db;
        private readonly ILogger _logger;

        public BetRepo(
            Lazy<IMongoDatabase> db,
            ILogger logger)
        {
            _db = db;
            _logger = logger;
        }

        public const string BetsCollectionName = "bets";

        public Task ResetAsync() => _db.Value.DropCollectionAsync(BetsCollectionName);

        private IMongoCollection<Bet> Bets => _db.Value.GetCollection<Bet>(BetsCollectionName);

        public Task InsertBetAsync(IBet bet)
        {
            _logger.Debug("Create Bet - BetId:{BetId}", bet?.BetId);

            if (bet == null || !(bet is Bet rec))
                throw new ArgumentNullException(nameof(bet));

            return Bets.InsertOneAsync(rec);
        }

        public Task InsertBetsAsync(IEnumerable<IBet> bets)
        {
            if (bets == null)
                throw new ArgumentNullException(nameof(bets));

            var recs = bets.Cast<Bet>();

            return Bets.InsertManyAsync(recs);
        }

        public IEnumerable<IBet> GetAllBets() => Bets.AsQueryable();

        public IEnumerable<IBet> QueryBetById(params string[] betId)
        {
            return Bets.AsQueryable().Where(x => betId.Contains(x.BetId));
        }

        public IBet CreateNewBet() => new Bet();
        public ILeg CreateNewLeg() => new Leg();
        public ISelection CreateNewSelection() => new Selection();

        public IBet[] DeserializeBet(string jsonStream) => JsonSerializer.Deserialize<Bet[]>(jsonStream);
    }
}
