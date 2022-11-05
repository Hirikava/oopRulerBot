using OopRulerBot.Verification.Transport;

namespace OopRulerBot.Verification.Strategies;

public interface IVerificationStrategy
{
    Task<bool> SendAsync(IEnumerable<IVerificationTransport> verificationTransport, IReadOnlyDictionary<string, string> identifiers, int verificationCode);
}