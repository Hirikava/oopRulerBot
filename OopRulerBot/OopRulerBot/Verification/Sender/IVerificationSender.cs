namespace OopRulerBot.Verification.Sender;

public interface IVerificationSender
{
    Task<bool> SendVerification(string identifier, int verificationCode);
}