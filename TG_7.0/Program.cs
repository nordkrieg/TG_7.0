using System.Net;
using System.Net.Sockets;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using File = System.IO.File;

namespace TG_7._0;

internal abstract class Program : OthersMethods
{
    private static class BotConfiguration
    {
        public const string NtpServer = "0.ru.pool.ntp.org";
    }

    private static string _month, _day;
    private static readonly ITelegramBotClient BotClient = new TelegramBotClient("6348440231:AAFO28UNHkVkNAw6JQ5kKg8_kdeo-7MjCsE");

    private static DateTime GetNetworkTime()
    {
        const string ntpServer = BotConfiguration.NtpServer;
        var ntpData = new byte[48];
        ntpData[0] = 0x1B;
        try
        {
            var addresses = Dns.GetHostEntry(ntpServer).AddressList;
            var ipEndPoint = new IPEndPoint(addresses[0], 123);
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.ReceiveTimeout = 4000;
            socket.Connect(ipEndPoint);
            socket.Send(ntpData);
            socket.Receive(ntpData);
            socket.Close();
            var intPart = ((ulong)ntpData[40] << 24) | ((ulong)ntpData[41] << 16) | ((ulong)ntpData[42] << 8) |
                          ntpData[43];
            var fractPart = ((ulong)ntpData[44] << 24) | ((ulong)ntpData[45] << 16) | ((ulong)ntpData[46] << 8) |
                            ntpData[47];
            var milliseconds = intPart * 1000 + fractPart * 1000 / 0x100000000L + 10798900;
            var networkDateTime = new DateTime(1900, 1, 1).AddMilliseconds((long)milliseconds);
            return networkDateTime;
        }
        catch (SocketException ex)
        {
            Console.WriteLine("Ошибка соксета при получении времени: " + ex.Message);
            return DateTime.Now;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Произошла ошибка при получении времени: " + ex.Message);
            return DateTime.Now;
        }
    }

    private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.Message)
        {
            var message = update.Message;
            Console.WriteLine($"User: {message.Chat.Username}" + "\n" + $"Name: {message.Chat.FirstName}" + "\n" +
                              $"Surnameame: {message.Chat.LastName}" + "\n" + $"ID Chat: {message.Chat.Id}" + "\n" +
                              $"Time: {GetNetworkTime()}" + "\n" + $"Text: {message.Text}" + "\n");
            if (message.Text == "/start")
            {
                ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                {
                    new KeyboardButton[] { "Расписание пар", "Инфа" },
                    new KeyboardButton[] { "Расписание звонков", "Капибара" }
                })
                {
                    ResizeKeyboard = true
                };
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat,
                    text: "ОК",
                    replyMarkup: replyKeyboardMarkup,
                    cancellationToken: cancellationToken);
            }
            if (message.Text == "Расписание звонков")
            {
                await botClient.SendMediaGroupAsync(message.Chat.Id, new IAlbumInputMedia[]
                {
                    new InputMediaPhoto(
                        InputFile.FromUri(
                            "https://sun9-77.userapi.com/impg/as1MA-6kTJiBgNaTzlJchVz9WIdRuTZt9uNJpQ/2kp1pa0vxL4.jpg?size=994x467&quality=96&sign=87102e4153f1c047a2012aa21487f1cb&type=album"))
                }, cancellationToken: cancellationToken);
                return;
            }
            if (message.Text == "Расписание пар")
            {
                ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                {
                    new KeyboardButton[] { "Пары на сегодня", "Пары на завтра","Назад"},
                }) { ResizeKeyboard = true };
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "ОК",
                    replyMarkup: replyKeyboardMarkup,
                    cancellationToken: cancellationToken);
            }
            if (message.Text == "Пары на завтра")
            {
                var exFile = false;
                _month = GetNetworkTime().AddDays(1).Month.ToString();
                _day = GetNetworkTime().AddDays(1).Day.ToString();
                if (Convert.ToInt32(_month) < 10) _month = "0" + _month;
                if (_day[0] == '0') _day = _day.TrimStart('0');
                if (File.Exists($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}-1.png") &&
                    File.Exists($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}-2.png"))
                {
                    exFile = true;
                    await using Stream stream =
                        File.OpenRead($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}-1.png");
                    await using Stream stream2 =
                        File.OpenRead($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}-2.png");
                    await botClient.SendMediaGroupAsync(message.Chat.Id,
                        new IAlbumInputMedia[]
                        {
                                new InputMediaPhoto(
                                    InputFile.FromStream(stream, $"{_day}.{_month}.{GetNetworkTime().Year}-1.png")),
                                new InputMediaPhoto(
                                    InputFile.FromStream(stream2, $"{_day}.{_month}.{GetNetworkTime().Year}-2.png"))
                        }, cancellationToken: cancellationToken);
                }

                if (File.Exists($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}-1.png") && exFile == false)
                {
                    exFile = true;
                    await using Stream stream =
                        File.OpenRead($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}-1.png");
                    await botClient.SendPhotoAsync(message.Chat.Id,
                        InputFile.FromStream(stream, $"{_day}.{_month}.{GetNetworkTime().Year}-1.png"),
                        cancellationToken: cancellationToken);
                }

                var urlCheckResult =
                    await CheckUrl(
                        $"https://mkeiit.ru/wp-content/uploads/{GetNetworkTime().Year}/{_month}/{_day}.{_month}.{GetNetworkTime().Year}.pdf");
                if (urlCheckResult && exFile == false)
                {
                    await DownLoad(
                        $"https://mkeiit.ru/wp-content/uploads/{GetNetworkTime().Year}/{_month}/{_day}.{_month}.{GetNetworkTime().Year}.pdf",
                        $"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}.pdf", GetNetworkTime());
                    ConvertFile($"{SchFold}", GetNetworkTime().AddDays(1));
                    Thread.Sleep(2000);
                    if (File.Exists($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}-1.png") &&
                        File.Exists($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}-2.png"))
                    {
                        exFile = true;
                        await using Stream stream =
                            File.OpenRead($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}-1.png");
                        await using Stream stream2 =
                            File.OpenRead($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}-2.png");
                        await botClient.SendMediaGroupAsync(message.Chat.Id,
                            new IAlbumInputMedia[]
                            {
                                    new InputMediaPhoto(
                                        InputFile.FromStream(stream, $"{_day}.{_month}.{GetNetworkTime().Year}-1.png")),
                                    new InputMediaPhoto(
                                        InputFile.FromStream(stream2, $"{_day}.{_month}.{GetNetworkTime().Year}-2.png"))
                            }, cancellationToken: cancellationToken);
                    }

                    if (File.Exists($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}-1.png") && exFile == false)
                    {
                        await using Stream stream =
                            File.OpenRead($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}-1.png");
                        await botClient.SendPhotoAsync(message.Chat.Id,
                            InputFile.FromStream(stream, $"{_day}.{_month}.{GetNetworkTime().Year}-1.png"),
                            cancellationToken: cancellationToken);
                    }
                }
                else
                {
                    if (!exFile)
                        await botClient.SendTextMessageAsync(message.Chat.Id,
                            $"Расписания на {GetNetworkTime().AddDays(1).ToShortDateString()} нет",
                            cancellationToken: cancellationToken);
                }
            }
            if (message.Text == "Пары на сегодня")
            {
                var exFile = false;
                _month = GetNetworkTime().Month.ToString();
                _day = GetNetworkTime().Day.ToString();
                if (Convert.ToInt32(_month) < 10) _month = "0" + _month;
                if (_day[0] == '0') _day = _day.TrimStart('0');
                if (File.Exists($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}-1.png") &&
                    File.Exists($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}-2.png"))
                {
                    exFile = true;
                    await using Stream stream =
                        File.OpenRead($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}-1.png");
                    await using Stream stream2 =
                        File.OpenRead($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}-2.png");
                    await botClient.SendMediaGroupAsync(message.Chat.Id,
                        new IAlbumInputMedia[]
                        {
                             new InputMediaPhoto(
                                 InputFile.FromStream(stream, $"{_day}.{_month}.{GetNetworkTime().Year}-1.png")),
                             new InputMediaPhoto(
                                 InputFile.FromStream(stream2, $"{_day}.{_month}.{GetNetworkTime().Year}-2.png"))
                        }, cancellationToken: cancellationToken);
                }
                if (File.Exists($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}-1.png") && exFile == false)
                {
                    exFile = true;
                    await using Stream stream =
                        File.OpenRead($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}-1.png");
                    await botClient.SendPhotoAsync(message.Chat.Id,
                        InputFile.FromStream(stream, $"{_day}.{_month}.{GetNetworkTime().Year}-1.png"),
                        cancellationToken: cancellationToken);
                }
                var urlCheckResult =
                    await CheckUrl(
                        $"https://mkeiit.ru/wp-content/uploads/{GetNetworkTime().Year}/{_month}/{_day}.{_month}.{GetNetworkTime().Year}.pdf");
                if (urlCheckResult && exFile == false)
                {
                    await DownLoad(
                        $"https://mkeiit.ru/wp-content/uploads/{GetNetworkTime().Year}/{_month}/{_day}.{_month}.{GetNetworkTime().Year}.pdf",
                        $"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}.pdf", GetNetworkTime());
                    ConvertFile($"{SchFold}", GetNetworkTime());
                    Thread.Sleep(2000);
                    if (File.Exists($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}-1.png") &&
                        File.Exists($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}-2.png"))
                    {
                        exFile = true;
                        await using Stream stream =
                            File.OpenRead($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}-1.png");
                        await using Stream stream2 =
                            File.OpenRead($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}-2.png");
                        await botClient.SendMediaGroupAsync(message.Chat.Id,
                            new IAlbumInputMedia[]
                            {
                                 new InputMediaPhoto(
                                     InputFile.FromStream(stream, $"{_day}.{_month}.{GetNetworkTime().Year}-1.png")),
                                 new InputMediaPhoto(
                                     InputFile.FromStream(stream2, $"{_day}.{_month}.{GetNetworkTime().Year}-2.png"))
                            }, cancellationToken: cancellationToken);
                    }
                    if (File.Exists($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}-1.png") && exFile == false)
                    {
                        await using Stream stream =
                            File.OpenRead($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}-1.png");
                        await botClient.SendPhotoAsync(message.Chat.Id,
                            InputFile.FromStream(stream, $"{_day}.{_month}.{GetNetworkTime().Year}-1.png"),
                            cancellationToken: cancellationToken);
                    }
                }
                else
                {
                    if (!exFile)
                        await botClient.SendTextMessageAsync(message.Chat.Id,
                            $"Расписания на {GetNetworkTime().ToShortDateString()} нет", cancellationToken: cancellationToken);
                }
            }
            if (message.Text == "Капибара")
            {
                var x = new Random();
                var rand = x.Next(1, 45);
                var link = File.ReadLines("../../../Fold_data/LinkCapybara.txt").ElementAtOrDefault(rand);
                if (link != null)
                    await botClient.SendPhotoAsync(message.Chat.Id,
                        InputFile.FromUri(link), caption: $"{rand}/45", cancellationToken: cancellationToken);
            }
            if (message.Text == "Назад")
            {
                ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                {
                    new KeyboardButton[] { "Расписание пар", "Инфа" },
                    new KeyboardButton[] { "Расписание звонков", "Капибара" }
                })
                {
                    ResizeKeyboard = true
                };
                await botClient.SendTextMessageAsync(chatId: message.Chat, text: "ОК", replyMarkup: replyKeyboardMarkup, cancellationToken: cancellationToken);
            }
            if (message.Text == "Поддержка")
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Поддержать разработчика: \n\n" +
                                                                      "СберБанк: `5469 4100 1429 4908`\n" +
                                                                      "ВТБ: `2200 2460 4327 6560`\n\n",
                    parseMode: ParseMode.MarkdownV2, cancellationToken: cancellationToken);
            }
            if (message.Text == "Сообщить о баге")
            {
                await botClient.SendTextMessageAsync(message.Chat.Id,
                    "Сообщить об ошибке:\nTG: @n0rd_kr1eg\n" + "VK: https://vk.com/n0rd_kr1eg" +
                    "\n\nВремя ответа: 5-15 минут", cancellationToken: cancellationToken);
            }
            if (message.Text == "Инфа")
            {
                ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                    {
                        new KeyboardButton[] { "Поддержка", "Сообщить о баге", "Назад"},
                    })
                    { ResizeKeyboard = true };
                await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "ОК", replyMarkup: replyKeyboardMarkup, cancellationToken: cancellationToken);
            }
        }
    }
    private static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        Console.WriteLine("Ужики я упал....");
        return Task.CompletedTask;
    }
    private static Task Main()
    {
        Console.WriteLine("Ужики я жив....");
        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        var receiverOptions = new ReceiverOptions{};
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