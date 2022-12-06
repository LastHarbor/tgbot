using System.IO;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using File = System.IO.File;

namespace tgbot;

public class Methods
{
    static TelegramBotClient botclient = new TelegramBotClient(API.BotToken);
    static readonly Update update = new Update();
    static Message message = new Message();
    static Message recive = update.Message;
    private static string path;

    /// <summary>
    /// Method show existing files in storage directory.
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


    public static async Task SendFile(TelegramBotClient botclient, Message message, string path)
    {
        await using Stream str = File.OpenRead(path + message.Text);
        InputOnlineFile iof = new InputOnlineFile(str);                                                     // ALREADY WORKS
        iof.FileName = message.Text;
        await botclient.SendDocumentAsync(message.Chat.Id, iof);
    }


    public static InlineKeyboardButton[][] GetKeyboard(string[] array)
    {

        var keyboardInline = new InlineKeyboardButton[array.Length][];

        for (var i = 0; i < 5; i++)
        {
            var keyboardButtons = new InlineKeyboardButton[1];
            keyboardButtons[0] = new InlineKeyboardButton(string.Empty)
            {
                Text = array[i],
                CallbackData = SendFile(botclient, recive, array[i]),
            };

            keyboardInline[i] = keyboardButtons;
        }

        return keyboardInline;
    }
    //public static InlineKeyboardMarkup GetKeyboard(string[] array,int buttonsPerRow = 0)
    // {
    //     var s = array.ToString();
    //     var data = array.Select(x => InlineKeyboardButton.WithCallbackData(s));
    //     if (buttonsPerRow == 0)
    //         return new InlineKeyboardButton[][] { data.ToArray() };

    //     else return data.Chunk(buttonsPerRow).Select(c => c.ToArray()).ToArray();
    //  }

    public class MessageTypes
    {
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

        public static async Task UploadAudio(ITelegramBotClient botclient, Message message, string path)
        {
            Console.WriteLine($"Получено {message.Type} от {message.Chat.Id} - {message.Chat.Username}");
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
    }
 }