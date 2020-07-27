using System;
using System.Threading.Tasks;
using ActivityLogEval.Abstractions;
using Serilog;

namespace ActivityLogEval.Client
{
    public class ResetCmd : ICmd
    {
        private readonly Lazy<IBetRepo> _betRepo;
        private readonly Lazy<IRecommendationRepo> _recRepo;
        private readonly ILogger _logger;

        public ResetCmd(
            Lazy<IBetRepo> betRepo,
            Lazy<IRecommendationRepo> recRepo,
            ILogger logger)
        {
            _betRepo = betRepo ?? throw new ArgumentNullException(nameof(betRepo));
            _recRepo = recRepo ?? throw new ArgumentNullException(nameof(recRepo)); ;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Run(string[] args)
        {
            _logger.Information("Resetting Database");

            if (args.Length == 0)
            {
                await _recRepo.Value.ResetAsync();
                await _betRepo.Value.ResetAsync();
            }
            else
            {
                foreach(var arg in args)
                {
                    switch(arg.ToLowerInvariant())
                    {
                        case "bets":
                            await _betRepo.Value.ResetAsync();
                            break;
                        case "recommendations":
                            await _recRepo.Value.ResetAsync();
                            break;
                        default:
                            throw new ApplicationException("Unknown type");
                    }
                }
            }
        }
    }
}
