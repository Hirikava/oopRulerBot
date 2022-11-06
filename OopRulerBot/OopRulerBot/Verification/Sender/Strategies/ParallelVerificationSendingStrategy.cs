using OopRulerBot.Verification.Transport;
using Vostok.Logging.Abstractions;

namespace OopRulerBot.Verification.Sender.Strategies;

public class ParallelVerificationSendingStrategy : IVerificationSendingStrategy
{
    private readonly ILog log;
    private readonly IVerificationTransport[] verificationTransports;

    public ParallelVerificationSendingStrategy(ILog log, IEnumerable<IVerificationTransport> verificationTransports)
    {
        this.log = log.ForContext(nameof(ParallelVerificationSendingStrategy));
        this.verificationTransports = verificationTransports.ToArray();
    }

    public async Task<bool> Execute(string identifier, int verificationCode)
    {
        log.Info("Available transports {VerificationTransportsCount}", verificationTransports.Length);

        var tasks = verificationTransports.Select(async transport =>
        {
            var transportName = transport.GetType().FullName;

            log.Info("Sending verification using '{VerificationTransportName}'", transportName);

            var success = await transport.SendVerificationCode(identifier, verificationCode);
            return (success, transportName);
        }).ToList();

        while (tasks.Count != 0)
        {
            var completedTask = await Task.WhenAny(tasks);
            
            if (completedTask.IsCompletedSuccessfully)
            {
                var (success, transportName) = completedTask.Result;
                if (success)
                {
                    log.Info("Received success from '{VerificationTransportName}'", transportName);
                    return true;
                }
            }

            tasks.Remove(completedTask);
        }

        log.Warn("All transports responded with failure");
        return false;
    }
}