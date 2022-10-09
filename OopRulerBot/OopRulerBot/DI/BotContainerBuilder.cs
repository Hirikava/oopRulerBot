using Autofac;
using Serilog;
using OopRulerBot.Infra;
using OopRulerBot.Settings;
using Vostok.Configuration;
using Vostok.Configuration.Abstractions;
using Vostok.Configuration.Sources.Json;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Console;
using Vostok.Logging.File;
using Vostok.Logging.File.Configuration;

namespace OopRulerBot.DI;

public static class BotContainerBuilder
{
    private static readonly int maxFiles = 10;
    private static readonly long maxSize = 1024 * 1024 * 100;

    public static IContainer Build()
    {
        var containerBuilder = new ContainerBuilder();

        containerBuilder.RegisterVostokLogger();
        containerBuilder.RegisterSerilogLogger();

        containerBuilder
            .Register<IConfigurationProvider>(cc =>
            {
                var provider = new ConfigurationProvider();

                provider.SetupSourceFor<BotSecretSettings>(new JsonFileSource("Settings/secrets.json"));

                return provider;
            }).Named<IConfigurationProvider>(ConfigurationScopes.BotSettingsScope);

        return containerBuilder.Build();
    }

    public static void RegisterVostokLogger(this ContainerBuilder builder)
    {
        builder.Register<ILog>(_ =>
        {
            var fileLogSettings = new FileLogSettings
            {
                RollingStrategy = new RollingStrategyOptions
                {
                    MaxFiles = maxFiles,
                    MaxSize = maxSize,
                    Period = RollingPeriod.Day,
                    Type = RollingStrategyType.Hybrid
                }
            };

            var fileLog = new FileLog(fileLogSettings);
            var consoleLog = new ConsoleLog();

            return new CompositeLog(consoleLog, fileLog);
        }).SingleInstance();

        builder.Register<IDiscordLogAdapter>(cc => new VostokDiscordLogAdapter(cc.Resolve<ILog>()))
            .SingleInstance()
            .Named<IDiscordLogAdapter>(ConfigurationScopes.VostokLogger);
    }

    public static void RegisterSerilogLogger(this ContainerBuilder builder)
    {
        builder.Register<ILogger>(_ => new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(
                "log.txt",
                retainedFileCountLimit: maxFiles,
                fileSizeLimitBytes: maxSize,
                rollOnFileSizeLimit: true,
                rollingInterval: RollingInterval.Day)
            .CreateLogger())
            .SingleInstance();

        builder.Register<IDiscordLogAdapter>(cc => new SerilogDiscordLogAdapter(cc.Resolve<ILogger>()))
            .SingleInstance()
            .Named<IDiscordLogAdapter>(ConfigurationScopes.SerilogLogger);
    }
}