using Autofac;
using OopRulerBot.Infra;
using OopRulerBot.Settings;
using Vostok.Configuration;
using Vostok.Configuration.Abstractions;
using Vostok.Configuration.Sources.Json;
using Serilog;

namespace OopRulerBot.DI;

public static class BotContainerBuilder
{
    public static IContainer Build()
    {
        var containerBuilder = new ContainerBuilder();
        
        containerBuilder.Register<ILogger>(cc =>
            new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(
                    retainedFileCountLimit: 10,
                    fileSizeLimitBytes: 1024 * 1024 * 100,
                    rollingInterval: RollingInterval.Day,
                    path: "/logs/log.txt"
                )
                .CreateLogger()
        );
        
        containerBuilder
            .Register<IDiscordLogAdapter>(cc => new SerilogDiscordLogAdapter(cc.Resolve<ILogger>()))
            .SingleInstance();
        
        containerBuilder
            .Register<IConfigurationProvider>(cc =>
            {
                var provider = new ConfigurationProvider();
                provider.SetupSourceFor<BotSecretSettings>(new JsonFileSource("Settings/secrets.json"));
                return provider;
            }).Named<IConfigurationProvider>(ConfigurationScopes.BotSettingsScope);

        return containerBuilder.Build();
    }
}