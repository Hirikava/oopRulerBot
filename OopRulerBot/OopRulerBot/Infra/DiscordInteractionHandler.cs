using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace OopRulerBot.Infra;

public class DiscordInteractionHandler
{
    private readonly DiscordSocketClient DiscordSocketClient;
    private readonly InteractionService InteractionService;
    private readonly IServiceProvider ServiceProvider;

    public DiscordInteractionHandler(
        InteractionService interactionService,
        DiscordSocketClient discordSocketClient,
        IServiceProvider serviceProvider)
    {
        InteractionService = interactionService;
        DiscordSocketClient = discordSocketClient;
        ServiceProvider = serviceProvider;
    }

    public async Task Handle(SocketInteraction interaction)
    {
        switch (interaction.Type)
        {
            case InteractionType.Ping:
            case InteractionType.ApplicationCommand:
            case InteractionType.ApplicationCommandAutocomplete:
            case InteractionType.ModalSubmit:
                var interactionContext = new SocketInteractionContext(DiscordSocketClient, interaction);
                await InteractionService.ExecuteCommandAsync(interactionContext, ServiceProvider);
                break;
            case InteractionType.MessageComponent:
                var socketMessageComponent = (SocketMessageComponent)interaction;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}