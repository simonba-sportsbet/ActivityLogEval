using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Serilog;
using Serilog.Events;

#pragma warning disable CA1031 // Do not catch general exception types

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
                Console.WriteLine("Usage - Environment, Cmd [, CmdArgs, ...]");
                Console.WriteLine("Environments : " + string.Join(",", _envEnvConfig.Keys));
                Console.WriteLine("Cmds : " + string.Join(",", _cmdToTypeMap.Keys));
                return Task.CompletedTask;
            }

            var cont = ConfigureIoc(logger, args[0]);
            if (cont == null)
                return Task.CompletedTask;

            return RunCmd(logger, cont, args[1..]);
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

        private static string CmdName(string typeName) =>  typeName.EndsWith("Cmd") ? typeName[0..^3] : typeName;

        private static readonly IReadOnlyDictionary<string, Type> _cmdToTypeMap =
            typeof(Program).Assembly.GetTypes()
                .Where(x => x.IsClass && typeof(ICmd).IsAssignableFrom(x))
                .ToDictionary(x => CmdName(x.Name), x => x, StringComparer.OrdinalIgnoreCase);

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

            // Commands
            foreach (var t in _cmdToTypeMap.Values)
                builder.RegisterType(t);

            return builder.Build();
        }

        private async Task RunCmd(ILogger logger, IContainer cont, string[] args)
        {
            logger.Information("Running Cmd : {Command}", args[0]);


            if (args[0].StartsWith("@"))
                await RunScript(logger, cont, args[0][1..]);
            else
            {

                if (!_cmdToTypeMap.TryGetValue(args[0], out var cmdType))
                {
                    logger.Error("Command not configured : {CmdName]", args[0]);
                    return;
                }

                using var nc = cont.BeginLifetimeScope();

                var cmd = (ICmd)nc.Resolve(cmdType);

                try
                {
                    await cmd.Run(args[1..]);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Doh!");
                }
            }
        }

        private async Task RunScript(ILogger logger, IContainer cont, string scriptFile)
        {
            if (!File.Exists(scriptFile))
            {
                logger.Error("Can't find script file: {ScriptFile}", scriptFile);
                return;
            }

            foreach (var cmd in File.ReadAllLines(scriptFile))
            {
                if (string.IsNullOrWhiteSpace(cmd) || cmd.StartsWith('#'))
                    continue;

                await RunCmd(logger, cont, cmd.Split(' ', StringSplitOptions.RemoveEmptyEntries));
            }
        }
    }
}
