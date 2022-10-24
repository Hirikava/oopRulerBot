using Telegram.Bot;
using Telegram.Bot.Types;
using Vostok.Logging.Abstractions;

namespace OopRulerBot.Verification;

public class TelegramVerificationTransmition : IVerificationTransmition
{
    private readonly ILog log;
    private readonly TelegramBotClient telegramBotClient;

    public TelegramVerificationTransmition(
        TelegramBotClient telegramBotClient, 
        ILog log)
    {
        this.telegramBotClient = telegramBotClient;
        this.log = log;
    }
    public async Task SendVerificationCode(string identifier, int code)
    { }
}