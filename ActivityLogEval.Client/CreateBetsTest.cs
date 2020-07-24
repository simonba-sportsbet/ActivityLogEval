using System;
using System.Collections.Generic;
using System.Text;
using Serilog;

namespace ActivityLogEval.Client
{
    class CreateBets : ITest
    {
        private readonly ILogger _logger;

        public CreateBets(ILogger logger)
        {
            _logger = logger;
        }

        public void Run(string[] args)
        {
            _logger.Information("Creating Bets");
        }
    }
}
