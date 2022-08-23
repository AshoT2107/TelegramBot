using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

class TelegramBotService
{
    string Token = "5226260727:AAFUy12HvOpth0oMzIn74UawVV_tSDKFZ8w";
    public TelegramBotClient Bot;
    public TelegramBotService()
    {
        Bot = new TelegramBotClient(Token);
    }

    public void GetUpdate(Func<ITelegramBotClient, Update, CancellationToken, Task> update)
    {
        Bot.StartReceiving(
            updateHandler: update,
            errorHandler: (_, ex, _) =>
                {
                    Console.WriteLine(ex.Message);
                    return Task.CompletedTask;
                });
    }
    public void SendMessage(long chatId, string message, IReplyMarkup? reply = null)
    {
        Bot.SendTextMessageAsync(chatId, message, replyMarkup: reply);
    }
    public void EditMessage(long chatId, int messageId, string text)
    {
        Bot.EditMessageTextAsync(chatId: chatId, messageId, text);
    }

    public void EditMessageReplyMarkup(long chatId, int messageId, InlineKeyboardMarkup? reply)
    {
        Bot.EditMessageReplyMarkupAsync(chatId: chatId, messageId: messageId, replyMarkup: reply);
    }

}



