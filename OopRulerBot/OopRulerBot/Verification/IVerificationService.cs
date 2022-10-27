namespace OopRulerBot.Verification;


public enum ConfirmVerificationStatus
{
    TimedOut,
    WrongCode,
    Success,
}


public enum SendVerificationStatus
{
    Success,
    UserAlreadyHasAnotherVerificationOnCurrentGuild,
    TransportError,
}

public class VerificationResult
{
    public ConfirmVerificationStatus Status { get; set; }
    public ulong RoleId { get; set; }
}


public interface IVerificationService
{
    Task<SendVerificationStatus> SendVerification(ulong discordGuildId, ulong discordRoleId, ulong discordUserId, string identifier);
    Task<VerificationResult> ConfirmVerification(ulong discordGuildId, ulong discordUserId, int verificationCode);
    Task<bool> CancelVerification(ulong discordGuildId, ulong discordUserId);
}