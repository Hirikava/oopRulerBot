using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Vostok.Logging.Abstractions;

namespace OopRulerBot.Telegram;

public class TelegramHandler : IUpdateHandler
{
    private readonly ILog log;
    private readonly ITelegramUsersStorage usersStorage;

    public TelegramHandler(
        ILog log,
        ITelegramUsersStorage usersStorage)
    {
        this.log = log;
        this.usersStorage = usersStorage;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        var handler = update switch
        {
            { Message: { } message } => BotOnMessageReceived(botClient, message, cancellationToken),
            _ => UnknownUpdateHandlerAsync(botClient, update, cancellationToken)
        };
        await handler;
    }

    public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
    {
        log.Error(exception);
    }

    private Task UnknownUpdateHandlerAsync(ITelegramBotClient telegramBotClient, Update update,
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task BotOnMessageReceived(ITelegramBotClient telegramBotClient, Message message,
        CancellationToken cancellationToken)
    {
        var messageText = message.Text;
        if (messageText is null)
            return;
        var action = messageText switch
        {
            _ => RegisterUserAndSendMessage(telegramBotClient, message, cancellationToken)
        };
        await action;
    }

    private async Task<Message> RegisterUserAndSendMessage(
        ITelegramBotClient telegramBotClient,
        Message message,
        CancellationToken cancellationToken)
    {
        await usersStorage.SaveUserInfo(new TelegramUserInfo
        {
            UserId = message.From.Id,
            UserName = message.From.Username
        });
        return await telegramBotClient.SendTextMessageAsync(new ChatId(message.From.Id),
            "You successfull registered for obtained verification codes");
    }
}