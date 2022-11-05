﻿using Autofac;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using OopRulerBot.Infra;
using OopRulerBot.Infra.CommandRegistration;
using OopRulerBot.Settings;
using OopRulerBot.Telegram;
using OopRulerBot.Verification;
using OopRulerBot.Verification.Storage;
using OopRulerBot.Verification.Strategies;
using OopRulerBot.Verification.Transport;
using Telegram.Bot;
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
    public static void Build(ContainerBuilder containerBuilder)
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
        containerBuilder
            .Register<IConfigurationProvider>(cc =>
            {
                var provider = new ConfigurationProvider();
                provider.SetupSourceFor<BotSecretSettings>(new JsonFileSource("Settings/secrets.json"));
                return provider;
            }).Named<IConfigurationProvider>(ConfigurationScopes.BotSettingsScope);

        containerBuilder
            .Register(cc => new DiscordSocketClient(new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.All
            }))
            .SingleInstance();

        containerBuilder
            .Register(cc => new InteractionService(cc.Resolve<DiscordSocketClient>()))
            .SingleInstance();

        containerBuilder.Register<ICommandRegistry>(cc =>
                new CommandRegistry(cc.Resolve<InteractionService>(),
                    cc.Resolve<DiscordSocketClient>()))
            .SingleInstance();

        containerBuilder.Register<ITelegramBotClient>(cc =>
        {
            var configurationProvider = cc.ResolveNamed<IConfigurationProvider>(ConfigurationScopes.BotSettingsScope);
            var secretSettings = configurationProvider.Get<BotSecretSettings>();
            return new TelegramBotClient(secretSettings.TelegramToken);
        }).SingleInstance();

        containerBuilder.Register<IVerificationStorage>(cc => new InMemoryVerificationStorage())
            .SingleInstance();

        containerBuilder.Register<ITelegramUsersStorage>(cc => new InMemoryTelegramUserStorage())
            .SingleInstance();
        
        containerBuilder.Register(cc =>
        {
            var log = cc.Resolve<ILog>();
            var telegramUserStorage = cc.Resolve<ITelegramUsersStorage>();
            return new TelegramHandler(log, telegramUserStorage);
        }).SingleInstance();

        containerBuilder.Register<IVerificationTransport>(cc =>
        {
            var telegramBotClient = cc.Resolve<ITelegramBotClient>();
            var telegramUsersStorage = cc.Resolve<ITelegramUsersStorage>();
            var log = cc.Resolve<ILog>();
            return new TelegramVerificationTransport(telegramBotClient, telegramUsersStorage, log);
        }).SingleInstance();


        containerBuilder.Register<IVerificationService>(cc => new VerificationService(
                cc.Resolve<IVerificationTransport[]>(),
                cc.Resolve<IVerificationStorage>(),
                new SequentialStrategy(),
                cc.Resolve<ILog>()))
            .SingleInstance();

        containerBuilder.Register<IVerificationTransport>(cc =>
        {
            var log = cc.Resolve<ILog>();
            var settings = cc.ResolveNamed<IConfigurationProvider>(ConfigurationScopes.BotSettingsScope)
                .Get<BotSecretSettings>().SmtpClientSettings;
            return new SmtpVerificationTransport(log, settings);
        }).SingleInstance();
    }
}