﻿namespace OopRulerBot.Settings;

public class BotSecretSettings
{
    public string DiscordToken { get; set; } = "";
    public string TelegramToken { get; set; } = "";
    public SmtpSettings SmtpClientSettings { get; set; } = new();
}