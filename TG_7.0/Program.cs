using System.Collections.Concurrent;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using File = System.IO.File;

namespace TG_7._0;

internal abstract class Program : OthersMethods
{
    private static readonly ITelegramBotClient BotClient = new TelegramBotClient("6348440231:AAFO28UNHkVkNAw6JQ5kKg8_kdeo-7MjCsE");

    private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.Message)
        {
            var message = update.Message;
            var aba = await UserCh.Task(message, cancellationToken, botClient);
            if (aba == true)
            {
                return;
            }
            var moscowTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time"));
            Console.WriteLine($"User: {message.Chat.Username}" + "\n" + $"Name: {message.Chat.FirstName}" + "\n" +
                              $"Surnameame: {message.Chat.LastName}" + "\n" + $"ID Chat: {message.Chat.Id}" + "\n" +
                              $"Time: {moscowTime}" + "\n" + $"Text: {message.Text}" + "\n");
            switch (message.Text)
            {
                case "/start":
                {
                    ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                    {
                        new KeyboardButton[] { "Расписание пар", "Инфа" },
                        new KeyboardButton[] { "Расписание звонков", "Капибара"}
                    })
                    {
                        ResizeKeyboard = true
                    };
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat,
                        text: "ОК",
                        replyMarkup: replyKeyboardMarkup,
                        cancellationToken: cancellationToken);
                    break;
                }
                case "Расписание звонков":
                    await botClient.SendPhotoAsync(message.Chat.Id, InputFile.FromUri("https://sun9-77.userapi.com/impg/as1MA-6kTJiBgNaTzlJchVz9WIdRuTZt9uNJpQ/2kp1pa0vxL4.jpg?size=994x467&quality=96&sign=87102e4153f1c047a2012aa21487f1cb&type=album"), cancellationToken: cancellationToken);
                    return;
                case "Расписание пар":
                {
                    ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                    {
                        new KeyboardButton[] { "Пары на сегодня", "Пары на завтра", "Назад" },
                    }) { ResizeKeyboard = true };
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "ОК",
                        replyMarkup: replyKeyboardMarkup,
                        cancellationToken: cancellationToken);
                    break;
                }
                case "Пары на завтра":
                    await Pari(botClient, cancellationToken, message, 1);
                    break;
                case "Пары на сегодня":
                    await Pari(botClient, cancellationToken, message, 0);
                    break;
                case "Капибара":
                {
                    var x = new Random();
                    var rand = x.Next(1, 45);
                    var link = File.ReadLines("../../../Fold_data/LinkCapybara.txt").ElementAtOrDefault(rand);
                    if (link != null)
                        await botClient.SendPhotoAsync(message.Chat.Id,
                            InputFile.FromUri(link), caption: $"{rand}/45", cancellationToken: cancellationToken);
                    break;
                }
                case "Назад":
                {
                    ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                    {
                        new KeyboardButton[] { "Расписание пар", "Инфа" },
                        new KeyboardButton[] { "Расписание звонков", "Капибара"}
                    })
                    {
                        ResizeKeyboard = true
                    };
                    await botClient.SendTextMessageAsync(chatId: message.Chat, text: "ОК",
                        replyMarkup: replyKeyboardMarkup, cancellationToken: cancellationToken);
                    break;
                }
                case "Поддержка":
                    await botClient.SendTextMessageAsync(message.Chat.Id,
                        "Поддержать разработчика: \n\n" + "СберБанк: `5469 4100 1429 4908`\n" +
                        "ВТБ: `2200 2460 4327 6560`\n\n", parseMode: ParseMode.MarkdownV2,
                        cancellationToken: cancellationToken);
                    break;
                case "Сообщить о баге":
                    await botClient.SendTextMessageAsync(message.Chat.Id,
                        "Сообщить об ошибке:\nTG: @n0rd_kr1eg\n" + "VK: https://vk.com/n0rd_kr1eg" +
                        "\n\nВремя ответа: 5-15 минут", cancellationToken: cancellationToken);
                    break;
                case "Инфа":
                {
                    ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                        {
                            new KeyboardButton[] { "Поддержка", "Сообщить о баге", "Назад" },
                        })
                        { ResizeKeyboard = true };
                    await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "ОК", replyMarkup: replyKeyboardMarkup, cancellationToken: cancellationToken);
                    break;
                }
                case "Пары на определённый день":
                    //await SendCalendar(message.Chat.Id, moscowTime, botClient);
                    break;
            }
        }
    }

    private static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
    {
        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        Console.WriteLine("Ужики я упал....");
        return Task.CompletedTask;
    }

    private static Task Main()
    {
        Console.WriteLine("Ужики я жив....");
        AppContext.SetSwitch("System.Drawing.EnableUnixSupport", true);
        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        var receiverOptions = new ReceiverOptions { };
        BotClient.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions,
            cancellationToken
        );
        Console.ReadLine();
        return Task.CompletedTask;
    }
}