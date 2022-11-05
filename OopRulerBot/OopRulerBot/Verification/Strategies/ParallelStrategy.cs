using OopRulerBot.Verification.Transport;

namespace OopRulerBot.Verification.Strategies;

public class ParallelStrategy : IVerificationStrategy
{
    public async Task<bool> SendAsync(IEnumerable<IVerificationTransport> verificationTransport, IReadOnlyDictionary<string, string> identifiers, int verificationCode)
    {
        var results = await Task.WhenAll(verificationTransport.Select(x => TrySendSingle(x, identifiers, verificationCode)));

        return results.Any(x => x);
    }

    private static async Task<bool> TrySendSingle(IVerificationTransport verificationTransport, IReadOnlyDictionary<string, string> identifiers, int code)
    {
        if (!identifiers.TryGetValue(verificationTransport.Name, out var identifier))
            return false;

        return await verificationTransport.SendVerificationCode(identifier, code);
    }
}