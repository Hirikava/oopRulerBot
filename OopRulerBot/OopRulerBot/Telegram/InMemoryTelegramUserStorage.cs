using System.Runtime.Caching;

namespace OopRulerBot.Telegram;

public class InMemoryTelegramUserStorage : ITelegramUsersStorage
{
    public Task<TelegramUserInfo?> GetUserInfo(string userName)
    {
        var key = $"telegram-user-info-{userName}";
        var cacheItem =  MemoryCache.Default.Get(key);
        return Task.FromResult(cacheItem as TelegramUserInfo);
    }

    public Task SaveUserInfo(TelegramUserInfo telegramUserInfo)
    {
        var key = $"telegram-user-info-{telegramUserInfo.UserName}";
        MemoryCache.Default.Add(key, telegramUserInfo, new CacheItemPolicy()
        {
            SlidingExpiration = TimeSpan.FromHours(1)
        });
        return Task.CompletedTask;
    }
}