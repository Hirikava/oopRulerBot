namespace OopRulerBot.Settings;

public class SmtpSettings
{
    public string SenderMailAddress { get; set; } = "";
    public string Host { get; set; } = "";
    public int Port { get; set; } = 0;
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
}