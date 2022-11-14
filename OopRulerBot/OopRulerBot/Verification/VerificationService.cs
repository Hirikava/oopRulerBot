using OopRulerBot.Verification.SmtpTransport;
using OopRulerBot.Verification.Storage;
using OopRulerBot.Verification.TelegramTransport;
using Vostok.Logging.Abstractions;

namespace OopRulerBot.Verification;

public class VerificationService : IVerificationService
{
    private readonly IVerificationStorage verificationStorage;
    private readonly IVerificationTransport verificationTransport;
    private readonly ILog log;
    private readonly ISmtpTransport smtpTransport;

    public VerificationService(
        IVerificationTransport verificationTransport,
        IVerificationStorage verificationStorage,
        ILog log, ISmtpTransport smtpTransport)
    {
        this.verificationTransport = verificationTransport;
        this.verificationStorage = verificationStorage;
        this.log = log;
        this.smtpTransport = smtpTransport;
    }

    public async Task<SendVerificationStatus> SendVerification(
        ulong discordGuildId, 
        ulong discordRoleId, 
        ulong discordUserId,
        string identifier,
        string email)
    {
        var verificationCode = CreateVerificationCode();
        var saveResult = await verificationStorage.AddVerificationCode(discordGuildId, discordRoleId, discordUserId, verificationCode, TimeSpan.FromMinutes(3));
        if (!saveResult)
            return SendVerificationStatus.UserAlreadyHasAnotherVerificationOnCurrentGuild; 
        var sendTelegramVerificationResult = await verificationTransport.SendVerificationCode(identifier, verificationCode);
        var sendSmtpVerificationResult = await smtpTransport.SendVerificationCode(email, verificationCode);
        if (!sendTelegramVerificationResult && !sendSmtpVerificationResult)
            await verificationStorage.DeleteVerificationCode(discordGuildId, discordUserId);
        return sendTelegramVerificationResult || sendSmtpVerificationResult ? SendVerificationStatus.Success : SendVerificationStatus.TransportError;
    }

    public async Task<VerificationResult> ConfirmVerification(
        ulong discordGuildId, 
        ulong discordUserId, 
        int verificationCode)
    {
        var verification = await verificationStorage.GetVerificationCode(discordGuildId, discordUserId);
        if (verification != null && verification.Code == verificationCode)
        {
            await verificationStorage.DeleteVerificationCode(discordGuildId, discordUserId);
            return new VerificationResult()
            {
                Status = ConfirmVerificationStatus.Success,
                RoleId = verification.RoleId,
            };
        }

        return new VerificationResult()
        {
            Status = ConfirmVerificationStatus.WrongCode,
            RoleId = verification.RoleId,
        };
    }

    public async Task<bool> CancelVerification(ulong discordGuildId, ulong discordUserId)
    {
        return await verificationStorage.DeleteVerificationCode(discordGuildId, discordUserId);
    }

    private int CreateVerificationCode()
    {
        var random = new Random();
        return random.Next(1, 999999);
    }
}