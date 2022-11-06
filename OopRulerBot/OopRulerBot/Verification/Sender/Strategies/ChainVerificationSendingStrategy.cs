using OopRulerBot.Verification.Transport;
using Vostok.Logging.Abstractions;

namespace OopRulerBot.Verification.Sender.Strategies;

public class ChainVerificationSendingStrategy : IVerificationSendingStrategy
{
    private readonly ILog log;
    private readonly IVerificationTransport[] verificationTransports;

    public ChainVerificationSendingStrategy(ILog log, IEnumerable<IVerificationTransport> verificationTransports)
    {
        this.log = log.ForContext(nameof(ChainVerificationSendingStrategy));
        this.verificationTransports = verificationTransports.ToArray();
    }

    public async Task<bool> Execute(string identifier, int verificationCode)
    {
        log.Info("Available transports {VerificationTransportsCount}", verificationTransports.Length);

        for (var i = 0; i < verificationTransports.Length; i++)
        {
            var verificationTransport = verificationTransports[i];
            var verificationTransportName = verificationTransport.GetType().FullName;

            log.Info("Attempt {Attempt}/{VerificationTransportsCount}. Using '{VerificationTransportName}'", i,
                verificationTransports.Length, verificationTransportName);

            var sendVerificationResult = await verificationTransport.SendVerificationCode(identifier, verificationCode);

            if (sendVerificationResult)
            {
                log.Info("Received success from '{VerificationTransportName}'", verificationTransportName);
                return true;
            }
        }

        log.Warn("All transports responded with failure");
        return false;
    }
}