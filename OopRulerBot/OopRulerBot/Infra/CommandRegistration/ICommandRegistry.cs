using Discord.WebSocket;

namespace OopRulerBot.Infra;

public interface ICommandRegistry
{
    Task RegisterCommandsOnExistingServers();
    Task RegisterCommandOnJoinedServer(SocketGuild socketGuild);
}