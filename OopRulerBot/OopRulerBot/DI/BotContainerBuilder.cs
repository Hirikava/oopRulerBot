using Autofac;
using OopRulerBot.Infra;
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

        return containerBuilder.Build();
    }
}