using Discord;
using Serilog;
using Serilog.Events;

namespace OopRulerBot.Infra;

public class SerilogDiscordLogAdapter: IDiscordLogAdapter
{
    private readonly ILogger logger;
    public SerilogDiscordLogAdapter(ILogger logger)
    {
        this.logger = logger;
    }
    public Task HandleLogEvent(LogMessage logMessage)
    {
        logger.Write(GetLogLevel(logMessage.Severity), logMessage.Exception, logMessage.Message);
        return Task.CompletedTask;
    }
    
    private LogEventLevel GetLogLevel(LogSeverity logSeverity) => logSeverity switch
    {
        LogSeverity.Critical => LogEventLevel.Fatal,
        LogSeverity.Error => LogEventLevel.Error,
        LogSeverity.Warning => LogEventLevel.Warning,
        LogSeverity.Info => LogEventLevel.Information,
        LogSeverity.Verbose => LogEventLevel.Verbose,
        LogSeverity.Debug => LogEventLevel.Debug,
        _ => throw new ArgumentOutOfRangeException(nameof(logSeverity), logSeverity, null)
    };
}