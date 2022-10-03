using Discord;
using Vostok.Logging.Abstractions;

namespace OopRulerBot.Infra;

public class VostokDiscordLogAdapter : IDiscordLogAdapter
{
    private readonly ILog log;
    public VostokDiscordLogAdapter(ILog log)
    {
        this.log = log.ForContext("DiscordApi");
    }

    public Task HandleLogEvent(LogMessage logMessage)
    {
        var logEvent = new LogEvent(GetLogLevel(logMessage.Severity), DateTime.UtcNow, logMessage.Message,
            logMessage.Exception);
        log.Log(logEvent);
        return Task.CompletedTask;
    }

    private LogLevel GetLogLevel(LogSeverity logSeverity) => logSeverity switch
    {
        LogSeverity.Critical => LogLevel.Fatal,
        LogSeverity.Error => LogLevel.Error,
        LogSeverity.Warning => LogLevel.Warn,
        LogSeverity.Info => LogLevel.Info,
        LogSeverity.Verbose => LogLevel.Info,
        LogSeverity.Debug => LogLevel.Debug,
        _ => throw new ArgumentOutOfRangeException(nameof(logSeverity), logSeverity, null)
    };
}