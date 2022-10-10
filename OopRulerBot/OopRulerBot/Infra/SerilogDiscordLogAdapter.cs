using Discord;
using Serilog.Events;

namespace OopRulerBot.Infra;
using Serilog;

public class SerilogDiscordLogAdapter : IDiscordLogAdapter
{
    private readonly ILogger log;
    public SerilogDiscordLogAdapter(ILogger log)
    {
        this.log = log.ForContext("DiscordApi", null);
    }

    public Task HandleLogEvent(LogMessage logMessage)
    {
        log.Write(GetLogLevel(logMessage.Severity),logMessage.Exception, logMessage.Message);
        return Task.CompletedTask;
    }

    private LogEventLevel GetLogLevel(LogSeverity logSeverity) => logSeverity switch
    {
        LogSeverity.Critical => LogEventLevel.Fatal,
        LogSeverity.Error => LogEventLevel.Error,
        LogSeverity.Warning => LogEventLevel.Warning,
        LogSeverity.Info => LogEventLevel.Information,
        LogSeverity.Verbose => LogEventLevel.Information,
        LogSeverity.Debug => LogEventLevel.Debug,
        _ => throw new ArgumentOutOfRangeException(nameof(logSeverity), logSeverity, null)
    };
}