using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace tgbot;

public class Methods
{
    /// <summary>
    /// Upload document method
    /// </summary>
    /// <param name="message"></param>
    /// <param name="botclient"></param>
    /// <param name="path"></param>
    public static async Task UploadDocuments(Message message, ITelegramBotClient botclient, string path)
    {
        Console.WriteLine($"Получен документ от {message.Chat.Id} - {message.Chat.Username}");
        var fileId = message.Document?.FileId;
        await botclient.GetFileAsync(fileId ?? string.Empty);
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        string destanationPath = $@"{path}\{message.Document?.FileName}";

        await using (FileStream fstream = new FileStream(destanationPath, FileMode.OpenOrCreate))
        {
            await botclient.GetInfoAndDownloadFileAsync(fileId ?? string.Empty, fstream);
        }

        Console.WriteLine($"Документ успешно сохранен ");
        await botclient.SendTextMessageAsync(message.Chat.Id, "Файл успешно схранен");
    }

    /// <summary>
    /// Upload video method
    /// </summary>
    /// <param name="message"></param>
    /// <param name="botclient"></param>
    /// <param name="path"></param>
    public static async Task UploadVideo(Message message, ITelegramBotClient botclient, string path)
    {
        Console.WriteLine($"Получено видео от {message.Chat.Id} - {message.Chat.Username}");
        var fileId = message.Video?.FileId;
        await botclient.GetFileAsync(fileId!);
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        string destanationPath = $@"{path}\{message.Video?.FileName}";

        await using (FileStream fstream = new FileStream(destanationPath, FileMode.OpenOrCreate))
        {
            await botclient.GetInfoAndDownloadFileAsync(fileId!, fstream);
        }

        Console.WriteLine($"Видео успешно сохранено ");
        await botclient.SendTextMessageAsync(message.Chat.Id, "Файл успешно схранен");
    }

    /// <summary>
    /// Upload voice method
    /// </summary>
    /// <param name="message"></param>
    /// <param name="botclient"></param>
    /// <param name="path"></param>
    public static async Task UploadVoice(Message message, ITelegramBotClient botclient, string path)
    {
        Console.WriteLine($"Получен войс от {message.Chat.Id} - {message.Chat.Username}");
        var fileId = message.Voice?.FileId;
        await botclient!.GetFileAsync(fileId!);
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        string destanationPath = $@"{path}\{message.Voice}";

        await using (FileStream fstream = new FileStream(destanationPath, FileMode.OpenOrCreate))
        {
            await botclient.GetInfoAndDownloadFileAsync(fileId!, fstream);
        }

        Console.WriteLine($"Войс успешно сохранен ");
        await botclient.SendTextMessageAsync(message.Chat.Id, "Файл успешно схранен");
    }

    /// <summary>
    /// Upload audio method
    /// </summary>
    /// <param name="botclient"></param>
    /// <param name="message"></param>
    /// <param name="path"></param>
    public static async Task UploadAudio(ITelegramBotClient botclient, Message message, string path)
    {
        Console.WriteLine($"Получено аудио от {message.Chat.Id} - {message.Chat.Username}");
        var fileId = message.Audio?.FileId;
        await botclient!.GetFileAsync(fileId!);
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        string destanationPath = $@"{path}\{message.Audio?.FileName}";

        await using (FileStream fstream = new FileStream(destanationPath, FileMode.OpenOrCreate))
        {
            await botclient.GetInfoAndDownloadFileAsync(fileId!, fstream);
        }

        Console.WriteLine($"Аудио успешно сохранен ");
        await botclient.SendTextMessageAsync(message.Chat.Id, "Файл успешно схранен");
    }

    /// <summary>
    /// Method show existing  files in storage directory
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="ms"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public static Task GetFiles(TelegramBotClient botClient, Message ms, string path)
    {
        Directory
            .GetFiles(path, "*", SearchOption.AllDirectories)
            .ToList()
            .ForEach(f => botClient.SendTextMessageAsync(ms.Chat.Id, Path.GetFileName(f)));
        return Task.CompletedTask;
    }

    // public async static Task GetFile(string path, string file, TelegramBotClient botClient, Message message)
    // {
    //     await using Stream stream = System.IO.File.OpenRead(path);
    //     await botClient.SendDocumentAsync(message.Chat.Id, new InputOnlineFile(stream, file));
    //
    // }

    public static async Task SendFile()
    {
        
    }

    // public static InlineKeyboardButton[][] GetKeyboard(List<string> array)
    // {
    //     var keyboardInline = new InlineKeyboardButton[1][];
    //     var keyboardButtons = new InlineKeyboardButton[array.Count];
    //     for (var i = 0; i < array.Count; i++)
    //     {
    //         keyboardButtons[i] = new InlineKeyboardButton(string.Empty)
    //         {
    //             Text = array[i],
    //             CallbackData = "some data"
    //         };
    //     }
    //
    //     keyboardInline[0] = keyboardButtons;
    //     return keyboardInline;
    // }
     public static InlineKeyboardMarkup GetKeyboard(string[] array,int buttonsPerRow = 0)
     {
         var s = array.ToString();
         var data = array.Select(x => InlineKeyboardButton.WithCallbackData(s));
         if (buttonsPerRow == 0)
             return new InlineKeyboardButton[][] { data.ToArray() };
    
         else return data.Chunk(buttonsPerRow).Select(c => c.ToArray()).ToArray();
      }
 }