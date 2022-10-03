// See https://aka.ms/new-console-template for more information

using Discord;
using Discord.WebSocket;
using OopRulerBot.Infra;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Console;

public static class Program
{

    public const string Token = "MTAyMTM3ODQ3NjA4MDY0ODI4Mg.GPyWDe.JRoLCmeDjf2aVIsKA8Ty2o13aHt6y3Rnb1uGdM";
    public static async Task Main(string[] args)
    {
        var consoleLog = new ConsoleLog();
        var discordLogAdapter = new VostokDiscordLogAdapter(consoleLog);
        var discordClient = new DiscordSocketClient();
        discordClient.Log += discordLogAdapter.HandleLogEvent;
       
        await discordClient.LoginAsync(TokenType.Bot, Token);
        await discordClient.StartAsync();
        await Task.Delay(-1);
    }
}