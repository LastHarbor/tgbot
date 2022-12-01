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
    }

    if (message.Text == "/dwnld")
    {
        await Methods.GetFilesOnDirectory(botclient, message, path);
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
