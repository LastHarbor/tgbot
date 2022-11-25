using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using tgbot;
var botclient = new TelegramBotClient(API.BotToken);
using var cts = new CancellationTokenSource();
var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = {}
};
botclient.StartReceiving(HandleUpdatesMessagesAsync, HandleErrorAsync,
    receiverOptions,
    cancellationToken: cts.Token);

User me = await botclient.GetMeAsync();
Console.WriteLine($"Начал прослушку : {me.Username}");
Console.ReadLine();
cts.Cancel();

async Task HandleUpdatesMessagesAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    var message = update.Message;
    var chatId = message.Chat.Id;
    
    if (message?.Text !=null)
    {
        await botclient.SendTextMessageAsync(chatId, $"Вас приветствует EkrimBot, что вы хотите сделать? ");
    }
    if (message.Type == MessageType.Document)
    {
        Console.WriteLine($"Получен документ {message.Document}");
        await DownloadDocuments(botclient, message);
        Console.WriteLine($"Документ успешно сохранен ");
    }
}


Task HandleErrorAsync (ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var errorMessage = exception switch
    {
        ApiRequestException apiRequestException =>
            $"Ошибка API телеграм: \n {apiRequestException.ErrorCode}\n{apiRequestException.Message}",
        _ => exception.ToString()
    };
    Console.WriteLine(errorMessage);
    return Task.CompletedTask;
}

async Task DownloadDocuments(TelegramBotClient botClient, Message message)
{
    var fileId = message.Document?.FileId;
    string destanationPath = $@"C:\TelegrammFiles\{message.Document.FileName}";
    
    if (!Directory.Exists(destanationPath)) Directory.CreateDirectory(destanationPath);
    await using FileStream fstream = new FileStream(destanationPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
    await botclient.GetInfoAndDownloadFileAsync(fileId: fileId, destination: fstream);
     fstream.Close();
}







