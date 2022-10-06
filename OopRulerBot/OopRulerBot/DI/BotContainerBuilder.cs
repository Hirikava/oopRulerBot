using Autofac;
using OopRulerBot.Infra;
using OopRulerBot.Settings;
using Serilog;
using Vostok.Configuration;
using Vostok.Configuration.Abstractions;
using Vostok.Configuration.Sources.Json;

namespace OopRulerBot.DI;

public static class BotContainerBuilder
{
    public static IContainer Build()
    {
        var containerBuilder = new ContainerBuilder();

        containerBuilder.Register<ILogger>(_ => new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(
                    path: "./logs/log.txt",
                    retainedFileCountLimit: 10,
                    fileSizeLimitBytes: 1024 * 1024 * 100,
                    rollOnFileSizeLimit: true,
                    rollingInterval: RollingInterval.Day)
                .CreateLogger())
            .SingleInstance();
        containerBuilder
            .Register<IDiscordLogAdapter>(
                cc => new SerilogDiscordLogAdapter(cc.Resolve<ILogger>()))
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