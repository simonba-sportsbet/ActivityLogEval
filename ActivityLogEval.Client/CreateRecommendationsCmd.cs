using System;
using System.IO;
using System.Threading.Tasks;
using ActivityLogEval.Abstractions;
using Serilog;

namespace ActivityLogEval.Client
{
    class CreateRecommendationsCmd : ICmd
    {
        private readonly IRecommendationRepo _repo;
        private readonly ILogger _logger;

        public CreateRecommendationsCmd(
            IRecommendationRepo repo, 
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

            _logger.Information("Creating Recommendation from {TestFileName}", testFileName);

            var json = File.ReadAllText(testFileName);

            var recs = _repo.DeserializeRecommendation(json);

            await _repo.InsertRecommendationsAsync(recs);
        }
    }
}
