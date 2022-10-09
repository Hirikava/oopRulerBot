using Autofac;
using OopRulerBot.Infra;
using OopRulerBot.Settings;
using Serilog;
using Serilog.Core;
using Vostok.Configuration;
using Vostok.Configuration.Abstractions;
using Vostok.Configuration.Sources.Json;


namespace OopRulerBot.DI;

public static class BotContainerBuilder
{
    public static IContainer Build()
    {
        var containerBuilder = new ContainerBuilder();

        containerBuilder.Register<ILogger>(cc =>
        {
            var consoleLog =  new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
            
            return consoleLog;
        }).SingleInstance();
        containerBuilder
            .Register<IDiscordLogAdapter>(cc => new SeriDiscordLogAdapter(cc.Resolve<ILogger>()))
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