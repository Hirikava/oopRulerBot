namespace OopRulerBot.Verification;

public interface IVerificationService
{
    Task SendVerification(ulong discordGuildId, ulong discordRoleId, ulong discordUserId, string identifier);
    Task<(bool, ulong?)> ConfirmVerification(ulong discordGuildId, ulong discordUserId, int verificationCode);
}