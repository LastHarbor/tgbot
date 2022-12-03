using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using tgbot;
using File = System.IO.File;

var botclient = new TelegramBotClient(API.BotToken);
using CancellationTokenSource cts = new();
var receiverOptions = new ReceiverOptions();
botclient.StartReceiving(HandleUpdatesMessagesAsync, HandleErrorAsync,
    receiverOptions,
    cancellationToken: cts.Token);

var me = await botclient.GetMeAsync();

Console.WriteLine($"Начал прослушку : {me.Username}");
Console.ReadLine();
cts.Cancel();


async Task HandleUpdatesMessagesAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    var path = @"C:\TelegramFiles\";
    var message = update.Message;
    var chatId = message!.Chat.Id;

    Console.WriteLine($"$Получено сообщение - {message.Text} от {chatId} - {message.Chat.Username} ");

    if (message.Text == "/upload")
    {
        await botclient.SendTextMessageAsync(chatId, "Начните отправлять файлы ");
    }

    switch (message.Type)
    {
        case MessageType.Document:
            await Methods.UploadDocuments(message, botclient, path);
            break;
        case MessageType.Video:
            await Methods.UploadVideo(message, botclient, path);
            break;
        case MessageType.Voice:
            await Methods.UploadVoice(message, botclient, path);
            break;
        case MessageType.Audio:
            await Methods.UploadAudio(botclient, message, path);
            break;
    }

    if (message.Text == "/dwnld")
    {
        var files = Directory.GetFiles(path)
            .Select(Path.GetFileName).ToArray();
        await botclient.SendTextMessageAsync(chatId, "Списков файлов:", 
            replyMarkup:  Methods.GetKeyboard(files!));
        //await botclient.SendTextMessageAsync(chatId, "Ответьте на это сообщение именем скопированного файла");
    }
    await using Stream str = File.OpenRead(path + message.Text);
    InputOnlineFile iof = new InputOnlineFile(str);                                                     // ALREADY WORKS
    iof.FileName = message.Text;
    await botclient.SendDocumentAsync(chatId, iof);
}

Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
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

