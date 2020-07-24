using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Serilog;
using Serilog.Events;

namespace ActivityLogEval.Client
{
    class Program
    {
        static Task Main(string[] args) => new Program().Run(args);
        public Task Run(string[] args)
        {
            var logger = ConfigureLogger();

            logger.Information("Starting");

            if (args.Length < 2)
            {
                logger.Error("No Parameters");
                Console.WriteLine("Usage - Environment, Test [, TestParams, ...]");
                Console.WriteLine("Environments : " + string.Join(",", _envEnvConfig.Keys));
                Console.WriteLine("Test/Cmds : " + string.Join(",", _testToTypeMap.Keys));
                return Task.CompletedTask;
            }

            var cont = ConfigureIoc(logger, args[0]);
            if (cont == null)
                return Task.CompletedTask;

            if (args[1].StartsWith('@'))
                return RunScript(logger, cont, args[1][1..]);
            else
                return RunTest(logger, cont, args[1..]);
        }

        private const string _logTemplate = @"{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3} - {Message:lj}{NewLine}{Exception}";

        public static ILogger ConfigureLogger() => new LoggerConfiguration()
            .WriteTo.Console(LogEventLevel.Information, _logTemplate)
            .WriteTo.Debug()
            .MinimumLevel.Verbose()
            .CreateLogger();

        private static readonly IReadOnlyDictionary<string, Action<ContainerBuilder>> _envEnvConfig =
            new Dictionary<string, Action<ContainerBuilder>>(StringComparer.OrdinalIgnoreCase)
        {
            { "mongodb", b => b.RegisterModule<MongoDb.ConfigModule>() }
        };

        private static string TestName(string typeName) => 
            typeName.EndsWith("Test") ? typeName[0..^4] 
            : typeName.EndsWith("Cmd") ? typeName[0..^3]
            : typeName;

        private static readonly IReadOnlyDictionary<string, Type> _testToTypeMap =
            typeof(Program).Assembly.GetTypes()
                .Where(x => x.IsClass && typeof(ITest).IsAssignableFrom(x))
                .ToDictionary(x => TestName(x.Name), x => x, StringComparer.OrdinalIgnoreCase);

        private static IContainer? ConfigureIoc(ILogger logger, string env)
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(logger);
            builder.RegisterModule<ConfigModule>();

            // Environments
            if (_envEnvConfig.TryGetValue(env, out var envConfig))
                envConfig(builder);
            else
            {
                logger.Error("Unknown Environment : {Env}", env);
                return null;
            }

            // Tests
            foreach (var t in _testToTypeMap.Values)
                builder.RegisterType(t);

            return builder.Build();
        }

        private async Task RunScript(ILogger logger, IContainer cont, string scriptFile)
        {
            if (!File.Exists(scriptFile))
            {
                logger.Error("Can't find script file: {ScriptFile}", scriptFile);
                return;
            }

            foreach(var cmd in File.ReadAllLines(scriptFile))
            {
                if (string.IsNullOrWhiteSpace(cmd) || cmd.StartsWith('#'))
                    continue;

                await RunTest(logger, cont, cmd.Split(' ', StringSplitOptions.RemoveEmptyEntries));
            }
        }

        private async Task RunTest(ILogger logger, IContainer cont, string[] args)
        {
            logger.Information("Running Cmd : {Command}", args[0]);

            if (!_testToTypeMap.TryGetValue(args[0], out var testType))
            {
                logger.Error("Test not configured : {TestName]", args[0]);
                return;
            }

            using var nc = cont.BeginLifetimeScope();

            var test = (ITest)nc.Resolve(testType);

            await test.Run(args[1..]);
        }
    }
}
