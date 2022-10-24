using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace OopRulerBot.Infra.CommandRegistration;

public class CommandRegistry : ICommandRegistry
{
    private readonly DiscordSocketClient discordSocketClient;
    private readonly InteractionService interactionService;

    public CommandRegistry(
        DiscordSocketClient discordSocketClient,
        InteractionService interactionService)
    {
        this.discordSocketClient = discordSocketClient;
        this.interactionService = interactionService;
    }


    public async Task RegisterCommandsOnExistingServers()
    {
        var registryTasks = discordSocketClient
            .Guilds.AsParallel().Select(async guild =>
            {
                await guild.DeleteApplicationCommandsAsync();
                await RegisterNewCommands(guild);
            }).ToArray();
        await Task.WhenAll(registryTasks);
    }

    public async Task RegisterCommandOnJoinedServer(SocketGuild socketGuild)
    {
        await socketGuild.DeleteApplicationCommandsAsync();
        await RegisterNewCommands(socketGuild);
    }


    private async Task RegisterNewCommands(SocketGuild socketGuild)
    {
        await socketGuild.DeleteApplicationCommandsAsync();
        var slashCommands = interactionService.Modules.Select(x => x.SlashCommands)
            .SelectMany(x => x)
            .ToArray();

        var commandRegistrationTasks = slashCommands.Select(commandInfo =>
        {
            var slashCommandBuilder = new SlashCommandBuilder()
                .WithName(commandInfo.Name)
                .WithDescription(commandInfo.Description)
                .WithDefaultMemberPermissions(commandInfo.DefaultMemberPermissions);
            foreach (var parameter in commandInfo.Parameters)
                slashCommandBuilder.AddOption(
                    parameter.Name,
                    (ApplicationCommandOptionType)parameter.DiscordOptionType,
                    parameter.Description);
            return socketGuild.CreateApplicationCommandAsync(slashCommandBuilder.Build());
        });

        await Task.WhenAll(commandRegistrationTasks);
    }
}