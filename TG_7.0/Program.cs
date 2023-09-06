using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

// ПОМЕНЯТЬ МЕСТАМИ ПРОВЕРКУ ОДНОГО И НЕСКОЛЬКИХ ФАЙЛОВ!!!
namespace TelegramBotFor3P_v0._1
{
    internal class Program : Others_Methods
    {

        public static string year, month, day;
        static void Main(string[] args)
        {

            //API Key Telegram Bot|Start Bot
            var botClient = new TelegramBotClient("6348440231:AAFO28UNHkVkNAw6JQ5kKg8_kdeo-7MjCsE");
            botClient.StartReceiving(Update, Error); //Start and first Function
            Console.WriteLine(String.Concat(Enumerable.Repeat("-", 120)));
            Console.Write(String.Concat(Enumerable.Repeat("\t", 5)));
            Console.WriteLine("WELCOME TO LessonAssistant Bot!");
            Console.WriteLine(String.Concat(Enumerable.Repeat("-", 120)));
            Console.Write(String.Concat(Enumerable.Repeat("\t", 2)));
            Console.WriteLine("All information is in the telegram channel: https://t.me/lesson_assistant_backstage !");
            Console.WriteLine(String.Concat(Enumerable.Repeat("-", 120)));
            Console.WriteLine("Setting:");
            Console.WriteLine($"Bot Version {version_bot}");

            Thread.Sleep(600);
            Console.WriteLine($"Getting started: {DateTime.Now}");

            Thread.Sleep(600);
            Console.WriteLine($"Name PC: {System.Net.Dns.GetHostName()}");

            Thread.Sleep(600);
            Console.WriteLine($"Name User PC: {System.Environment.UserName}");

            Thread.Sleep(1000);
            Console.WriteLine();

            Console.WriteLine(String.Concat(Enumerable.Repeat("-", 120)));

            Console.WriteLine();

            Console.WriteLine("Bot Started...");

            Thread.Sleep(1000);
            Console.WriteLine();

            Console.WriteLine(String.Concat(Enumerable.Repeat("-", 120)));

            Console.WriteLine();

            Console.ReadLine();

        }


        //Update message from Users  
        async static Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            var message = update.Message;
            if (message.Text != null && access_user.Contains(Convert.ToString( message.Chat.Id)))
            {

                Console.WriteLine($"User: {message.Chat.Username}");
                Console.WriteLine($"Name: {message.Chat.FirstName}");
                Console.WriteLine($"Surnameame: {message.Chat.LastName}");
                Console.WriteLine($"ID Chat: {message.Chat.Id}");
                Console.WriteLine($"Time: {DateTime.Now}");
                Console.WriteLine($"Text: {message.Text}");
                Console.WriteLine();

                await System.IO.File.AppendAllTextAsync("../../../Fold_data/historeChat.txt", $"User: {message.Chat.Username}");
                await System.IO.File.AppendAllTextAsync("../../../Fold_data/historeChat.txt", $"\nName: {message.Chat.FirstName}");
                await System.IO.File.AppendAllTextAsync("../../../Fold_data/historeChat.txt", $"\nSurnameame: {message.Chat.LastName}");
                await System.IO.File.AppendAllTextAsync("../../../Fold_data/historeChat.txt", $"\nID Chat: {message.Chat.Id}");
                await System.IO.File.AppendAllTextAsync("../../../Fold_data/historeChat.txt", $"\nTime: {DateTime.Now}");
                await System.IO.File.AppendAllTextAsync("../../../Fold_data/historeChat.txt", $"\nText: {message.Text}");
                await System.IO.File.AppendAllTextAsync("../../../Fold_data/historeChat.txt", "\n\n");




                if (command.Contains(message.Text.ToLower()) == false && message.Text.ToLower().Contains("!sc") == false)
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Неверная команда!");
                }




                if (message.Text.ToLower() == "/start")
                {
                    await botClient.SendPhotoAsync(message.Chat.Id, InputFile.FromUri("https://sun9-33.userapi.com/impg/ryNcRmoOZGUm-_faB4kaVQV4ywo-3BL2R2GFVg/jlav8UuJ7Us.jpg?size=430x178&quality=95&sign=17941b56a7056aa8549bb8765cfaa472&type=album"),
                        caption: "Добро пожаловать!\n\nДля пользования ботом советую прочитать сводку команд, использовав /info в меню");

                }

                if (message.Text.ToLower() == "/start" && access_user.Contains(Convert.ToString(message.Chat.Id)))
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Уважаемый тестировщик, добро пожаловать! " +
                        "Спасибо за участие в бета-тестировании бота! Пожалуйста, действуйте согласно вашим требованиям, и не забывайте отсылать результаты о проведении тестирования - разработчику ;)");
                }

                    if (message.Text.ToLower() == "/info")
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, System.IO.File.ReadAllText("../../../Fold_data/information.txt"));
                }




                //Hello
                if (message.Text.ToLower().Contains("привет"))
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Добро пожаловать!");
                    return;
                }




                //Check Date Now
                if (message.Text.ToLower().Contains("дата"))
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, DateTime.Now.DayOfWeek.ToString());
                }




                //Object Menu for call schedule
                if (message.Text.ToLower() == "/call_schedule")
                {
                    await botClient.SendMediaGroupAsync(message.Chat.Id, media:
                        new IAlbumInputMedia[]
                        {
                            new InputMediaPhoto(
                                InputFile.FromUri("https://sun9-57.userapi.com/impg/YwVh6wWjfC6g3riZcAkPdwXFzKhSRgjow8Yc3A/Lck6nMQyVJE.jpg?size=1620x2160&quality=95&sign=afce1a14a9edbbaa7e840a7ebce0ab20&type=album")),
                            new InputMediaPhoto(
                                InputFile.FromUri("https://sun9-50.userapi.com/impg/aFRzasi9vHU6xkUdM9P0xA1XqSMVoG8bHk9WmQ/8GItW_4b3Os.jpg?size=1620x2160&quality=95&sign=394ff86c1105f04988c56f17f11a3a63&type=album"))
                        });
                }




                //Object Menu for schedule
                if (message.Text.ToLower() == "/schedule_today")
                {
                    bool ExFile = false;
                    month = DateTime.Now.Month.ToString();
                    day = DateTime.Now.Day.ToString();
                    if (Convert.ToInt32(month) < 10)
                    {
                        month = "0" + month;
                    }

                    if (day[0] == '0')
                    {
                        day = day.TrimStart('0');
                    }

                    if (System.IO.File.Exists($"{sch_fold}{day}.{month}.{DateTime.Now.Year}.jpg"))
                    {


                        if (System.IO.File.Exists($"{sch_fold}{day}.{month}.{DateTime.Now.Year}.jpg") && System.IO.File.Exists($"{sch_fold}{day}.{month}.{DateTime.Now.Year}-2.jpg"))
                        {
                            ExFile = true;
                            await using Stream stream = System.IO.File.OpenRead($"{sch_fold}{day}.{month}.{DateTime.Now.Year}.jpg");
                            await using Stream stream_2 = System.IO.File.OpenRead($"{sch_fold}{day}.{month}.{DateTime.Now.Year}-2.jpg");
                            await botClient.SendMediaGroupAsync(message.Chat.Id,
                                    media: new IAlbumInputMedia[]
                                    {
                                    new InputMediaPhoto(
                                        InputFile.FromStream(stream, $"{day}.{month}.{DateTime.Now.Year}.jpg")),
                                    new InputMediaPhoto(
                                        InputFile.FromStream(stream_2, $"{day}.{month}.{DateTime.Now.Year}-2.jpg"))
                                    });
                        }

                        if (System.IO.File.Exists($"{sch_fold}{day}.{month}.{DateTime.Now.Year}.jpg") && ExFile == false)
                        {
                            ExFile = true;
                            await using Stream stream = System.IO.File.OpenRead($"{sch_fold}{day}.{month}.{DateTime.Now.Year}.jpg");
                            await botClient.SendPhotoAsync(message.Chat.Id, InputFile.FromStream(stream, $"{day}.{month}.{DateTime.Now.Year}.jpg"));
                        }
                    }

                    if (CheckURL($"https://mkeiit.ru/wp-content/uploads/{DateTime.Now.Year}/{month}/{day}.{month}.{DateTime.Now.Year}.pdf") == true && ExFile == false)
                    {
                        /*Message message_pdf = await botClient.SendDocumentAsync(chatId: message.Chat.Id,
                        document: InputFile.FromUri($"https://mkeiit.ru/wp-content/uploads/{DateTime.Now.Year}/{month}/{DateTime.Now.ToShortDateString()}.pdf"),
                        caption: $"Расписание на {DateTime.Now.ToShortDateString()}");*/

                       /* DirectoryInfo dirInfo = new DirectoryInfo(sch_fold);

                        foreach (FileInfo file in dirInfo.GetFiles())
                        {
                            file.Delete();
                        }*/

                        DownLoad($"https://mkeiit.ru/wp-content/uploads/{DateTime.Now.Year}/{month}/{day}.{month}.{DateTime.Now.Year}.pdf", $"{sch_fold}{day}.{month}.{DateTime.Now.Year}.pdf", DateTime.Now);
                        ConvertFile(sch_fold, DateTime.Now);

                        Thread.Sleep(1500);



                        if (System.IO.File.Exists($"{sch_fold}{day}.{month}.{DateTime.Now.Year}.jpg") && System.IO.File.Exists($"{sch_fold}{day}.{month}.{DateTime.Now.Year}-2.jpg"))
                        {
                            ExFile=true;
                            await using Stream stream = System.IO.File.OpenRead($"{sch_fold}{day}.{month}.{DateTime.Now.Year}.jpg");
                            await using Stream stream_2 = System.IO.File.OpenRead($"{sch_fold}{day}.{month}.{DateTime.Now.Year}-2.jpg");
                            await botClient.SendMediaGroupAsync(message.Chat.Id,
                                    media: new IAlbumInputMedia[]
                                    {
                                    new InputMediaPhoto(
                                        InputFile.FromStream(stream, $"{day}.{month}.{DateTime.Now.Year}.jpg")),
                                    new InputMediaPhoto(
                                        InputFile.FromStream(stream_2, $"{day}.{month}.{DateTime.Now.Year}-2.jpg"))
                                    });
                        }

                        if (System.IO.File.Exists($"{sch_fold}{day}.{month}.{DateTime.Now.Year}.jpg") && !ExFile)
                        {
                            ExFile = true;
                            await using Stream stream = System.IO.File.OpenRead($"{sch_fold}{day}.{month}.{DateTime.Now.Year}.jpg");
                            await botClient.SendPhotoAsync(message.Chat.Id, InputFile.FromStream(stream, $"{day}.{month}.{DateTime.Now.Year}.jpg"));
                        }
                    }
                    else
                    {
                        if (!ExFile)
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, $"Расписания на {DateTime.Now.ToShortDateString()} нет");
                            //await botClient.SendTextMessageAsync(message.Chat.Id, $"https://mkeiit.ru/wp-content/uploads/{DateTime.Now.Year}/{month}/{day}.{month}.{DateTime.Now.Year}.pdf");
                        }
                    }
                    DeletePDF();
                }



                //Object Menu for schedule tomorrow
                if (message.Text.ToLower() == "/schedule_tomorrow")
                {
                    bool ExFile = false;
                    month = DateTime.Now.AddDays(1).Month.ToString();
                    day = DateTime.Now.AddDays(1).Day.ToString();
                    if (Convert.ToInt32(month) < 10)
                    {
                        month = "0" + month;
                    }

                    if (day[0] == '0')
                    {
                        day = day.TrimStart('0');
                    }

                    if (System.IO.File.Exists($"{sch_fold}{day}.{month}.{DateTime.Now.Year}.jpg"))
                    {

                        if (System.IO.File.Exists($"{sch_fold}{day}.{month}.{DateTime.Now.Year}.jpg") && System.IO.File.Exists($"{sch_fold}{day}.{month}.{DateTime.Now.Year}-2.jpg"))
                        {
                            ExFile = true;
                            await using Stream stream = System.IO.File.OpenRead($"{sch_fold}{day}.{month}.{DateTime.Now.Year}.jpg");
                            await using Stream stream_2 = System.IO.File.OpenRead($"{sch_fold}{day}.{month}.{DateTime.Now.Year}-2.jpg");
                            await botClient.SendMediaGroupAsync(message.Chat.Id,
                                    media: new IAlbumInputMedia[]
                                    {
                                    new InputMediaPhoto(
                                        InputFile.FromStream(stream, $"{day}.{month}.{DateTime.Now.Year}.jpg")),
                                    new InputMediaPhoto(
                                        InputFile.FromStream(stream_2, $"{day}.{month}.{DateTime.Now.Year}-2.jpg"))
                                    });
                        }

                        if (System.IO.File.Exists($"{sch_fold}{day}.{month}.{DateTime.Now.Year}.jpg") && ExFile == false)
                        {
                            ExFile = true;
                            await using Stream stream = System.IO.File.OpenRead($"{sch_fold}{day}.{month}.{DateTime.Now.Year}.jpg");
                            await botClient.SendPhotoAsync(message.Chat.Id, InputFile.FromStream(stream, $"{day}.{month}.{DateTime.Now.Year}.jpg"));
                        }
                    }

                    if (CheckURL($"https://mkeiit.ru/wp-content/uploads/{DateTime.Now.Year}/{month}/{day}.{month}.{DateTime.Now.Year}.pdf") == true && ExFile == false)
                    {
                       

                        /*Message message_pdf = await botClient.SendDocumentAsync(chatId: message.Chat.Id,
                        document: InputFile.FromUri($"https://mkeiit.ru/wp-content/uploads/{DateTime.Now.Year}/{month}/{day}.{month}.{DateTime.Now.Year}.pdf"),
                        caption: $"Расписание на {DateTime.Now.AddDays(1).ToShortDateString()}");*/

                        /*DirectoryInfo dirInfo = new DirectoryInfo(sch_fold);


                        foreach (FileInfo file in dirInfo.GetFiles())
                        {
                            file.Delete();
                        }*/


                        DownLoad($"https://mkeiit.ru/wp-content/uploads/{DateTime.Now.Year}/{month}/{day}.{month}.{DateTime.Now.Year}.pdf", $"{sch_fold}{day}.{month}.{DateTime.Now.Year}.pdf", DateTime.Now);
                        
                        ConvertFile($"{sch_fold}", DateTime.Now.AddDays(1));

                        
                        Thread.Sleep(2000);
                      

                        if (System.IO.File.Exists($"{sch_fold}{day}.{month}.{DateTime.Now.Year}.jpg") && System.IO.File.Exists($"{sch_fold}{day}.{month}.{DateTime.Now.Year}-2.jpg"))
                        {
                            ExFile = true;
                            await using Stream stream = System.IO.File.OpenRead($"{sch_fold}{day}.{month}.{DateTime.Now.Year}.jpg");
                            await using Stream stream_2 = System.IO.File.OpenRead($"{sch_fold}{day}.{month}.{DateTime.Now.Year}-2.jpg");
                            await botClient.SendMediaGroupAsync(message.Chat.Id,
                                    media: new IAlbumInputMedia[]
                                    {
                                    new InputMediaPhoto(
                                        InputFile.FromStream(stream, $"{day}.{month}.{DateTime.Now.Year}.jpg")),
                                    new InputMediaPhoto(
                                        InputFile.FromStream(stream_2, $"{day}.{month}.{DateTime.Now.Year}-2.jpg"))
                                    });
                        }

                        if (System.IO.File.Exists($"{sch_fold}{day}.{month}.{DateTime.Now.Year}.jpg") && ExFile == false)
                        {
                            ExFile = true;
                            await using Stream stream = System.IO.File.OpenRead($"{sch_fold}{day}.{month}.{DateTime.Now.Year}.jpg");
                            await botClient.SendPhotoAsync(message.Chat.Id, InputFile.FromStream(stream, $"{day}.{month}.{DateTime.Now.Year}.jpg"));
                        }

                    }
                    else
                    {
                        if (!ExFile)
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, $"Расписания на {DateTime.Now.AddDays(1).ToShortDateString()} нет");
                        }
                    }

                    DeletePDF();
                }




                    //Object Menu for Image Capybara
                if (message.Text.ToLower() == "/capybara")
                {
                    Random x = new Random();
                    int rand = x.Next(1, 45);
                    string link = System.IO.File.ReadLines("../../../Fold_data/LinkCapybara.txt").ElementAtOrDefault(rand);
                    await botClient.SendPhotoAsync(chatId: message.Chat.Id,
                        photo: InputFile.FromUri(link), caption: $"{rand}/45");
                }




                //support money
                if (message.Text.ToLower() == "/support")
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Поддержать разработчика: \n\n" +
                        "СберБанк: `5469 4100 1429 4908`\n" +
                        "ВТБ: `2200 2460 4327 6560`\n\n", parseMode: ParseMode.MarkdownV2);
                }




                //updates
                if (message.Text.ToLower() == "/update")
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, System.IO.File.ReadAllText("../../../Fold_data/update.txt"));
                }




                //fut_updates
                if (message.Text.ToLower() == "/future_updates")
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, System.IO.File.ReadAllText("../../../Fold_data/fut_update.txt"));
                }


                //message bugs
                if (message.Text.ToLower() == "/bugs")
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Сообщить об ошибке:\nTG: @n0rd_kr1eg\n" + "VK: https://vk.com/n0rd_kr1eg" +
                        "\n\nВремя ответа: 5-15 минут");
                }

                //news_college
                if (message.Text.ToLower() == "/news")
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "В разработке :)");
                }

                //session
                if (message.Text.ToLower() == "/schedule_session")
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Информации по сессии нет :(");
                }

                if (message.Text.ToLower() == "/teacher_list")
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "В разработке...");
                }

                if (message.Text == "/stop_test")
                {
                    for (int i = 0; i < 4; i++)
                    {
                        await botClient.SendPhotoAsync(id_us[i], 
                            photo: InputFile.FromUri("http://s00.yaplakal.com/pics/pics_original/4/0/8/17305804.jpg"),
                            caption: "Дорогой тестировщик! Благодарю за участие в закрытом бета-тестировании бота! Ваши результаты помогут уладить ошибки, и продвинут бота к выходу в релиз! До новых встреч..!");
                    }
                }

                switch(message.Text)
                {
                    case "/st_test":
                        for (int i = 0; i < 4; i++)
                        {
                            await botClient.SendTextMessageAsync(id_us[i], "Доброе утро, тестировщик! Напоминаю, что сегодняшнее тестирование началось, и продлится до 19 часов!");
                        }
                        break;
                    case "/stop_test_tod":
                        for (int i = 0; i < 4; i++)
                        {
                            await botClient.SendTextMessageAsync(id_us[i], "Тестирование на сегодня окончено. Не забудьте сообщить разработчику о результатах тестирования. До завтра!");
                        }
                        break;
                    case "/tech":
                        for (int i = 0; i < 4; i++)
                        {
                            await botClient.SendTextMessageAsync(id_us[i], "Тестирование временно приостановленно. Ожидайте.");
                        }
                        break;
                    case "/return":
                        for (int i = 0; i < 4; i++)
                        {
                            await botClient.SendTextMessageAsync(id_us[i], "Тестирование возобновлено! Можете продолжать работать!");
                        }
                        break;
                }

                if (message.Text == "/info_dev")
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, System.IO.File.ReadAllText("../../../Fold_data/infodev.txt"));                
                }
               

            }
            else
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Доступ запрещен!");
            }
        }

        private static Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            throw new NotImplementedException();
        }

    }
}
