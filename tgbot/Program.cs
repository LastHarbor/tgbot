using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using tgbot;

var botclient = new TelegramBotClient(API.BotToken);
using var cts = new CancellationTokenSource();
var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = { }
};
var path = @"C:\TelegramFiles";
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
    
    Console.WriteLine($"Получено сообщение: ({message.Text}) от {chatId} - {message.Chat.Username}");
    
    if (message?.Text !=null)
    {
    }

    if (message?.Text == "Файлы")
    {
        GetFilesOnDirectory(botclient, message);
    }
    if (message.Type == MessageType.Document)
    {
        Console.WriteLine($"Получен документ {message.Document} от {message.Chat.Id} - {message.Chat.Username}");
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
    var file = await botclient.GetFileAsync(fileId);
    var filePath = file.FilePath;
    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
    string destanationPath = $@"{path}\{message.Document.FileName}";

    await using (FileStream fstream = new FileStream(destanationPath, FileMode.OpenOrCreate))
    {
        file = await botclient.GetInfoAndDownloadFileAsync(fileId, fstream);
    }
}

async Task GetFilesOnDirectory(TelegramBotClient botClient, Message ms)
{
    List<string> FileList = new List<string>();
    var Files = Directory.GetFiles(path);
    var fileNames = String.Format($"{Files}");
    await botclient.SendTextMessageAsync(ms.Chat.Id, $"{fileNames}");
}







