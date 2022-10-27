// See https://aka.ms/new-console-template for more information

using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using OopRulerBot.DI;
using OopRulerBot.Infra;
using OopRulerBot.Infra.CommandRegistration;
using OopRulerBot.Settings;
using OopRulerBot.Telegram;
using Telegram.Bot;
using Vostok.Configuration.Abstractions;

namespace OopRulerBot;

public static class Program
{
    private static IContainer Container = null!;

    public static async Task Main(string[] args)
    {
        var autofacServiceProviderFactory = new AutofacServiceProviderFactory(BotContainerBuilder.Build);
        var builder = autofacServiceProviderFactory.CreateBuilder(new ServiceCollection());
        Container = builder.Build();
        var serviceProvider = Container.Resolve<IServiceProvider>();

        var discordSocketClient = Container.Resolve<DiscordSocketClient>();
        discordSocketClient.Log += Container.Resolve<IDiscordLogAdapter>().HandleLogEvent;
        var discordInteractionHandler = new DiscordInteractionHandler(Container.Resolve<InteractionService>(),
            Container.Resolve<DiscordSocketClient>(),
            serviceProvider);

        discordSocketClient.InteractionCreated += discordInteractionHandler.Handle;
        


        var interactionService = Container.Resolve<InteractionService>();
        await interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), serviceProvider);
        interactionService.Log += Container.Resolve<IDiscordLogAdapter>().HandleLogEvent;

        var commandRegistrationHelper = Container.Resolve<ICommandRegistry>();
        discordSocketClient.Ready += commandRegistrationHelper.RegisterCommandsOnReady;
        discordSocketClient.JoinedGuild += commandRegistrationHelper.RegisterCommandOnJoinedServer;

        var discordToken = Container
            .ResolveNamed<IConfigurationProvider>(ConfigurationScopes.BotSettingsScope)
            .Get<BotSecretSettings>().DiscordToken;
        
        await discordSocketClient.LoginAsync(TokenType.Bot, discordToken);
        await discordSocketClient.StartAsync();

        //TODO remove later
        var telegramClient = Container.Resolve<ITelegramBotClient>();
        var telegramHandler = Container.Resolve<TelegramHandler>();
        telegramClient.StartReceiving(telegramHandler);
        await Task.Delay(-1);
    }
}