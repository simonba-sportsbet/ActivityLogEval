using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ActivityLogEval.Abstractions;
using Serilog;

namespace ActivityLogEval.Client
{
    public class ListCmd : ICmd
    {
        private readonly IBetRepo _repo;
        private readonly ILogger _logger;

        public ListCmd(
            IBetRepo repo,
            ILogger logger)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
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
                bets = _repo.GetAllBets();
            else
                bets = _repo.QueryBetById(args);

            var betsJson = bets.ToJson();

            _logger.Information(betsJson);
        }
    }
}
