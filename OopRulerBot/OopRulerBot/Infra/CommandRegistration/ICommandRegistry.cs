using Discord.WebSocket;

namespace OopRulerBot.Infra.CommandRegistration;

public interface ICommandRegistry
{
    Task RegisterCommandsOnReady();
    Task RegisterCommandOnJoinedServer(SocketGuild socketGuild);
}