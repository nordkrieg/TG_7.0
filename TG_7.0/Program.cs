using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net;
using System.Net.Sockets;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TG_7._0
{
    internal abstract class Program : OthersMethods
    {
        private static string _month, _day;
        private static DateTime GetNetworkTime()
        {
            const string ntpServer = "0.ru.pool.ntp.org";
            var ntpData = new byte[48];
            ntpData[0] = 0x1B;
            var addresses = Dns.GetHostEntry(ntpServer).AddressList;
            var ipEndPoint = new IPEndPoint(addresses[0], 123);
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.ReceiveTimeout = 3000;
            socket.Connect(ipEndPoint);
            socket.Send(ntpData);
            socket.Receive(ntpData);
            socket.Close();
            var intPart = (ulong)ntpData[40] << 24 | (ulong)ntpData[41] << 16 | (ulong)ntpData[42] << 8 | (ulong)ntpData[43];
            var fractPart = (ulong)ntpData[44] << 24 | (ulong)ntpData[45] << 16 | (ulong)ntpData[46] << 8 | (ulong)ntpData[47];
            var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L) + 10798900;
            var networkDateTime = (new DateTime(1900, 1, 1)).AddMilliseconds((long)milliseconds);
            return networkDateTime;
        }
        private static async Task Main()
        {
            var botClient = new TelegramBotClient("6348440231:AAFO28UNHkVkNAw6JQ5kKg8_kdeo-7MjCsE");
            botClient.StartReceiving(Update, Error);
            Console.WriteLine(string.Concat(Enumerable.Repeat("-", 120)));
            Console.Write(string.Concat(Enumerable.Repeat("\t", 5)));
            Console.WriteLine("WELCOME TO LessonAssistant Bot!");
            Console.WriteLine(string.Concat(Enumerable.Repeat("-", 120)));
            Console.Write(string.Concat(Enumerable.Repeat("\t", 2)));
            Console.WriteLine("All information is in the telegram channel: https://t.me/lesson_assistant_backstage !");
            Console.WriteLine(string.Concat(Enumerable.Repeat("-", 120)));
            Console.WriteLine("Setting:");
            Console.WriteLine("Bot Version v0.9");
            Console.WriteLine($"Getting started: {GetNetworkTime()}");
            Console.WriteLine($"Name PC: {Dns.GetHostName()}");
            Console.WriteLine();
            Console.WriteLine(string.Concat(Enumerable.Repeat("-", 120)));
            Console.WriteLine();
            Console.WriteLine("Bot Started...");
            Console.WriteLine();
            Console.WriteLine(string.Concat(Enumerable.Repeat("-", 120)));
            var hostBuilder = new HostBuilder()
            .ConfigureServices((hostContext, services) => services.AddHostedService<Service>());
            await hostBuilder.RunConsoleAsync().ConfigureAwait(false);
        }
        private static async Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            var message = update.Message;
            if (message is { Text: not null })
            {
                Console.WriteLine($"User: {message.Chat.Username}");
                Console.WriteLine($"Name: {message.Chat.FirstName}");
                Console.WriteLine($"Surnameame: {message.Chat.LastName}");
                Console.WriteLine($"ID Chat: {message.Chat.Id}");
                Console.WriteLine($"Time: {GetNetworkTime()}");
                Console.WriteLine($"Text: {message.Text}");
                Console.WriteLine();
                if (message.Text.ToLower() == "/start")
                    await botClient.SendTextMessageAsync(message.Chat.Id,
                        "Добро пожаловать!\n\nДля пользования ботом советую прочитать сводку команд, использовав /info в меню",
                        cancellationToken: token);
                switch (message.Text.ToLower())
                {
                    case "привет":
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Добро пожаловать!", cancellationToken: token);
                        break;
                    case "дата":
                        await botClient.SendTextMessageAsync(message.Chat.Id, GetNetworkTime().DayOfWeek.ToString(), cancellationToken: token);
                        break;
                    case "/call_schedule":
                        await botClient.SendMediaGroupAsync(message.Chat.Id, media:
                            new IAlbumInputMedia[]
                            {
                                new InputMediaPhoto(
                                    InputFile.FromUri("https://sun9-28.userapi.com/impg/IEuODnxfR1Vk11rBQGHL-kXbhEWDmgNTJuw2TA/xBOIVnIXLvE.jpg?size=998x475&quality=96&sign=bf2bd85a0dd3cbd250da0760b4966079&type=album"))
                            }, cancellationToken: token);
                        break;
                    /* case "/schedule_today":
                         {
                             var exFile = false;
                             _month = GetNetworkTime().Month.ToString();
                             Console.WriteLine("я думаю что сегодня   " + GetNetworkTime().ToShortDateString());
                             _day = GetNetworkTime().Day.ToString();
                             if (Convert.ToInt32(_month) < 10)
                                 _month = "0" + _month;
                             if (_day[0] == '0')
                                 _day = _day.TrimStart('0');
                             Console.WriteLine(_day);
                             Console.WriteLine(_month);
                             Console.WriteLine(GetNetworkTime().Year);
                             Console.WriteLine($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}.jpg");
                             if (System.IO.File.Exists($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}.jpg"))
                             {
                                 if (System.IO.File.Exists($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}.jpg") && System.IO.File.Exists($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}-2.jpg"))
                                 {
                                     exFile = true;
                                     await using Stream stream = System.IO.File.OpenRead($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}.jpg");
                                     await using Stream stream2 = System.IO.File.OpenRead($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}-2.jpg");
                                     await botClient.SendMediaGroupAsync(message.Chat.Id,
                                         media: new IAlbumInputMedia[]
                                         {
                                         new InputMediaPhoto(
                                             InputFile.FromStream(stream, $"{_day}.{_month}.{GetNetworkTime().Year}.jpg")),
                                         new InputMediaPhoto(
                                             InputFile.FromStream(stream2, $"{_day}.{_month}.{GetNetworkTime().Year}-2.jpg"))
                                         }, cancellationToken: token);
                                 }
                                 if (System.IO.File.Exists($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}.jpg") && exFile == false)
                                 {
                                     exFile = true;
                                     await using Stream stream = System.IO.File.OpenRead($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}.jpg");
                                     await botClient.SendPhotoAsync(message.Chat.Id, InputFile.FromStream(stream, $"{_day}.{_month}.{GetNetworkTime().Year}.jpg"), cancellationToken: token);
                                 }
                             }
                        var urlCheckResult = await CheckUrl($"https://mkeiit.ru/wp-content/uploads/{GetNetworkTime().Year}/{_month}/{_day}.{_month}.{GetNetworkTime().Year}.pdf");
                        if (urlCheckResult = true && exFile == false)
                        {
                            await DownLoad($"https://mkeiit.ru/wp-content/uploads/{GetNetworkTime().Year}/{_month}/{_day}.{_month}.{GetNetworkTime().Year}.pdf", $"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}.pdf", GetNetworkTime());
                            ConvertFile(SchFold, GetNetworkTime());
                            Thread.Sleep(1500);
                            if (System.IO.File.Exists($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}.jpg") && System.IO.File.Exists($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}-2.jpg"))
                            {
                                exFile = true;
                                await using Stream stream = System.IO.File.OpenRead($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}.jpg");
                                await using Stream stream2 = System.IO.File.OpenRead($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}-2.jpg");
                                await botClient.SendMediaGroupAsync(message.Chat.Id,
                                    media: new IAlbumInputMedia[]
                                    {
                                    new InputMediaPhoto(
                                        InputFile.FromStream(stream, $"{_day}.{_month}.{GetNetworkTime().Year}.jpg")),
                                    new InputMediaPhoto(
                                        InputFile.FromStream(stream2, $"{_day}.{_month}.{GetNetworkTime().Year}-2.jpg"))
                                    }, cancellationToken: token);
                            }

                            if (System.IO.File.Exists($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}.jpg") && !exFile)
                            {
                                await using Stream stream = System.IO.File.OpenRead($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}.jpg");
                                await botClient.SendPhotoAsync(message.Chat.Id, InputFile.FromStream(stream, $"{_day}.{_month}.{GetNetworkTime().Year}.jpg"), cancellationToken: token);
                            }
                        }
                        else
                        {
                            if (!exFile) await botClient.SendTextMessageAsync(message.Chat.Id, $"Расписания на {GetNetworkTime().ToShortDateString()} нет", cancellationToken: token);
                        }
                        DeletePdf();
                        break;
                    }*/
                    case "/schedule_tomorrow":
                        {
                            var exFile = false;
                            _month = GetNetworkTime().AddDays(1).Month.ToString();
                            _day = GetNetworkTime().AddDays(1).Day.ToString();
                            if (Convert.ToInt32(_month) < 10)
                                _month = "0" + _month;
                            if (_day[0] == '0')
                                _day = _day.TrimStart('0');
                            if (System.IO.File.Exists($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}.jpg"))
                            {
                                if (System.IO.File.Exists($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}.jpg") && System.IO.File.Exists($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}-2.jpg"))
                                {
                                    exFile = true;
                                    await using Stream stream = System.IO.File.OpenRead($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}.jpg");
                                    await using Stream stream2 = System.IO.File.OpenRead($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}-2.jpg");
                                    await botClient.SendMediaGroupAsync(message.Chat.Id,
                                        media: new IAlbumInputMedia[]
                                        {
                                        new InputMediaPhoto(
                                            InputFile.FromStream(stream, $"{_day}.{_month}.{GetNetworkTime().Year}.jpg")),
                                        new InputMediaPhoto(
                                            InputFile.FromStream(stream2, $"{_day}.{_month}.{GetNetworkTime().Year}-2.jpg"))
                                        }, cancellationToken: token);
                                }
                                if (System.IO.File.Exists($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}.jpg") && exFile == false)
                                {
                                    exFile = true;
                                    await using Stream stream = System.IO.File.OpenRead($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}.jpg");
                                    await botClient.SendPhotoAsync(message.Chat.Id, InputFile.FromStream(stream, $"{_day}.{_month}.{GetNetworkTime().Year}.jpg"), cancellationToken: token);
                                }
                            }
                            var urlCheckResult = await CheckUrl($"https://mkeiit.ru/wp-content/uploads/{GetNetworkTime().Year}/{_month}/{_day}.{_month}.{GetNetworkTime().Year}.pdf");
                            if (urlCheckResult && exFile == false)
                            {
                                await DownLoad($"https://mkeiit.ru/wp-content/uploads/{GetNetworkTime().Year}/{_month}/{_day}.{_month}.{GetNetworkTime().Year}.pdf", $"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}.pdf", GetNetworkTime());
                                ConvertFile($"{SchFold}", GetNetworkTime().AddDays(1));
                                Thread.Sleep(2000);
                                if (System.IO.File.Exists($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}.jpg") && System.IO.File.Exists($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}-2.jpg"))
                                {
                                    exFile = true;
                                    await using Stream stream = System.IO.File.OpenRead($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}.jpg");
                                    await using Stream stream2 = System.IO.File.OpenRead($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}-2.jpg");
                                    await botClient.SendMediaGroupAsync(message.Chat.Id,
                                        media: new IAlbumInputMedia[]
                                        {
                                        new InputMediaPhoto(
                                            InputFile.FromStream(stream, $"{_day}.{_month}.{GetNetworkTime().Year}.jpg")),
                                        new InputMediaPhoto(
                                            InputFile.FromStream(stream2, $"{_day}.{_month}.{GetNetworkTime().Year}-2.jpg"))
                                        }, cancellationToken: token);
                                }
                                if (System.IO.File.Exists($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}.jpg") && exFile == false)
                                {
                                    await using Stream stream = System.IO.File.OpenRead($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}.jpg");
                                    await botClient.SendPhotoAsync(message.Chat.Id, InputFile.FromStream(stream, $"{_day}.{_month}.{GetNetworkTime().Year}.jpg"), cancellationToken: token);
                                }
                            }
                            else
                            {
                                if (!exFile)
                                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Расписания на {GetNetworkTime().AddDays(1).ToShortDateString()} нет", cancellationToken: token);
                            }
                            DeletePdf();
                            break;
                        }
                    case "/schedule_today":
                        {
                            var exFile = false;
                            _month = GetNetworkTime().Month.ToString();
                            _day = GetNetworkTime().Day.ToString();
                            if (Convert.ToInt32(_month) < 10)
                                _month = "0" + _month;
                            if (_day[0] == '0')
                                _day = _day.TrimStart('0');
                            if (System.IO.File.Exists($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}.jpg"))
                            {
                                if (System.IO.File.Exists($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}.jpg") && System.IO.File.Exists($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}-2.jpg"))
                                {
                                    exFile = true;
                                    await using Stream stream = System.IO.File.OpenRead($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}.jpg");
                                    await using Stream stream2 = System.IO.File.OpenRead($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}-2.jpg");
                                    await botClient.SendMediaGroupAsync(message.Chat.Id,
                                        media: new IAlbumInputMedia[]
                                        {
                                        new InputMediaPhoto(
                                            InputFile.FromStream(stream, $"{_day}.{_month}.{GetNetworkTime().Year}.jpg")),
                                        new InputMediaPhoto(
                                            InputFile.FromStream(stream2, $"{_day}.{_month}.{GetNetworkTime().Year}-2.jpg"))
                                        }, cancellationToken: token);
                                }
                                if (System.IO.File.Exists($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}.jpg") && exFile == false)
                                {
                                    exFile = true;
                                    await using Stream stream = System.IO.File.OpenRead($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}.jpg");
                                    await botClient.SendPhotoAsync(message.Chat.Id, InputFile.FromStream(stream, $"{_day}.{_month}.{GetNetworkTime().Year}.jpg"), cancellationToken: token);
                                }
                            }
                            var urlCheckResult = await CheckUrl($"https://mkeiit.ru/wp-content/uploads/{GetNetworkTime().Year}/{_month}/{_day}.{_month}.{GetNetworkTime().Year}.pdf");
                            if (urlCheckResult && exFile == false)
                            {
                                await DownLoad($"https://mkeiit.ru/wp-content/uploads/{GetNetworkTime().Year}/{_month}/{_day}.{_month}.{GetNetworkTime().Year}.pdf", $"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}.pdf", GetNetworkTime());
                                ConvertFile($"{SchFold}", GetNetworkTime());
                                Thread.Sleep(2000);
                                if (System.IO.File.Exists($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}.jpg") && System.IO.File.Exists($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}-2.jpg"))
                                {
                                    exFile = true;
                                    await using Stream stream = System.IO.File.OpenRead($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}.jpg");
                                    await using Stream stream2 = System.IO.File.OpenRead($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}-2.jpg");
                                    await botClient.SendMediaGroupAsync(message.Chat.Id,
                                        media: new IAlbumInputMedia[]
                                        {
                                        new InputMediaPhoto(
                                            InputFile.FromStream(stream, $"{_day}.{_month}.{GetNetworkTime().Year}.jpg")),
                                        new InputMediaPhoto(
                                            InputFile.FromStream(stream2, $"{_day}.{_month}.{GetNetworkTime().Year}-2.jpg"))
                                        }, cancellationToken: token);
                                }
                                if (System.IO.File.Exists($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}.jpg") && exFile == false)
                                {
                                    await using Stream stream = System.IO.File.OpenRead($"{SchFold}{_day}.{_month}.{GetNetworkTime().Year}.jpg");
                                    await botClient.SendPhotoAsync(message.Chat.Id, InputFile.FromStream(stream, $"{_day}.{_month}.{GetNetworkTime().Year}.jpg"), cancellationToken: token);
                                }
                            }
                            else
                            {
                                if (!exFile)
                                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Расписания на {GetNetworkTime().ToShortDateString()} нет", cancellationToken: token);
                            }
                            DeletePdf();
                            break;
                        }
                    case "/capybara":
                        {
                            var x = new Random();
                            var rand = x.Next(1, 45);
                            var link = System.IO.File.ReadLines("../../../Fold_data/LinkCapybara.txt").ElementAtOrDefault(rand);
                            if (link != null)
                                await botClient.SendPhotoAsync(chatId: message.Chat.Id,
                                    photo: InputFile.FromUri(link), caption: $"{rand}/45", cancellationToken: token);
                            break;
                        }
                    case "/support":
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Поддержать разработчика: \n\n" +
                                                                              "СберБанк: `5469 4100 1429 4908`\n" +
                                                                              "ВТБ: `2200 2460 4327 6560`\n\n", parseMode: ParseMode.MarkdownV2, cancellationToken: token);
                        break;
                    case "/update":
                        await botClient.SendTextMessageAsync(message.Chat.Id, await System.IO.File.ReadAllTextAsync("../../../Fold_data/update.txt", token), cancellationToken: token);
                        break;
                    case "/future_updates":
                        await botClient.SendTextMessageAsync(message.Chat.Id, await System.IO.File.ReadAllTextAsync("../../../Fold_data/fut_update.txt", token), cancellationToken: token);
                        break;
                    case "/bugs":
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Сообщить об ошибке:\nTG: @n0rd_kr1eg\n" + "VK: https://vk.com/n0rd_kr1eg" + "\n\nВремя ответа: 5-15 минут", cancellationToken: token);
                        break;
                    case "/news":
                        await botClient.SendTextMessageAsync(message.Chat.Id, "В разработке :)", cancellationToken: token);
                        break;
                    case "/schedule_session":
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Информации по сессии нет :(", cancellationToken: token);
                        break;
                    case "/teacher_list":
                        await botClient.SendTextMessageAsync(message.Chat.Id, "В разработке...", cancellationToken: token);
                        break;
                    default:
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Неверная команда!",
                            cancellationToken: token);
                        break;
                }
            }
        }
        private static Task Error(ITelegramBotClient client, Exception exception, CancellationToken token) => throw new NotImplementedException();
    }
    public class Service : IHostedService
    {
        public Service() {}
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

}