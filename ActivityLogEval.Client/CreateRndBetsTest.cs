using System;
using System.Linq;
using System.Threading.Tasks;
using ActivityLogEval.Abstractions;
using Serilog;

namespace ActivityLogEval.Client
{
    class CreateRndBets : ITest
    {
        private readonly IBetGenerator _betGenerator;
        private readonly IRepo _repo;
        private readonly ILogger _logger;

        public CreateRndBets(
            IBetGenerator betGenerator,
            IRepo repo, 
            ILogger logger)
        {
            _betGenerator = betGenerator;
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); 
        }

        public async Task Run(string[] args)
        {
            int betCount = args.Length > 0 && int.TryParse(args[0], out var bc) ? bc : 1;

            _logger.Information("Creating Bets - Count:{BetCount}", betCount);

            string lb = string.Empty;

            foreach (var (idx, bet) in Enumerable.Range(1, betCount).Select(idx => (idx, _betGenerator.CreateRandomBet())))
            {
                _logger.Debug("New bet - {count}", idx);
                await _repo.InsertBetAsync(bet);

                lb = bet.BetId;
            }
        }
    }
}
