using Discord;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Parsing;


namespace OopRulerBot.Infra;

public class SeriDiscordLogAdapter : IDiscordLogAdapter
{
    private readonly ILogger _log;
    public SeriDiscordLogAdapter(ILogger log)
    {
        _log = log.ForContext("DiscordBot", null);

    }

    public Task HandleLogEvent(LogMessage logMessage)
    {
        var logLevel = GetLogLevel(logMessage.Severity);
        var logEvent = new LogEvent(DateTime.UtcNow, logLevel, logMessage.Exception,  
            new MessageTemplate(logMessage.Message, new List<MessageTemplateToken>()), 
            new List<LogEventProperty>());
        _log.Write(logEvent);
        return Task.CompletedTask;
    }

    private static LogEventLevel GetLogLevel(LogSeverity logSeverity) => logSeverity switch
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