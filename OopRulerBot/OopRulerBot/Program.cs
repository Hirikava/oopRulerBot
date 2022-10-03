// See https://aka.ms/new-console-template for more information

using Autofac;
using Discord;
using Discord.WebSocket;
using OopRulerBot.DI;
using OopRulerBot.Infra;


namespace OopRulerBot;

public static class Program
{
    public const string Token = "MTAyMTM3ODQ3NjA4MDY0ODI4Mg.GwqFzN.bRrUmTfMQLYszKcnx4HA8hbeMd02Psxn0Mdmdw";
    public static async Task Main(string[] args)
    {
        var container = BotContainerBuilder.Build();
        var discordClient = new DiscordSocketClient();
        
        discordClient.Log += container.Resolve<IDiscordLogAdapter>().HandleLogEvent;
        await discordClient.LoginAsync(TokenType.Bot, Token);
        await discordClient.StartAsync();
        await Task.Delay(-1);
    }
}