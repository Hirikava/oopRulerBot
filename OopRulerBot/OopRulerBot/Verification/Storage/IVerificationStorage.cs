namespace OopRulerBot.Verification.Storage;


public class Verification
{
    public ulong RoleId { get; set; }
    public int Code { get; set; }
}
public interface IVerificationStorage
{
    public Task<bool> AddVerificationCode(ulong discordGuildId, ulong discordRoleId, ulong discordUserId, int code, TimeSpan? ttl = null);
    public Task<bool> DeleteVerificationCode(ulong discordGuildId, ulong discordUserId);
    public Task<Verification?> GetVerificationCode(ulong discordGuildId, ulong discordUserId);
}