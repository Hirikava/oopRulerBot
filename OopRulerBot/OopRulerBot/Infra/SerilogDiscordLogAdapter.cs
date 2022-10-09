using Discord;
using Serilog;
using Serilog.Events;

namespace OopRulerBot.Infra;

public class SerilogDiscordLogAdapter : IDiscordLogAdapter
{
    private readonly ILogger log;

    public SerilogDiscordLogAdapter(ILogger log)
    {
        this.log = log;
    }

    public Task HandleLogEvent(LogMessage logEvent)
    {
        log.Write(GetLogLevel(logEvent.Severity), logEvent.Exception,
            "{Source}: {Message}", logEvent.Source, logEvent.Message);
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
