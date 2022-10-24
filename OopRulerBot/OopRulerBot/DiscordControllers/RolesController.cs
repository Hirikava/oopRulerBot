using System.ComponentModel;
using Discord.Interactions;
using Discord.WebSocket;
using OopRulerBot.Verification;
using Vostok.Logging.Abstractions;

namespace OopRulerBot.DiscordControllers;

public class RolesController : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ILog log;
    private readonly IVerificationService verificationService;

    public RolesController(
        ILog log,
        IVerificationService verificationService)
    {
        this.log = log;
        this.verificationService = verificationService;
    }

    [SlashCommand("requestrole", "123123", runMode: RunMode.Async)]
    public async Task RequestRoleForUser(string roleName,
        [Description("Имя пользователя должно начинаться с @")] string telegramUserName)
    {
        log.Info("User:{userName} requested role with name:{roleName} on server:{serverId}",
            Context.User.Username,
            roleName,
            Context.Guild.Id);

        var role = Context.Guild.Roles
            .FirstOrDefault(x =>
                x.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase) && x.Permissions.Administrator == false);

        if (role == null)
        {
            await RespondAsync("Роль не найдена или является ролью администротора", ephemeral: true);
            return;
        }

        await verificationService.SendVerification(Context.Guild.Id, role.Id, Context.User.Id, telegramUserName);
    }

    [SlashCommand("confirmrole", "123123")]
    public async Task ConfirmRoleForUser(int verificationCode)
    {
        var (result, role) = await verificationService.ConfirmVerification(Context.Guild.Id, Context.User.Id, verificationCode);
        if (result)
        {
            var guildUser = (SocketGuildUser)Context.User;
            //await guildUser.AddRoleAsync(role.Value);
            await RespondAsync("Роль удачна выдана", ephemeral: true);
        }
        else
        {
            await RespondAsync("Код введён не верно или истёк", ephemeral: true);
        }
    }
}