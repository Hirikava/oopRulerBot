namespace OopRulerBot.Verification.Sender;

public interface IVerificationSendingStrategy
{
    Task<bool> Execute(string identifier, int verificationCode);
}