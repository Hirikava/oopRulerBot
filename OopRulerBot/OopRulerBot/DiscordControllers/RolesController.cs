using Discord.Interactions;
using Discord.WebSocket;
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
    public async Task GiveRoleToUser(string roleName)
    {
        log.Info("User:{userName} requested role with name:{roleName} on server:{serverId}",
            Context.User.Username,
            roleName,
            Context.Guild.Id);
        var guildUser = (SocketGuildUser)Context.User;
        var role = Context.Guild.Roles
            .FirstOrDefault(x => x.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase) && x.Permissions.Administrator == false);

        if (role == null)
        {
            await RespondAsync("Роль не найдена или является ролью администротора", ephemeral: true);
            return;
        }

        await guildUser.AddRoleAsync(role.Id);
        await RespondAsync("Роль удачна выдана", ephemeral: true);
    }
}