﻿namespace OopRulerBot.Settings;

public class SmtpSettings
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";

    public string Host { get; set; } = "";
    public int Port { get; set; }
}