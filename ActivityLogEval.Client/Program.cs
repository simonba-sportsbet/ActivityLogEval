using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autofac;
using Serilog;
using Serilog.Events;

namespace ActivityLogEval.Client
{
    class Program
    {
        private readonly ILogger _logger;
        private readonly IContainer _cont;
        public Program(ILogger logger, IContainer cont)
        {
            _logger = logger;
            _cont = cont;
        }

        static void Main(string[] args)
        {
            var logger = ConfigureLogger();

            logger.Information("Starting");

            if (args.Length < 2)
            {
                logger.Error("No Parameters");
                Console.WriteLine("Usage - Environment, Test [, TestParams, ...]");
                Console.WriteLine("Environments : " + string.Join(",", _envEnvConfig.Keys));
                Console.WriteLine("Test/Cmds : " + string.Join(",", TestTypes.Select(x => TestName(x.Name))));
                return;
            }

            var cont = ConfigureIoc(logger, args[0]);
            if (cont == null)
                return;

            new Program(logger, cont).Run(args[1..]);
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

        private static IEnumerable<Type> TestTypes => typeof(Program).Assembly.GetTypes().Where(x => x.IsClass && typeof(ITest).IsAssignableFrom(x));
        private static string TestName(string typeName) => typeName.EndsWith("Test") ? typeName[0..^4] : typeName;

        private static IContainer ConfigureIoc(ILogger logger, string env)
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(logger);

            // Environments
            if (_envEnvConfig.TryGetValue(env, out var envConfig))
                envConfig(builder);
            else
            {
                logger.Error("Unknown Environment : {Env}", env);
                return null;
            }

            // Tests
            foreach(var t in TestTypes)
                builder.RegisterType(t).As<ITest>().Named(TestName(t.Name), typeof(ITest));

            return builder.Build();
        }

        public void Run(string[] args)
        {
            if (args[0].StartsWith('@'))
                RunScript(args[0][1..]);
            else
                RunTest(args);
        }

        private void RunScript(string scriptFile)
        {
            if (!File.Exists(scriptFile))
            {
                _logger.Error("Can't find script file: {ScriptFile}", scriptFile);
                return;
            }

            foreach(var cmd in File.ReadAllLines(scriptFile))
            {
                if (string.IsNullOrWhiteSpace(cmd) || cmd.StartsWith('#'))
                    continue;

                RunTest(cmd.Split(' ', StringSplitOptions.RemoveEmptyEntries));
            }
        }
        private void RunTest(string[] cmd)
        {
            _logger.Information("Running Cmd : {Command}", cmd[0]);

            using var nc = _cont.BeginLifetimeScope();

            var test = nc.ResolveNamed<ITest>(cmd[0]);

            test.Run(cmd[1..]);
        }
    }
}
