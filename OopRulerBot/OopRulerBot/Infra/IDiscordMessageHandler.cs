using Discord.WebSocket;

namespace OopRulerBot.Infra;

public interface IDiscordMessageHandler
{
    Task HandleMessage(SocketMessage socketMessage);
}