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
        [Description("Имя пользователя должно начинаться с @")]
        string? telegramUserName = null,
        string? email = null)
    {
        log.Info("User:{userName} requested role with name:{roleName} on server:{serverId}",
            Context.User.Username,
            roleName,
            Context.Guild.Id);
        if (telegramUserName is null && email is null)
        {
            await RespondAsync("Нужен хотя бы один идентификатор", ephemeral: true);
            return;
        }

        var role = Context.Guild.Roles
            .FirstOrDefault(x =>
                x.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase) && x.Permissions.Administrator == false);
        if (role == null)
        {
            await RespondAsync("Роль не найдена или является ролью администротора", ephemeral: true);
            return;
        }

        var guildUser = (SocketGuildUser)Context.User;
        if (guildUser.Roles.Contains(role))
        {
            await RespondAsync("У вас уже есть запрашиваемая роль.", ephemeral: true);
            return;
        }

        await DeferAsync(ephemeral: true);
        var identifiers = new Dictionary<string, string>(2);
        if (telegramUserName is not null)
            identifiers["Telegram"] = telegramUserName;
        if (email is not null)
            identifiers["Mail"] = email;
        var verificationStatus =
            await verificationService.SendVerification(Context.Guild.Id, role.Id, Context.User.Id, identifiers);
        switch (verificationStatus)
        {
            case SendVerificationStatus.Success:
                await FollowupAsync("Вам выслан код подтверждения", ephemeral: true);
                break;
            case SendVerificationStatus.UserAlreadyHasAnotherVerificationOnCurrentGuild:
                await FollowupAsync("Нельзя запрашивать больше одной роли за раз, подвердите предыдущий запрос.",
                    ephemeral: true);
                break;
            case SendVerificationStatus.TransportError:
                await FollowupAsync("Мы не смогли доставить вам код подтверждения.", ephemeral: true);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    [SlashCommand("confirmrole", "123123")]
    public async Task ConfirmRoleForUser(int verificationCode)
    {
        var verificationResult =
            await verificationService.ConfirmVerification(Context.Guild.Id, Context.User.Id, verificationCode);
        switch (verificationResult.Status)
        {
            case ConfirmVerificationStatus.TimedOut:
            case ConfirmVerificationStatus.WrongCode:
                await RespondAsync("Введён не верный код", ephemeral: true);
                break;
            case ConfirmVerificationStatus.Success:
                var guildUser = (SocketGuildUser)Context.User;
                await guildUser.AddRoleAsync(verificationResult.RoleId);
                await RespondAsync("Роль выдана", ephemeral: true);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}