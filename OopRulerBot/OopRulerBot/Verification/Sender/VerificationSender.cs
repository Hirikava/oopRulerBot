using Vostok.Logging.Abstractions;

namespace OopRulerBot.Verification.Sender;

public class VerificationSender : IVerificationSender
{
    private readonly ILog log;
    private readonly IVerificationSendingStrategy sendingStrategy;

    public VerificationSender(ILog log, IVerificationSendingStrategy sendingStrategy)
    {
        this.log = log.ForContext(nameof(VerificationSender));
        this.sendingStrategy = sendingStrategy;
    }

    public async Task<bool> SendVerification(string identifier, int verificationCode)
    {
        log.Info("Sending verification");
        return await sendingStrategy.Execute(identifier, verificationCode);
    }
}