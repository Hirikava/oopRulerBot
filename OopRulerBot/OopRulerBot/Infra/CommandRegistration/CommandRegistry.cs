using Discord.Interactions;
using Discord.WebSocket;

namespace OopRulerBot.Infra.CommandRegistration;

public class CommandRegistry : ICommandRegistry
{

    private readonly DiscordSocketClient discordSocketClient;
    private readonly InteractionService interactionService;

    public CommandRegistry(
        InteractionService interactionService, 
        DiscordSocketClient discordSocketClient)
    {
        this.interactionService = interactionService;
        this.discordSocketClient = discordSocketClient;
    }

    public async Task RegisterCommandsOnReady()
    {
        var registrationTasks = discordSocketClient.Guilds.AsParallel()
            .Select(async guild =>
            {
                await guild.DeleteApplicationCommandsAsync();
                return await interactionService.RegisterCommandsToGuildAsync(guild.Id);
            })
            .ToArray();
        await Task.WhenAll(registrationTasks);
    }

    public async Task RegisterCommandOnJoinedServer(SocketGuild socketGuild)
    {
        await socketGuild.DeleteApplicationCommandsAsync();
        await interactionService.RegisterCommandsToGuildAsync(socketGuild.Id);
    }
}