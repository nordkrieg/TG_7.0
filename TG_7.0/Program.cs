using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TG_7._0
{
    internal abstract class Program : OthersMethods
    {
        private static string _month, _day;
        private static void Main()
        static void Main(string[] args)
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
            Console.WriteLine($"Getting started: {DateTime.Now}");
            Console.WriteLine($"Name PC: {System.Net.Dns.GetHostName()}");
            Console.WriteLine($"Name User PC: {Environment.UserName}");

            Console.WriteLine();
            Console.WriteLine(string.Concat(Enumerable.Repeat("-", 120)));
            Console.WriteLine();
            Console.WriteLine("Bot Started...");
            Console.WriteLine();
            Console.WriteLine(string.Concat(Enumerable.Repeat("-", 120)));
            Console.WriteLine();
            Console.ReadLine();
        }

        //Update message from Users  
        private static async Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            var message = update.Message;
            if (message is { Text: not null } && AccessUser.Contains(Convert.ToString( message.Chat.Id)))
            {
                Console.WriteLine($"User: {message.Chat.Username}");
                Console.WriteLine($"Name: {message.Chat.FirstName}");
                Console.WriteLine($"Surnameame: {message.Chat.LastName}");
                Console.WriteLine($"ID Chat: {message.Chat.Id}");
                Console.WriteLine($"Time: {DateTime.Now}");
                Console.WriteLine($"Text: {message.Text}");
                Console.WriteLine();

                await System.IO.File.AppendAllTextAsync("../../../Fold_data/historeChat.txt", $"User: {message.Chat.Username}", token);
                await System.IO.File.AppendAllTextAsync("../../../Fold_data/historeChat.txt", $"\nName: {message.Chat.FirstName}", token);
                await System.IO.File.AppendAllTextAsync("../../../Fold_data/historeChat.txt", $"\nSurnameame: {message.Chat.LastName}", token);
                await System.IO.File.AppendAllTextAsync("../../../Fold_data/historeChat.txt", $"\nID Chat: {message.Chat.Id}", token);
                await System.IO.File.AppendAllTextAsync("../../../Fold_data/historeChat.txt", $"\nTime: {DateTime.Now}", token);
                await System.IO.File.AppendAllTextAsync("../../../Fold_data/historeChat.txt", $"\nText: {message.Text}", token);
                await System.IO.File.AppendAllTextAsync("../../../Fold_data/historeChat.txt", "\n\n", token);

                if (Value.Contains(message.Text.ToLower()) == false && message.Text.ToLower().Contains("!sc") == false)
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Неверная команда!", cancellationToken: token);
                }

                if (message.Text.ToLower() == "/start")
                {
                    await botClient.SendPhotoAsync(message.Chat.Id, InputFile.FromUri("https://sun9-33.userapi.com/impg/ryNcRmoOZGUm-_faB4kaVQV4ywo-3BL2R2GFVg/jlav8UuJ7Us.jpg?size=430x178&quality=95&sign=17941b56a7056aa8549bb8765cfaa472&type=album"),
                        caption: "Добро пожаловать!\n\nДля пользования ботом советую прочитать сводку команд, использовав /info в меню", cancellationToken: token);
                }
                switch (message.Text.ToLower())
                {
                    case "/start" when AccessUser.Contains(Convert.ToString(message.Chat.Id)):
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Уважаемый тестировщик, добро пожаловать! " +
                                                                              "Спасибо за участие в бета-тестировании бота! Пожалуйста, действуйте согласно вашим требованиям, и не забывайте отсылать результаты о проведении тестирования - разработчику ;)", cancellationToken: token);
                        break;
                    case "/info":
                        await botClient.SendTextMessageAsync(message.Chat.Id, await System.IO.File.ReadAllTextAsync("../../../Fold_data/information.txt", token), cancellationToken: token);
                        break;
                    case "привет":
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Добро пожаловать!", cancellationToken: token);
                        break;
                    case "дата":
                        await botClient.SendTextMessageAsync(message.Chat.Id, DateTime.Now.DayOfWeek.ToString(), cancellationToken: token);
                        break;
                    case "/call_schedule":
                        await botClient.SendMediaGroupAsync(message.Chat.Id, media:
                            new IAlbumInputMedia[]
                            {
                                new InputMediaPhoto(
                                    InputFile.FromUri("https://sun9-57.userapi.com/impg/YwVh6wWjfC6g3riZcAkPdwXFzKhSRgjow8Yc3A/Lck6nMQyVJE.jpg?size=1620x2160&quality=95&sign=afce1a14a9edbbaa7e840a7ebce0ab20&type=album")),
                                new InputMediaPhoto(
                                    InputFile.FromUri("https://sun9-50.userapi.com/impg/aFRzasi9vHU6xkUdM9P0xA1XqSMVoG8bHk9WmQ/8GItW_4b3Os.jpg?size=1620x2160&quality=95&sign=394ff86c1105f04988c56f17f11a3a63&type=album"))
                            }, cancellationToken: token);
                        break;
                    case "/schedule_today":
                    {
                        var exFile = false;
                        _month = DateTime.Now.Month.ToString();
                        _day = DateTime.Now.Day.ToString();
                        if (Convert.ToInt32(_month) < 10)
                            _month = "0" + _month;

                        if (_day[0] == '0')
                            _day = _day.TrimStart('0');

                        if (System.IO.File.Exists($"{SchFold}{_day}.{_month}.{DateTime.Now.Year}.jpg"))
                        {
                            if (System.IO.File.Exists($"{SchFold}{_day}.{_month}.{DateTime.Now.Year}.jpg") && System.IO.File.Exists($"{SchFold}{_day}.{_month}.{DateTime.Now.Year}-2.jpg"))
                            {
                                exFile = true;
                                await using Stream stream = System.IO.File.OpenRead($"{SchFold}{_day}.{_month}.{DateTime.Now.Year}.jpg");
                                await using Stream stream2 = System.IO.File.OpenRead($"{SchFold}{_day}.{_month}.{DateTime.Now.Year}-2.jpg");
                                await botClient.SendMediaGroupAsync(message.Chat.Id,
                                    media: new IAlbumInputMedia[]
                                    {
                                        new InputMediaPhoto(
                                            InputFile.FromStream(stream, $"{_day}.{_month}.{DateTime.Now.Year}.jpg")),
                                        new InputMediaPhoto(
                                            InputFile.FromStream(stream2, $"{_day}.{_month}.{DateTime.Now.Year}-2.jpg"))
                                    }, cancellationToken: token);
                            }

                            if (System.IO.File.Exists($"{SchFold}{_day}.{_month}.{DateTime.Now.Year}.jpg") && exFile == false)
                            {
                                exFile = true;
                                await using Stream stream = System.IO.File.OpenRead($"{SchFold}{_day}.{_month}.{DateTime.Now.Year}.jpg");
                                await botClient.SendPhotoAsync(message.Chat.Id, InputFile.FromStream(stream, $"{_day}.{_month}.{DateTime.Now.Year}.jpg"), cancellationToken: token);
                            }
                        }
                        var urlCheckResult = await CheckUrl($"https://mkeiit.ru/wp-content/uploads/{DateTime.Now.Year}/{_month}/{_day}.{_month}.{DateTime.Now.Year}.pdf");
                        if (urlCheckResult != true && exFile == false)
                        {
                            await DownLoad($"https://mkeiit.ru/wp-content/uploads/{DateTime.Now.Year}/{_month}/{_day}.{_month}.{DateTime.Now.Year}.pdf", $"{SchFold}{_day}.{_month}.{DateTime.Now.Year}.pdf", DateTime.Now);
                            ConvertFile(SchFold, DateTime.Now);
                            Thread.Sleep(1500);
                            if (System.IO.File.Exists($"{SchFold}{_day}.{_month}.{DateTime.Now.Year}.jpg") && System.IO.File.Exists($"{SchFold}{_day}.{_month}.{DateTime.Now.Year}-2.jpg"))
                            {
                                exFile=true;
                                await using Stream stream = System.IO.File.OpenRead($"{SchFold}{_day}.{_month}.{DateTime.Now.Year}.jpg");
                                await using Stream stream2 = System.IO.File.OpenRead($"{SchFold}{_day}.{_month}.{DateTime.Now.Year}-2.jpg");
                                await botClient.SendMediaGroupAsync(message.Chat.Id,
                                    media: new IAlbumInputMedia[]
                                    {
                                        new InputMediaPhoto(
                                            InputFile.FromStream(stream, $"{_day}.{_month}.{DateTime.Now.Year}.jpg")),
                                        new InputMediaPhoto(
                                            InputFile.FromStream(stream2, $"{_day}.{_month}.{DateTime.Now.Year}-2.jpg"))
                                    }, cancellationToken: token);
                            }

                            if (System.IO.File.Exists($"{SchFold}{_day}.{_month}.{DateTime.Now.Year}.jpg") && !exFile)
                            {
                                await using Stream stream = System.IO.File.OpenRead($"{SchFold}{_day}.{_month}.{DateTime.Now.Year}.jpg");
                                await botClient.SendPhotoAsync(message.Chat.Id, InputFile.FromStream(stream, $"{_day}.{_month}.{DateTime.Now.Year}.jpg"), cancellationToken: token);
                            }
                        }
                        else
                        {
                            if (!exFile)
                                await botClient.SendTextMessageAsync(message.Chat.Id, $"Расписания на {DateTime.Now.ToShortDateString()} нет", cancellationToken: token);
                        }
                        DeletePdf();
                        break;
                    }
                    case "/schedule_tomorrow":
                    {
                        var exFile = false;
                        _month = DateTime.Now.AddDays(1).Month.ToString();
                        _day = DateTime.Now.AddDays(1).Day.ToString();
                        if (Convert.ToInt32(_month) < 10)
                            _month = "0" + _month;
                        if (_day[0] == '0')
                            _day = _day.TrimStart('0');
                        if (System.IO.File.Exists($"{SchFold}{_day}.{_month}.{DateTime.Now.Year}.jpg"))
                        {
                            if (System.IO.File.Exists($"{SchFold}{_day}.{_month}.{DateTime.Now.Year}.jpg") && System.IO.File.Exists($"{SchFold}{_day}.{_month}.{DateTime.Now.Year}-2.jpg"))
                            {
                                exFile = true;
                                await using Stream stream = System.IO.File.OpenRead($"{SchFold}{_day}.{_month}.{DateTime.Now.Year}.jpg");
                                await using Stream stream2 = System.IO.File.OpenRead($"{SchFold}{_day}.{_month}.{DateTime.Now.Year}-2.jpg");
                                await botClient.SendMediaGroupAsync(message.Chat.Id,
                                    media: new IAlbumInputMedia[]
                                    {
                                        new InputMediaPhoto(
                                            InputFile.FromStream(stream, $"{_day}.{_month}.{DateTime.Now.Year}.jpg")),
                                        new InputMediaPhoto(
                                            InputFile.FromStream(stream2, $"{_day}.{_month}.{DateTime.Now.Year}-2.jpg"))
                                    }, cancellationToken: token);
                            }
                            if (System.IO.File.Exists($"{SchFold}{_day}.{_month}.{DateTime.Now.Year}.jpg") && exFile == false)
                            {
                                exFile = true;
                                await using Stream stream = System.IO.File.OpenRead($"{SchFold}{_day}.{_month}.{DateTime.Now.Year}.jpg");
                                await botClient.SendPhotoAsync(message.Chat.Id, InputFile.FromStream(stream, $"{_day}.{_month}.{DateTime.Now.Year}.jpg"), cancellationToken: token);
                            }
                        }
                        var urlCheckResult = await CheckUrl($"https://mkeiit.ru/wp-content/uploads/{DateTime.Now.Year}/{_month}/{_day}.{_month}.{DateTime.Now.Year}.pdf");
                        if (urlCheckResult && exFile == false)
                        {
                            await DownLoad($"https://mkeiit.ru/wp-content/uploads/{DateTime.Now.Year}/{_month}/{_day}.{_month}.{DateTime.Now.Year}.pdf", $"{SchFold}{_day}.{_month}.{DateTime.Now.Year}.pdf", DateTime.Now);
                            ConvertFile($"{SchFold}", DateTime.Now.AddDays(1));
                            Thread.Sleep(2000);
                            if (System.IO.File.Exists($"{SchFold}{_day}.{_month}.{DateTime.Now.Year}.jpg") && System.IO.File.Exists($"{SchFold}{_day}.{_month}.{DateTime.Now.Year}-2.jpg"))
                            {
                                exFile = true;
                                await using Stream stream = System.IO.File.OpenRead($"{SchFold}{_day}.{_month}.{DateTime.Now.Year}.jpg");
                                await using Stream stream2 = System.IO.File.OpenRead($"{SchFold}{_day}.{_month}.{DateTime.Now.Year}-2.jpg");
                                await botClient.SendMediaGroupAsync(message.Chat.Id,
                                    media: new IAlbumInputMedia[]
                                    {
                                        new InputMediaPhoto(
                                            InputFile.FromStream(stream, $"{_day}.{_month}.{DateTime.Now.Year}.jpg")),
                                        new InputMediaPhoto(
                                            InputFile.FromStream(stream2, $"{_day}.{_month}.{DateTime.Now.Year}-2.jpg"))
                                    }, cancellationToken: token);
                            }
                            if (System.IO.File.Exists($"{SchFold}{_day}.{_month}.{DateTime.Now.Year}.jpg") && exFile == false)
                            {
                                await using Stream stream = System.IO.File.OpenRead($"{SchFold}{_day}.{_month}.{DateTime.Now.Year}.jpg");
                                await botClient.SendPhotoAsync(message.Chat.Id, InputFile.FromStream(stream, $"{_day}.{_month}.{DateTime.Now.Year}.jpg"), cancellationToken: token);
                            }
                        }
                        else
                        {
                            if (!exFile)
                                await botClient.SendTextMessageAsync(message.Chat.Id, $"Расписания на {DateTime.Now.AddDays(1).ToShortDateString()} нет", cancellationToken: token);
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
                }


                if (message.Text == "/stop_test")
                    for (int i = 0; i < 4; i++)
                    {
                        await botClient.SendPhotoAsync(IdUs[i], 
                            photo: InputFile.FromUri("http://s00.yaplakal.com/pics/pics_original/4/0/8/17305804.jpg"),
                            caption: "Дорогой тестировщик! Благодарю за участие в закрытом бета-тестировании бота! Ваши результаты помогут уладить ошибки, и продвинут бота к выходу в релиз! До новых встреч..!", cancellationToken: token);
                    }

                switch(message.Text)
                {
                    case "/st_test":
                        for (int i = 0; i < 4; i++)
                        {
                            await botClient.SendTextMessageAsync(IdUs[i], "Доброе утро, тестировщик! Напоминаю, что сегодняшнее тестирование началось, и продлится до 19 часов!", cancellationToken: token);
                        }
                        break;
                    case "/stop_test_tod":
                        for (int i = 0; i < 4; i++)
                        {
                            await botClient.SendTextMessageAsync(IdUs[i], "Тестирование на сегодня окончено. Не забудьте сообщить разработчику о результатах тестирования. До завтра!", cancellationToken: token);
                        }
                        break;
                    case "/tech":
                        for (int i = 0; i < 4; i++)
                        {
                            await botClient.SendTextMessageAsync(IdUs[i], "Тестирование временно приостановленно. Ожидайте.", cancellationToken: token);
                        }
                        break;
                    case "/return":
                        for (int i = 0; i < 4; i++)
                        {
                            await botClient.SendTextMessageAsync(IdUs[i], "Тестирование возобновлено! Можете продолжать работать!", cancellationToken: token);
                        }
                        break;

                }

                if (message.Text == "/info_dev")
                    await botClient.SendTextMessageAsync(message.Chat.Id, await System.IO.File.ReadAllTextAsync("../../../Fold_data/infodev.txt", token), cancellationToken: token);                
            }
            else if (message != null)
                await botClient.SendTextMessageAsync(message.Chat.Id, "Доступ запрещен!", cancellationToken: token);
        }
        private static Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}