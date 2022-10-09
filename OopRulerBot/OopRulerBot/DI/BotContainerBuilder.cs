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
    public static IContainer Build()
    {
        var containerBuilder = new ContainerBuilder();

        SetSerilogLog(containerBuilder);
        //SetVostokLog(containerBuilder);

        containerBuilder
            .Register<IConfigurationProvider>(cc =>
            {
                var provider = new ConfigurationProvider();
                provider.SetupSourceFor<BotSecretSettings>(new JsonFileSource("Settings/secrets.json"));
                return provider;
            }).Named<IConfigurationProvider>(ConfigurationScopes.BotSettingsScope);

        return containerBuilder.Build();
    }
    
    private static void SetSerilogLog(ContainerBuilder containerBuilder)
    {
        containerBuilder.Register<ILogger>(_ => new LoggerConfiguration()
            .MinimumLevel.Information ()
            .WriteTo.Console()
            .WriteTo.File("log.txt",
                retainedFileCountLimit: 10,
                fileSizeLimitBytes: 1024 * 1024 * 100,
                rollingInterval: RollingInterval.Day,
                rollOnFileSizeLimit: true)
            .CreateLogger());
        containerBuilder
            .Register<IDiscordLogAdapter>(cc => new SerilogDiscordLogAdapter(cc.Resolve<ILogger>()))
            .SingleInstance();
    }

    private static void SetVostokLog(ContainerBuilder containerBuilder)
    {
        containerBuilder.Register<ILog>(cc =>
        {
            var consoleLog = new ConsoleLog();
            var fileLogSettings = new FileLogSettings
            {
                RollingStrategy = new RollingStrategyOptions
                {
                    MaxFiles = 10,
                    MaxSize = 1024 * 1024 * 100,
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