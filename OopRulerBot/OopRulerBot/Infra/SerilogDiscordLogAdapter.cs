using Discord;
using Serilog;


namespace OopRulerBot.Infra;

public class SerilogDiscordLogAdapter : IDiscordLogAdapter
{
    private readonly ILogger log;
    
    public SerilogDiscordLogAdapter(ILogger log)
    {
        this.log = log;
    }

    public Task HandleLogEvent(LogMessage logMessage)
    {
        switch (logMessage.Severity)
        {
            case LogSeverity.Critical:
                log.Fatal(logMessage.Exception, logMessage.Message);
                break;
            case LogSeverity.Error:
                log.Error(logMessage.Exception, logMessage.Message);
                break;
            case LogSeverity.Warning:
                log.Warning(logMessage.Exception, logMessage.Message);
                break;
            case LogSeverity.Info:
                log.Information(logMessage.Exception, logMessage.Message);
                break;
            case LogSeverity.Verbose:
                log.Verbose(logMessage.Exception, logMessage.Message);
                break;
            case LogSeverity.Debug:
                log.Debug(logMessage.Exception, logMessage.Message);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return Task.CompletedTask;
    }
}