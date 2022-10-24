using Discord.Interactions;
using Discord.WebSocket;

namespace OopRulerBot.Infra.CommandRegistration;

public class CommandRegistry : ICommandRegistry
{
    private readonly InteractionService interactionService;

    public CommandRegistry(InteractionService interactionService)
    {
        this.interactionService = interactionService;
    }

    public async Task RegisterCommandsOnReady()
    {
        await interactionService.RegisterCommandsGloballyAsync();
    }

    public async Task RegisterCommandOnGuildJoined(SocketGuild socketGuild)
    {
        await interactionService.RegisterCommandsToGuildAsync(socketGuild.Id);
    }
}