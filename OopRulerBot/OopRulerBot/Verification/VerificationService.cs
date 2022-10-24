namespace OopRulerBot.Verification;

public class VerificationService : IVerificationService
{
    private readonly IVerificationStorage storage;
    private readonly IVerificationTransmition verificationTransmition;

    public VerificationService(
        IVerificationTransmition verificationTransmition,
        IVerificationStorage storage)
    {
        this.verificationTransmition = verificationTransmition;
        this.storage = storage;
    }

    public async Task SendVerification(ulong discordGuildId, ulong discordRoleId, ulong discordUserId,
        string identifier)
    {
        var verificationCode = CreateVerificationCode();
        await storage.Save(discordGuildId, discordRoleId, discordUserId, verificationCode, TimeSpan.FromMinutes(3));
        await verificationTransmition.SendVerificationCode(identifier, verificationCode);
    }

    public async Task<(bool, ulong?)> ConfirmVerification(ulong discordGuildId, ulong discordUserId,
        int verificationCode)
    {
        var (correctCode, role) = await storage.GetVerificationCode(discordGuildId, discordUserId);
        if (correctCode == null || correctCode.Value != verificationCode)
            return (false, null);
        return (true, role);
    }

    private int CreateVerificationCode()
    {
        var random = new Random();
        return random.Next(1, 999999);
    }
}