using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using tgbot;

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

    var path = @"..\TelegramFiles\";
    var message = update.Message;
    var chatId = message!.Chat.Id;

    if (!Directory.Exists(path)) Directory.CreateDirectory(path);

    Console.WriteLine($"$Получено сообщение - {message.Text} от {chatId} - {message.Chat.Username} ");

    if (message.Text == "/upload")
    {
        await botclient.SendTextMessageAsync(chatId, "Начните отправлять файлы ");
    }
    switch (message.Type)
    {
        case MessageType.Document:
            await Methods.MessageTypes.UploadDocuments(message, botclient, path);
            break;
        case MessageType.Video:
            await Methods.MessageTypes.UploadVideo(message, botclient, path);
            break;
        case MessageType.Voice:
            await Methods.MessageTypes.UploadVoice(message, botclient, path);
            break;
        case MessageType.Audio:
            await Methods.MessageTypes.UploadAudio(botclient, message, path);
            break;
        case MessageType.Photo:
            await botClient.SendTextMessageAsync(chatId, "Отправьте фотографию документом");
            break;
    }



    if (message.Text == "/dwnld")
    {
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        // var files = Directory.GetFiles(path)
        //     .Select(Path.GetFileName).ToArray();
       
        //var keyboard = new InlineKeyboardMarkup(Methods.GetKeyboard(files!));
        await botclient.SendTextMessageAsync(chatId, "Списков файлов:");
        await  Methods.GetFiles(botclient, message, path);
        await botclient.SendTextMessageAsync(chatId, "Ответьте на это сообщение именем скопированного файла");
    }

    if (message.ReplyToMessage != null&& message.ReplyToMessage.Text!.Contains("Ответьте на это сообщение именем скопированного файла"))
    {
       await Methods.SendFile(botclient, message, path);
    }

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


