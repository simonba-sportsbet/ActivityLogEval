﻿using System;
using System.Threading.Tasks;
using ActivityLogEval.Abstractions;
using Serilog;

namespace ActivityLogEval.Client
{
    public class ResetCmd : ITest
    {
        private readonly IRepo _repo;
        private readonly ILogger _logger;

        public ResetCmd(
            IRepo repo,
            ILogger logger)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task Run(string[] args)
        {
            _logger.Information("Resetting Database");
            return _repo.ResetAsync();
        }
    }
}