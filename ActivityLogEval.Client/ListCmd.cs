using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ActivityLogEval.Abstractions;
using Serilog;

namespace ActivityLogEval.Client
{
    public class ListCmd : ICmd
    {
        private readonly Lazy<IBetRepo> _betRepo;
        private readonly Lazy<IRecommendationRepo> _recRepo;
        private readonly ILogger _logger;

        public ListCmd(
            Lazy<IBetRepo> betRepo,
            Lazy<IRecommendationRepo> recRepo,
            ILogger logger)
        {
            _betRepo = betRepo ?? throw new ArgumentNullException(nameof(betRepo));
            _recRepo = recRepo ?? throw new ArgumentNullException(nameof(recRepo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task Run(string[] args)
        {
            var coln = args[0].ToLowerInvariant();
            
            switch(coln)
            {
                case "bets":
                    ListBets(args[1..]);
                    break;
                case "recs":
                case "recommendations":
                    ListRecommendations(args[1..]);
                    break;
                default:
                    _logger.Error("Unknown collection - {coln}", coln);
                    break;
            }
            
            return Task.CompletedTask;
        }

        public void ListBets(string[] args)
        {
            IEnumerable<IBet> bets;

            if (args.Length == 0)
                bets = _betRepo.Value.GetAllBets();
            else
                bets = _betRepo.Value.QueryBetById(args);

            var betsJson = bets.ToJson();

            _logger.Information(betsJson);
        }

        public void ListRecommendations(string[] args)
        {
            IEnumerable<IRecommendation> recs;

            if (args.Length == 0)
                recs = _recRepo.Value.GetAllRecommendations();
            else
                recs = _recRepo.Value.QueryRecommendationById(args);

            var recsJson = recs.ToJson();

            _logger.Information(recsJson);
        }
    }
}
