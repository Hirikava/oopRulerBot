namespace OopRulerBot.Verification.SmtpTransport;

public interface ISmtpTransport
{
    Task<bool> SendVerificationCode(string identifier, int code);
}