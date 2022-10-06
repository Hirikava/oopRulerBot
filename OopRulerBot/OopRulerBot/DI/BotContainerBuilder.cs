using Autofac;
using OopRulerBot.Infra;
using OopRulerBot.Settings;
using Serilog;
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
    private const int MaxLogFileSize = 1024 * 1024 * 100;
    private const int MaxFileCount = 10;

    public static IContainer Build()
    {
        var containerBuilder = new ContainerBuilder();
        
        UseSerilog(containerBuilder);

        containerBuilder
            .Register<IConfigurationProvider>(cc =>
            {
                var provider = new ConfigurationProvider();
                provider.SetupSourceFor<BotSecretSettings>(new JsonFileSource("Settings/secrets.json"));
                return provider;
            }).Named<IConfigurationProvider>(ConfigurationScopes.BotSettingsScope);

        return containerBuilder.Build();
    }

    private static void UseSerilog(ContainerBuilder containerBuilder)
    {
        containerBuilder.Register<ILogger>(_ => new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("log.txt",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: MaxFileCount,
                fileSizeLimitBytes: MaxLogFileSize,
                rollOnFileSizeLimit: true)
            .CreateLogger());

        containerBuilder.Register<IDiscordLogAdapter>(cc => 
            new SerilogDiscordLogAdapter(cc.Resolve<ILogger>())
        );
    }

    private static void UseVostokLog(ContainerBuilder containerBuilder)
    {
        containerBuilder.Register<ILog>(cc =>
        {
            var consoleLog = new ConsoleLog();
            var fileLogSettings = new FileLogSettings
            {
                RollingStrategy = new RollingStrategyOptions
                {
                    MaxFiles = MaxFileCount,
                    MaxSize = MaxLogFileSize,
                    Period = RollingPeriod.Day,
                    Type = RollingStrategyType.Hybrid
                }
            };
            var fileLog = new FileLog(fileLogSettings);
            return new CompositeLog(consoleLog, fileLog);
        }).SingleInstance();

        containerBuilder
            .Register<IDiscordLogAdapter>(cc => new VostokDiscordLogAdapter(cc.Resolve<ILog>()))
            .SingleInstance();
    }
}