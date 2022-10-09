using Discord;

namespace OopRulerBot.Infra;

public interface IDiscordLogAdapter
{
   Task HandleLogEvent(LogMessage logMessage);
}