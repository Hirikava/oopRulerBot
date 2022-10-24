using Discord.Interactions;
using Vostok.Logging.Abstractions;

namespace OopRulerBot.DiscordControllers;

public class RolesController : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ILog log;

    public RolesController(ILog log)
    {
        this.log = log;
    }

    [SlashCommand("giverole", "Выдаёт участнику выбранную на сервере роль", runMode: RunMode.Async)]
    public async Task GiveRoleToUser(ulong roleId)
    {
        log.Info("User:{userName} requested role with id:{roleId} on server:{serverId}",
            Context.User.Username,
            roleId,
            Context.Guild.Id);
        await RespondAsync("Роль удачна выдана", ephemeral: true);
    }
}