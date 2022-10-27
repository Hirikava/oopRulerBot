namespace OopRulerBot.Telegram;
public class TelegramUserInfo
{
    public string UserName { get; set; } = null!;
    public long UserId { get; set; }
}

public interface ITelegramUsersStorage
{
    Task<TelegramUserInfo?> GetUserInfo(string userName);
    Task SaveUserInfo(TelegramUserInfo telegramUserInfo);
}