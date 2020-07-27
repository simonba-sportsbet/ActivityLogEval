using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActivityLogEval.Abstractions;
using Serilog;

namespace ActivityLogEval.Client
{
    public class ListRecBets : ICmd
    {
        private readonly IRecommendationRepo _recRepo;
        private readonly ILogger _logger;

        public ListRecBets(
            IRecommendationRepo recRepo,
            ILogger logger)
        {
            _recRepo = recRepo;
            _logger = logger;
        }

        public Task Run(string[] args)
        {
            if (args.Length < 1)
                throw new ArgumentException("No RecId");

            var bets = _recRepo.QueryBetsForRecommendationId(args[0]);

            _logger.Information(bets.ToJson());


            return Task.CompletedTask;
        }
    }
}
