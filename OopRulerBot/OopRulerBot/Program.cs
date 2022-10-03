// See https://aka.ms/new-console-template for more information

using Autofac;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using OopRulerBot.DI;
using OopRulerBot.Infra;
using OopRulerBot.Settings;
using Vostok.Configuration.Abstractions;
using Vostok.Logging.Abstractions;

namespace OopRulerBot;

public static class Program
{
    private static IContainer Container = null!;

    public static async Task Main(string[] args)
    {
        Container = BotContainerBuilder.Build();
        var discordClient = new DiscordSocketClient();

        discordClient.Log += Container.Resolve<IDiscordLogAdapter>().HandleLogEvent;
        discordClient.MessageReceived += HandleMessage;
        
        var discordToken = Container.ResolveNamed<IConfigurationProvider>(ConfigurationScopes.BotSettingsScope)
            .Get<BotSecretSettings>().DiscordToken;
        await discordClient.LoginAsync(TokenType.Bot, discordToken);
        await discordClient.StartAsync();
        await Task.Delay(-1);
    }

    public static Task HandleMessage(SocketMessage socketMessage)
    {
        var message = socketMessage as SocketUserMessage;
        if (message == null) return Task.CompletedTask;
        var log = Container.Resolve<ILog>();
        log.Info("{sender} says {message}", message.Author.Username, message.Content);
        return Task.CompletedTask;
    }
}