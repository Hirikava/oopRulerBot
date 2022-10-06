using Discord;
using Serilog;
using Serilog.Events;

namespace OopRulerBot.Infra;

public class SerilogDiscordLogAdapter : IDiscordLogAdapter
{
    private readonly ILogger _log;

    public SerilogDiscordLogAdapter(ILogger log)
    {
        _log = log;
    }

    public Task HandleLogEvent(LogMessage logEvent)
    {
        _log.Write(
            GetLogLevel(logEvent.Severity),
            logEvent.Exception,
            "[{Source}] {Message}",
            logEvent.Source,
            logEvent.Message);
        return Task.CompletedTask;
    }

    private static LogEventLevel GetLogLevel(LogSeverity logSeverity) => logSeverity switch
    {
        LogSeverity.Critical => LogEventLevel.Fatal,
        LogSeverity.Error => LogEventLevel.Error,
        LogSeverity.Warning => LogEventLevel.Warning,
        LogSeverity.Info => LogEventLevel.Information,
        LogSeverity.Debug => LogEventLevel.Debug,
        LogSeverity.Verbose => LogEventLevel.Verbose,
        _ => throw new ArgumentOutOfRangeException(nameof(logSeverity), logSeverity, null)
    };
}