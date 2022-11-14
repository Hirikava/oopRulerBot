namespace OopRulerBot.Verification.TelegramTransport;

public interface IVerificationTransport
{
    Task<bool> SendVerificationCode(string identifier, int code);
}