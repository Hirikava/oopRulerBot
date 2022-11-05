using OopRulerBot.Verification.Transport;

namespace OopRulerBot.Verification.Strategies;

public class SequentialStrategy : IVerificationStrategy
{
    public async Task<bool> SendAsync(IEnumerable<IVerificationTransport> verificationTransport, IReadOnlyDictionary<string, string> identifiers, int verificationCode)
    {
        foreach (var transport in verificationTransport)
        {
            if(!identifiers.TryGetValue(transport.Name, out var identifier))
                continue;
            var result = await transport.SendVerificationCode(identifier, verificationCode);
            if (result)
                return true;
        }

        return false;
    }
}