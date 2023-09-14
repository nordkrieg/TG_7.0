using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
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
    private static async Task Main()
    {
        var botClient = new TelegramBotClient("6348440231:AAFO28UNHkVkNAw6JQ5kKg8_kdeo-7MjCsE");
        botClient.StartReceiving(Update, Error);
        Console.WriteLine(string.Concat(Enumerable.Repeat("-", 120)) + string.Concat(Enumerable.Repeat("\t", 5)) +
                          "\n" + "WELCOME TO LessonAssistant Bot!" + "\n" + string.Concat(Enumerable.Repeat("-", 120)) + string.Concat(Enumerable.Repeat("\t", 2)) + "\n" + 
                          "All information is in the telegram channel: https://t.me/lesson_assistant_backstage!" + "\n" + string.Concat(Enumerable.Repeat("-", 120)) + "\n" +"Setting:" + 
                          "\n" + "Bot Version v0.9" + "\n" + $"Getting started: {GetNetworkTime()}" +  "\n" + $"Name PC: {Dns.GetHostName()}" + "\n" + string.Concat(Enumerable.Repeat("-", 120)) + 
                          "\n" + "Bot Started..." + "\n" + string.Concat(Enumerable.Repeat("-", 120)));
            var hostBuilder = new HostBuilder()
            .ConfigureServices((hostContext, services) => services.AddHostedService<Service>());
        await hostBuilder.RunConsoleAsync().ConfigureAwait(false);
    }
    private static async Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
    {
        var message = update.Message;
        if (message is { Text: not null })
        {
            Console.WriteLine($"User: {message.Chat.Username}" + "\n" + $"Name: {message.Chat.FirstName}" + "\n" +
                              $"Surnameame: {message.Chat.LastName}" + "\n" + $"ID Chat: {message.Chat.Id}" + "\n" +
                              $"Time: {GetNetworkTime()}" + "\n" + $"Text: {message.Text}" + "\n");
            switch (message.Text.ToLower())
            {
                case "/start":
                    await botClient.SendTextMessageAsync(message.Chat.Id,
                        "Добро пожаловать!\n\nДля пользования ботом советую прочитать сводку команд, использовав /info в меню",
                        cancellationToken: token);
                    break;
                case "/call_schedule":
                    await botClient.SendMediaGroupAsync(message.Chat.Id, new IAlbumInputMedia[]
                    {
                        new InputMediaPhoto(
                            InputFile.FromUri(
                                "https://sun9-77.userapi.com/impg/as1MA-6kTJiBgNaTzlJchVz9WIdRuTZt9uNJpQ/2kp1pa0vxL4.jpg?size=994x467&quality=96&sign=87102e4153f1c047a2012aa21487f1cb&type=album"))
                    }, cancellationToken: token);
                    break;
                case "/schedule_tomorrow":
                {
                    var exFile = false;
                    _month = GetNetworkTime().AddDays(1).Month.ToString();
                    _day = GetNetworkTime().AddDays(1).Day.ToString();
                    if (Convert.ToInt32(_month) < 10) _month ="0" + _month;
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
                                }, cancellationToken: token);
                        }
                        if (File.Exists($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}-1.png") && exFile == false)
                        {
                            exFile = true;
                            await using Stream stream =
                                File.OpenRead($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}-1.png");
                            await botClient.SendPhotoAsync(message.Chat.Id,
                                InputFile.FromStream(stream, $"{_day}.{_month}.{GetNetworkTime().Year}-1.png"),
                                cancellationToken: token);
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
                                }, cancellationToken: token);
                        }

                        if (File.Exists($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}-1.png") && exFile == false)
                        {
                            await using Stream stream =
                                File.OpenRead($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}-1.png");
                            await botClient.SendPhotoAsync(message.Chat.Id,
                                InputFile.FromStream(stream, $"{_day}.{_month}.{GetNetworkTime().Year}-1.png"),
                                cancellationToken: token);
                        }
                    }
                    else
                    {
                        if (!exFile)
                            await botClient.SendTextMessageAsync(message.Chat.Id,
                                $"Расписания на {GetNetworkTime().AddDays(1).ToShortDateString()} нет",
                                cancellationToken: token);
                    }
                    break;
                }
                case "/schedule_today":
                {
                    var exFile = false;
                    _month = GetNetworkTime().Month.ToString();
                    _day = GetNetworkTime().Day.ToString();
                    if (Convert.ToInt32(_month) < 10)  _month = "0" + _month;
                    if (_day[0] == '0')  _day = _day.TrimStart('0');
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
                            }, cancellationToken: token);
                    }
                    if (File.Exists($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}-1.png") && exFile == false)
                    {
                        exFile = true;
                        await using Stream stream =
                            File.OpenRead($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}-1.png");
                        await botClient.SendPhotoAsync(message.Chat.Id,
                            InputFile.FromStream(stream, $"{_day}.{_month}.{GetNetworkTime().Year}-1.png"),
                            cancellationToken: token);
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
                                }, cancellationToken: token);
                        }
                        if (File.Exists($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}-1.png") && exFile == false)
                        {
                            await using Stream stream =
                                File.OpenRead($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}-1.png");
                            await botClient.SendPhotoAsync(message.Chat.Id,
                                InputFile.FromStream(stream, $"{_day}.{_month}.{GetNetworkTime().Year}-1.png"),
                                cancellationToken: token);
                        }
                    }
                    else
                    {
                        if (!exFile)
                            await botClient.SendTextMessageAsync(message.Chat.Id,
                                $"Расписания на {GetNetworkTime().ToShortDateString()} нет", cancellationToken: token);
                    }
                    break;
                }
                case "/capybara":
                {
                    var x = new Random();
                    var rand = x.Next(1, 45);
                    var link = File.ReadLines("../../../Fold_data/LinkCapybara.txt").ElementAtOrDefault(rand);
                    if (link != null)
                        await botClient.SendPhotoAsync(message.Chat.Id,
                            InputFile.FromUri(link), caption: $"{rand}/45", cancellationToken: token);
                    break;
                }
                case "/support":
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Поддержать разработчика: \n\n" +
                                                                          "СберБанк: `5469 4100 1429 4908`\n" +
                                                                          "ВТБ: `2200 2460 4327 6560`\n\n",
                        parseMode: ParseMode.MarkdownV2, cancellationToken: token);
                    break;
                case "/future_updates":
                    await botClient.SendTextMessageAsync(message.Chat.Id,
                        await File.ReadAllTextAsync("../../../Fold_data/fut_update.txt", token),
                        cancellationToken: token);
                    break;
                case "/bugs":
                    await botClient.SendTextMessageAsync(message.Chat.Id,
                        "Сообщить об ошибке:\nTG: @n0rd_kr1eg\n" + "VK: https://vk.com/n0rd_kr1eg" +
                        "\n\nВремя ответа: 5-15 минут", cancellationToken: token);
                    break;
                case "/schedule_session":
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Информации по сессии нет :(",
                        cancellationToken: token);
                    break;
                case "/info":
                    await botClient.SendTextMessageAsync(message.Chat.Id,
                        await File.ReadAllTextAsync("../../../Fold_data/information.txt", token),
                        cancellationToken: token);
                    break;
                default:
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Неверная команда!",
                        cancellationToken: token);
                    break;
            }
        }
    }
    private static Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
    {
        throw new NotImplementedException("треш");
    }
}

public class Service : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Task is started.");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Task stopped.");
        return Task.CompletedTask;
    }
}