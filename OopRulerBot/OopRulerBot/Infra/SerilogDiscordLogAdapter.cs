using Discord;
using Serilog;
using Serilog.Events;

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
        logger.Write(
            GetLevel(logEvent.Severity),
            logEvent.Exception,
            $"({logEvent.Source}) {logEvent.Message}"
        );
        return Task.CompletedTask;
    }

    private static LogEventLevel GetLevel(LogSeverity severity) => severity switch
    {
        LogSeverity.Critical => LogEventLevel.Fatal,
        LogSeverity.Error => LogEventLevel.Error,
        LogSeverity.Warning => LogEventLevel.Warning,
        LogSeverity.Info => LogEventLevel.Information,
        LogSeverity.Verbose => LogEventLevel.Verbose,
        LogSeverity.Debug => LogEventLevel.Debug,
        _ => throw new ArgumentOutOfRangeException(nameof(severity), severity, null)
    };
}