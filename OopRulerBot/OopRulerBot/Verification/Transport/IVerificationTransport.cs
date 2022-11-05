namespace OopRulerBot.Verification.Transport;

public interface IVerificationTransport
{
    Task<bool> SendVerificationCode(string identifier, int code);
    string Name { get; }
}