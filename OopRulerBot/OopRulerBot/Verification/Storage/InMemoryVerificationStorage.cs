using System.Runtime.Caching;

namespace OopRulerBot.Verification.Storage;

public class InMemoryVerificationStorage : IVerificationStorage
{
    public Task<bool> AddVerificationCode(
        ulong discordGuildId,
        ulong discordRoleId,
        ulong discordUserId,
        int code,
        TimeSpan? ttl)
    {
        var timeToLive = ttl ?? TimeSpan.FromMinutes(5);
        var memoryCacheKey = GetCacheKey(discordGuildId, discordUserId);
        var verification = new Verification
        {
            RoleId = discordRoleId,
            Code = code
        };
        return Task.FromResult(MemoryCache.Default.Add(memoryCacheKey, verification, new CacheItemPolicy
        {
            SlidingExpiration = timeToLive,
            Priority = CacheItemPriority.Default
        }));
    }

    public Task<bool> DeleteVerificationCode(ulong discordGuildId, ulong discordUserId)
    {
        var memoryCacheKey = GetCacheKey(discordGuildId, discordUserId);
        return Task.FromResult(MemoryCache.Default.Remove(memoryCacheKey) != null);
    }

    public Task<Verification?> GetVerificationCode(ulong discordGuildId, ulong discordUserId)
    {
        var cacheKey = GetCacheKey(discordGuildId, discordUserId);
        var verification = MemoryCache.Default.Get(cacheKey);
        return Task.FromResult((Verification)verification);
    }

    private string GetCacheKey(ulong discordGuildId, ulong discordUserId)
    {
        return $"{discordGuildId}-{discordUserId}";
    }
}