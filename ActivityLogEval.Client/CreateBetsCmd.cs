using System;
using System.IO;
using System.Threading.Tasks;
using ActivityLogEval.Abstractions;
using Serilog;

namespace ActivityLogEval.Client
{
    class CreateBets : ICmd
    {
        private readonly IBetRepo _repo;
        private readonly ILogger _logger;

        public CreateBets(
            IBetRepo repo, 
            ILogger logger)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); 
        }

        public async Task Run(string[] args)
        {
            if (args.Length < 1)
            {
                _logger.Error("Test Args: <FileName>");
                return;
            }

            var testFileName = args[0];

            if (!File.Exists(testFileName))
            {
                _logger.Error("Test FileName doesn't exist - {TestFileName}", testFileName);
                return;
            }

            _logger.Information("Creating Bets from {TestFileName}", testFileName);

            var json = File.ReadAllText(testFileName);
            var bets = _repo.DeserializeBet(json);

            await _repo.InsertBetsAsync(bets);
        }
    }
}
