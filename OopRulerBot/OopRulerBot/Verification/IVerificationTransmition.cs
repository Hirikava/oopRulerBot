namespace OopRulerBot.Verification;

public interface IVerificationTransmition
{
    Task SendVerificationCode(string identifier, int code);
}