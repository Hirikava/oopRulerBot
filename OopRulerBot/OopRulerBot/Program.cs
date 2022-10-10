// See https://aka.ms/new-console-template for more information

using System.Reflection;
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

        var discordSocketClient = Container.Resolve<DiscordSocketClient>();
        discordSocketClient.Log += Container.Resolve<IDiscordLogAdapter>().HandleLogEvent;

        var discordMessageHandler = Container.Resolve<IDiscordMessageHandler>();
        discordSocketClient.MessageReceived += discordMessageHandler.HandleMessage;

        var discordToken = Container
            .ResolveNamed<IConfigurationProvider>(ConfigurationScopes.BotSettingsScope)
            .Get<BotSecretSettings>().DiscordToken;

        var commandService = Container.Resolve<CommandService>();
        await commandService.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), 
            services: null);
        
        await discordSocketClient.LoginAsync(TokenType.Bot, discordToken);
        await discordSocketClient.StartAsync();
        await Task.Delay(-1);
    }

    public static async Task HandleMessage(SocketMessage socketMessage)
    {
        var discordSocketClient = Container.Resolve<DiscordSocketClient>();
        var commandService = Container.Resolve<CommandService>();

        var message = socketMessage as SocketUserMessage;
        if (message == null) return;

        // Create a number to track where the prefix ends and the command begins
        int argPos = 0;

        // Determine if the message is a command based on the prefix and make sure no bots trigger commands
        if (!(message.HasCharPrefix('!', ref argPos) || 
              message.HasMentionPrefix(discordSocketClient.CurrentUser, ref argPos)) ||
            message.Author.IsBot)
            return;

        // Create a WebSocket-based command context based on the message
        var context = new SocketCommandContext(discordSocketClient, message);

        // Execute the command with the command context we just
        // created, along with the service provider for precondition checks.
        await commandService.ExecuteAsync(
            context: context, 
            argPos: argPos,
            services: null);
    }
}