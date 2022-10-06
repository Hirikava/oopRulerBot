using Discord;
using Serilog;
using Serilog.Events;
using Serilog.Parsing;

namespace OopRulerBot.Infra;

public class SerilogDiscordLogAdapter : IDiscordLogAdapter
{
    private readonly ILogger logger;

    public SerilogDiscordLogAdapter(ILogger logger)
    {
        this.logger = logger;
    }

    public Task HandleLogEvent(LogMessage logEvent)
    {
        logger.Write(MapEvent(logEvent));
        return Task.CompletedTask;
    }

    private static LogEvent MapEvent(LogMessage logEvent)
    {
        return new LogEvent(DateTimeOffset.UtcNow,
            GetLogLevel(logEvent.Severity),
            logEvent.Exception,
            new MessageTemplate(new[] { new TextToken(logEvent.Message) }),
            Enumerable.Empty<LogEventProperty>());
    }

    private static LogEventLevel GetLogLevel(LogSeverity logEventSeverity)
    {
        return logEventSeverity switch
        {
            LogSeverity.Critical => LogEventLevel.Fatal,
            LogSeverity.Error => LogEventLevel.Error,
            LogSeverity.Warning => LogEventLevel.Warning,
            LogSeverity.Info => LogEventLevel.Information,
            LogSeverity.Verbose => LogEventLevel.Verbose,
            LogSeverity.Debug => LogEventLevel.Debug,
            _ => throw new ArgumentOutOfRangeException(nameof(logEventSeverity), logEventSeverity, null)
        };
    }
}