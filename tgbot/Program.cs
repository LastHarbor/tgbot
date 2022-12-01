using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using tgbot;

var botclient = new TelegramBotClient(API.BotToken);
using var cts = new CancellationTokenSource();
var receiverOptions = new ReceiverOptions();
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
    var chatId = message!.Chat.Id;
    
    Console.WriteLine($"$Получено сообщение - {message.Text} от {chatId} - {message.Chat.Username} ");

    if (message.Text == "/upload")
    {
        await botclient.SendTextMessageAsync(chatId, "Начните отправлять файлы");
    }
    if (message.ReplyToMessage != null && message.ReplyToMessage.Text!.Contains("Начните отправлять файлы") )
    {
        switch (message.Type)
        {
            case MessageType.Document:
                Console.WriteLine($"Получен документ от {message.Chat.Id} - {message.Chat.Username}");
                await UploadDocuments(message);
                Console.WriteLine($"Документ успешно сохранен ");
                await botclient.SendTextMessageAsync(chatId, "Файл успешно схранен");
                break;
            case MessageType.Video:
                Console.WriteLine($"Получено видео от {message.Chat.Id} - {message.Chat.Username}");
                await UploadVideo(message);
                Console.WriteLine($"Видео успешно сохранено ");
                await botclient.SendTextMessageAsync(chatId, "Файл успешно схранен");
                break;
            case MessageType.Voice:
                Console.WriteLine($"Получен войс от {message.Chat.Id} - {message.Chat.Username}");
                await UploadVoice(message);
                Console.WriteLine($"Войс успешно сохранен ");
                await botclient.SendTextMessageAsync(chatId, "Файл успешно схранен");
                break;
            case MessageType.Audio:
                Console.WriteLine($"Получено аудио от {message.Chat.Id} - {message.Chat.Username}");
                await UploadAudio(message);
                Console.WriteLine($"Аудио успешно сохранен ");
                await botclient.SendTextMessageAsync(chatId, "Файл успешно схранен");
                break;
        }
    }

    if (message.Text == "/dwnld")
    {
        await GetFilesOnDirectory(botclient, message);
    }
}

Task HandleErrorAsync (ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    Newtonsoft.Json.JsonConvert.SerializeObject(exception);
    var errorMessage = exception switch
    {
        ApiRequestException apiRequestException =>
            $"Ошибка API телеграм: \n {apiRequestException.ErrorCode}\n{apiRequestException.Message}",
        _ => exception.ToString()
    };
    Console.WriteLine(errorMessage);
    return Task.CompletedTask;
}

async Task UploadDocuments(Message message)
{
    var fileId = message.Document?.FileId;
    await botclient.GetFileAsync(fileId ?? string.Empty);
    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
    string destanationPath = $@"{path}\{message.Document?.FileName}";

    await using (FileStream fstream = new FileStream(destanationPath, FileMode.OpenOrCreate))
    {
        await botclient.GetInfoAndDownloadFileAsync(fileId ?? string.Empty, fstream);
    }
}
async Task UploadVideo(Message message)
{
    var fileId = message.Video?.FileId;
    await botclient!.GetFileAsync(fileId!);
    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
    string destanationPath = $@"{path}\{message.Video?.FileName}";

    await using (FileStream fstream = new FileStream(destanationPath, FileMode.OpenOrCreate))
    {
        await botclient.GetInfoAndDownloadFileAsync(fileId, fstream);
    }
}
async Task UploadVoice(Message message)
{
    var fileId = message.Voice?.FileId;
    await botclient!.GetFileAsync(fileId!);
    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
    string destanationPath = $@"{path}\{message.Voice}";

    await using (FileStream fstream = new FileStream(destanationPath, FileMode.OpenOrCreate))
    {
        await botclient.GetInfoAndDownloadFileAsync(fileId, fstream);
    }
}
async Task UploadAudio(Message message)
{
    var fileId = message.Audio?.FileId;
    await botclient!.GetFileAsync(fileId!);
    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
    string destanationPath = $@"{path}\{message.Audio?.FileName}";

    await using (FileStream fstream = new FileStream(destanationPath, FileMode.OpenOrCreate))
    {
        await botclient.GetInfoAndDownloadFileAsync(fileId, fstream);
    }
}

async Task GetFilesOnDirectory(TelegramBotClient botClient, Message ms)
{
    Directory
        .GetFiles(path, "*", SearchOption.AllDirectories)
        .ToList()
        .ForEach(async f => await botclient.SendTextMessageAsync(ms.Chat.Id, Path.GetFileName(f)));
}







