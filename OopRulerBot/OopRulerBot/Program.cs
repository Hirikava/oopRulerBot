// See https://aka.ms/new-console-template for more information

using Autofac;
using Discord;
using Discord.WebSocket;
using OopRulerBot.DI;
using OopRulerBot.Infra;
using OopRulerBot.Settings;
using Vostok.Configuration;
using Vostok.Configuration.Abstractions;
using Vostok.Configuration.Sources.Json;


namespace OopRulerBot;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var container = BotContainerBuilder.Build();
        var discordClient = new DiscordSocketClient();
        
        discordClient.Log += container.Resolve<IDiscordLogAdapter>().HandleLogEvent;
        var discordToken = container.ResolveNamed<IConfigurationProvider>(ConfigurationScopes.BotSettingsScope)
            .Get<BotSecretSettings>().DiscordToken;
        await discordClient.LoginAsync(TokenType.Bot, discordToken);
        await discordClient.StartAsync();
        await Task.Delay(-1);
    }
}