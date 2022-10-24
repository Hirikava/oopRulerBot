namespace OopRulerBot.Verification;

public class InMemoryVerificationStorage : IVerificationStorage
{
    public async Task Save(ulong discordGuildId, ulong discordRoleId, ulong discordUserId, int code, TimeSpan timeToLive)
    {
    }

    public async Task<(int? code, ulong roleId)> GetVerificationCode(ulong discordGuildId, ulong discordUserId)
    {
        return (111111, 123123123);
    }
}