using OopRulerBot.Telegram;
using OopRulerBot.Verification.Transport;
using Telegram.Bot;
using Telegram.Bot.Types;
using Vostok.Logging.Abstractions;

namespace OopRulerBot.Verification;

public class TelegramVerificationTransport : IVerificationTransport
{
    private readonly ILog log;
    private readonly ITelegramBotClient telegramBotClient;
    private readonly ITelegramUsersStorage telegramUsersStorage;

    public TelegramVerificationTransport(
        ITelegramBotClient telegramBotClient,
        ITelegramUsersStorage telegramUsersStorage,
        ILog log)
    {
        this.telegramBotClient = telegramBotClient;
        this.log = log;
        this.telegramUsersStorage = telegramUsersStorage;
    }

    public async Task<bool> SendVerificationCode(string identifier, int code)
    {
        var telegramUser = await telegramUsersStorage.GetUserInfo(identifier);
        if(telegramUser == null)
            return false;
        try
        {
            await telegramBotClient
                .SendTextMessageAsync(new ChatId(telegramUser.UserId), $"Your verification code is {code}");
            return true;
        }
        catch (Exception e)
        {
            log.Error(e);
            return false;
        }
    }
}