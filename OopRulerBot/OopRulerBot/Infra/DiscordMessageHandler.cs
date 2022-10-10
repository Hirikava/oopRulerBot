using Discord.Commands;
using Discord.WebSocket;

namespace OopRulerBot.Infra;

public class DiscordMessageHandler : IDiscordMessageHandler
{
    private readonly CommandService commandService;
    private readonly DiscordSocketClient discordSocketClient;

    public DiscordMessageHandler(DiscordSocketClient discordSocketClient, CommandService commandService)
    {
        this.discordSocketClient = discordSocketClient;
        this.commandService = commandService;
    }

    public async Task HandleMessage(SocketMessage socketMessage)
    {
        var message = socketMessage as SocketUserMessage;
        if (message == null) return;

        // Create a number to track where the prefix ends and the command begins
        var argPos = 0;

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
            context,
            argPos,
            null);
    }
}