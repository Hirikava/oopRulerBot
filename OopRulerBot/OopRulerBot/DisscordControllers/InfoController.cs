using Discord.Commands;
using Vostok.Logging.Abstractions;

namespace OopRulerBot.DisscordControllers;

public class InfoController : ModuleBase<SocketCommandContext>
{
    private readonly ILog log;

    public InfoController(ILog log)
    {
        this.log = log;
    }

    [Command("say")]
    [Summary("Echoes a message.")]
    public async Task SayAsync([Remainder] [Summary("The text to echo")] string echo)
    {
        await ReplyAsync(echo);
    }
}