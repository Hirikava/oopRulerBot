using Autofac;
using Discord;
using Discord.WebSocket;
using OopRulerBot.DI;
using OopRulerBot.Infra;
using OopRulerBot.Settings;
using Serilog;
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

        discordClient.Log += Container.ResolveNamed<IDiscordLogAdapter>(ConfigurationScopes.SerilogLogger)
            .HandleLogEvent;
        discordClient.MessageReceived += HandleMessageWithSerilog;

        var discordToken = Container.ResolveNamed<IConfigurationProvider>(ConfigurationScopes.BotSettingsScope)
            .Get<BotSecretSettings>().DiscordToken;

        await discordClient.LoginAsync(TokenType.Bot, discordToken);
        await discordClient.StartAsync();
        await Task.Delay(-1);
    }

    public static Task HandleMessageWithVostok(SocketMessage socketMessage)
    {
        if (socketMessage is not SocketUserMessage message)
            return Task.CompletedTask;

        var log = Container.Resolve<ILog>();

        log.Info(
            "{sender} says {message}",
            message.Author.Username,
            message.Content);

        return Task.CompletedTask;
    }

    public static Task HandleMessageWithSerilog(SocketMessage socketMessage)
    {
        if (socketMessage is not SocketUserMessage message) 
            return Task.CompletedTask;

        var log = Container.Resolve<ILogger>();

        log.Information(
            "{sender} says {message}",
            message.Author.Username,
            message.Content);

        return Task.CompletedTask;
    }
}