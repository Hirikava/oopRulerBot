namespace OopRulerBot.Verification;

public interface IVerificationStorage
{
    public Task Save(ulong discordGuildId, ulong discordRoleId, ulong discordUserId, int code, TimeSpan timeToLive);
    public Task<(int? code, ulong roleId)> GetVerificationCode(ulong discordGuildId, ulong discordUserId);
}