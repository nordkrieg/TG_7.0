using System.Globalization;
using System.Timers;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableMethods.FormattingOptions;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;
using Telegram.BotAPI.UpdatingMessages;
using File = System.IO.File;
using Timer = System.Timers.Timer;

namespace Schedule;

internal abstract class Program
{
    private static readonly BotClient Bot = new("6348440231:AAFO28UNHkVkNAw6JQ5kKg8_kdeo-7MjCsE");
    private static Timer _timer;
    private static async Task Main()
    {
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        Console.WriteLine("Ужики я жив....");
        var cts = new CancellationTokenSource();
        _timer = new Timer(300000);
        _timer.Elapsed += OnTimedEvent;
        _timer.AutoReset = true;
        _timer.Enabled = true;
        while (!cts.Token.IsCancellationRequested)
            try
            {
                var updates = await Bot.GetUpdatesAsync();
                if (updates.Length > 0)
                {
                    foreach (var update in updates)
                        switch (update.Type)
                        {
                            case UpdateType.Message:
                                await OnMessage(update.Message, cts.Token);
                                break;
                            case UpdateType.CallbackQuery:
                                await OnCallbackQuery(update.CallbackQuery, Bot, cts.Token);
                                break;
                        }

                    var maxUpdateId = updates.Max(u => u.UpdateId) + 1;
                    await Bot.GetUpdatesAsync(maxUpdateId, cancellationToken: cts.Token);
                }
                else
                {
                    await Bot.GetUpdatesAsync();
                }
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("Запрос был отменен из-за истечения времени ожидания.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }
    }

    private static async Task OnMessage(Message message, CancellationToken cancellationToken)
    {
        var isHandled = await SpamBlock.Task(message, cancellationToken, Bot);
        if (isHandled) return;
        await Ddhelper.Dbreq(message, cancellationToken);
        var commands = new Dictionary<string, Func<long, CancellationToken, Task>>
        {
            { "/start", HandleStartCommand },
            { "Расписание звонков", SendCallSchedule },
            { "Расписание пар", HandleScheduleCommand },
            { "Пары на завтра", (id, token) => WebService.Pari(Bot, token, message, 1, null) },
            { "Пары на сегодня", (id, token) => WebService.Pari(Bot, token, message, 0, null) },
            { "Капибара", (id, token) => SendRandomImage(id, "LinkCapybara.txt", token) },
            { "Шлёпа", (id, token) => SendRandomImage(id, "LinkBigRussianCat.txt", token) },
            { "Рофлс", HandleRoflsCommand },
            { "Назад", HandleStartCommand },
            { "Поддержка", SendSupportInfo },
            { "Сообщить о баге", SendBugReportInfo },
            { "Инфа", HandleInfoCommand },
            { "Расписание пар на определённый день", SendCalendar },
            {
                "halt", (id, token) =>
                {
                    if (id == 1079037911) Environment.Exit(0);
                    return Task.CompletedTask;
                }
            }
        };
        if (commands.TryGetValue(message.Text, out var command)) await command(message.Chat.Id, cancellationToken);
    }

    private static async Task HandleStartCommand(long chatId, CancellationToken cancellationToken)
    {
        var keyboard = new ReplyKeyboardMarkup
        {
            Keyboard = new[]
            {
                new[]
                {
                    new KeyboardButton("Расписание пар"),
                    new KeyboardButton("Инфа")
                },
                new[]
                {
                    new KeyboardButton("Расписание звонков"),
                    new KeyboardButton("Рофлс")
                }
            },
            ResizeKeyboard = true
        };
        await Bot.SendMessageAsync(chatId, "OK", replyMarkup: keyboard, cancellationToken: cancellationToken);
    }

    private static async Task SendCallSchedule(long chatId, CancellationToken cancellationToken)
    {
        await Bot.SendPhotoAsync(chatId,
            "https://sun9-77.userapi.com/impg/as1MA-6kTJiBgNaTzlJchVz9WIdRuTZt9uNJpQ/2kp1pa0vxL4.jpg?size=994x467&quality=96&sign=87102e4153f1c047a2012aa21487f1cb&type=album",
            cancellationToken: cancellationToken);
    }

    private static async Task HandleScheduleCommand(long chatId, CancellationToken cancellationToken)
    {
        var keyboard = new ReplyKeyboardMarkup
        {
            Keyboard = new[]
            {
                new[]
                {
                    new KeyboardButton("Пары на сегодня"),
                    new KeyboardButton("Пары на завтра"),
                    new KeyboardButton("Назад")
                },
                new[]
                {
                    new KeyboardButton("Расписание пар на определённый день")
                }
            },
            ResizeKeyboard = true
        };
        await Bot.SendMessageAsync(chatId, "OK", replyMarkup: keyboard, cancellationToken: cancellationToken);
    }

    private static async Task SendRandomImage(long chatId, string fileName, CancellationToken cancellationToken)
    {
        var x = new Random();
        var link = File.ReadLines($"Fold_data/{fileName}").ElementAtOrDefault(x.Next(1, 45));
        if (link != null)
            await Bot.SendPhotoAsync(chatId, link, caption: $"{x.Next(1, 45)}/45",
                cancellationToken: cancellationToken);
    }

    private static async Task HandleRoflsCommand(long chatId, CancellationToken cancellationToken)
    {
        var keyboard = new ReplyKeyboardMarkup
        {
            Keyboard = new[]
            {
                new[]
                {
                    new KeyboardButton("Шлёпа"),
                    new KeyboardButton("Капибара"),
                    new KeyboardButton("Назад")
                }
            },
            ResizeKeyboard = true
        };
        await Bot.SendMessageAsync(chatId, "несмешно", replyMarkup: keyboard, cancellationToken: cancellationToken);
    }

    private static async Task SendSupportInfo(long chatId, CancellationToken cancellationToken)
    {
        const string supportInfo = "Поддержать разработчика: \n\n" + "СберБанк: `5469 4100 1429 4908`\n" +
                                   "ВТБ: `2200 2460 4327 6560`\n\n";
        await Bot.SendMessageAsync(chatId, supportInfo, parseMode: ParseMode.MarkdownV2,
            cancellationToken: cancellationToken);
    }

    private static async Task SendBugReportInfo(long chatId, CancellationToken cancellationToken)
    {
        const string bugReportInfo = "Сообщить об ошибке:\nTG: @n0rd_kr1eg\n" + "VK: https://vk.com/n0rd_kr1eg\n\n" +
                                     "Время ответа: 5-15 минут";
        await Bot.SendMessageAsync(chatId, bugReportInfo, cancellationToken: cancellationToken);
    }

    private static async Task HandleInfoCommand(long chatId, CancellationToken cancellationToken)
    {
        var keyboard = new ReplyKeyboardMarkup
        {
            Keyboard = new[]
            {
                new[]
                {
                    new KeyboardButton("Поддержка"),
                    new KeyboardButton("Сообщить о баге"),
                    new KeyboardButton("Назад")
                }
            },
            ResizeKeyboard = true
        };
        await Bot.SendMessageAsync(chatId, "OK", replyMarkup: keyboard, cancellationToken: cancellationToken);
    }

    private static async Task SendCalendar(long chatId, CancellationToken cancellationToken)
    {
        var rm = new InlineKeyboardMarkup { InlineKeyboard = CreateCalendar(2023) };
        await Bot.SendMessageAsync(chatId, "ОК", parseMode: ParseMode.HTML, replyMarkup: rm,
            cancellationToken: cancellationToken);
    }

    private static async Task OnCallbackQuery(CallbackQuery query, BotClient bot, CancellationToken cancellationToken)
    {
        if (query.Data == null) return;
        var cbargs = query.Data.Split(' ');
        switch (cbargs[0])
        {
            case "month":
                var month = new Month((MonthName)Enum.Parse(typeof(MonthName), cbargs[2]), uint.Parse(cbargs[1]));
                var mkeyboard = new InlineKeyboardMarkup { InlineKeyboard = CreateCalendar(month) };
                Bot.EditMessageReplyMarkup<Message>(new EditMessageReplyMarkup
                {
                    ChatId = query.Message.Chat.Id,
                    MessageId = query.Message.MessageId,
                    ReplyMarkup = mkeyboard
                });
                break;
            case "year":
                var ykeyboard = new InlineKeyboardMarkup { InlineKeyboard = CreateCalendar(uint.Parse(cbargs[1])) };
                Bot.EditMessageReplyMarkup<Message>(new EditMessageReplyMarkup
                {
                    ChatId = query.Message.Chat.Id,
                    MessageId = query.Message.MessageId,
                    ReplyMarkup = ykeyboard
                });
                break;
            case "day":
                await WebService.Pari(bot, cancellationToken, query.Message, 0, cbargs);
                break;
        }
    }

    private static IEnumerable<InlineKeyboardButton[]> CreateCalendar(Month mon)
    {
        var calendar = new InlineKeyboardButton[mon.Weeks + 3][];
        var pos = 0;
        calendar[0] = new InlineKeyboardButton[1]
            { InlineButtonBuilder.SetCallbackData($"{mon.Name} {mon.Year}", $"year {mon.Year}") };
        var days = new[] { "Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Вс" };
        calendar[1] = new InlineKeyboardButton[7];
        for (var i = 0; i < 7; i++) calendar[1][i] = InlineButtonBuilder.SetCallbackData(days[i], $"{(DayName)i}");
        for (var i = 2; i < mon.Weeks + 2; i++)
        {
            calendar[i] = new InlineKeyboardButton[7];
            for (var j = 0; j < 7; j++)
                if (pos < mon.Days.Length)
                {
                    if ((int)mon.Days[pos].Name == j)
                    {
                        var day = mon.Days[pos];
                        calendar[i][j] = InlineButtonBuilder.SetCallbackData(
                            $"{day.Number}",
                            $"day {mon.Year} {(ushort)mon.Name} {day.Number}"
                        );
                        pos++;
                    }
                    else
                    {
                        calendar[i][j] = InlineButtonBuilder.SetCallbackData("*", "Empty day");
                    }
                }
                else
                {
                    calendar[i][j] = InlineButtonBuilder.SetCallbackData("*", "Empty day");
                }
        }

        calendar[^1] = new InlineKeyboardButton[2];
        var previousmonth = mon.Name == MonthName.January ? MonthName.December : mon.Name - 1;
        var nextmonth = mon.Name == MonthName.December ? MonthName.January : mon.Name + 1;
        var previousyear = previousmonth == MonthName.December ? mon.Year - 1 : mon.Year;
        var nextyear = nextmonth == MonthName.January ? mon.Year + 1 : mon.Year;
        calendar[^1][0] =
            InlineButtonBuilder.SetCallbackData($"{previousmonth}", $"month {previousyear} {(ushort)previousmonth}");
        calendar[^1][1] = InlineButtonBuilder.SetCallbackData($"{nextmonth}", $"month {nextyear} {(ushort)nextmonth}");

        return calendar;
    }

    private static IEnumerable<InlineKeyboardButton[]> CreateCalendar(uint year)
    {
        var keyboard = new InlineKeyboardButton[6][];
        keyboard[0] = new InlineKeyboardButton[1]
        {
            InlineButtonBuilder.SetCallbackData($"{year}", $"Year {year}")
        };
        for (int i = 1, n = 0; i < 5; i++)
        {
            keyboard[i] = new InlineKeyboardButton[3];
            for (var j = 0; j < 3; j++, n++)
            {
                var month = (MonthName)n;
                keyboard[i][j] = new InlineKeyboardButton
                {
                    Text = $"{month}",
                    CallbackData = $"month {year} {n}"
                };
            }
        }

        keyboard[5] = new InlineKeyboardButton[2]
        {
            InlineButtonBuilder.SetCallbackData($"{year - 1}", $"year {year - 1}"),
            InlineButtonBuilder.SetCallbackData($"{year + 1}", $"year {year + 1}")
        };
        return keyboard;
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var ex = (Exception)e.ExceptionObject;

        using var writer = new StreamWriter("ErrorLog.txt", true);
        writer.WriteLine("Message :" + ex.Message + "<br/>" + Environment.NewLine + "StackTrace :" + ex.StackTrace +
                         "" + Environment.NewLine + "Date :" + DateTime.Now.ToString(CultureInfo.InvariantCulture));
        writer.WriteLine(Environment.NewLine +
                         "-----------------------------------------------------------------------------" +
                         Environment.NewLine);
    }

    private static void OnTimedEvent(object source, ElapsedEventArgs e)
    {
        const string path = @"time.txt";
        using var sw = File.AppendText(path);
        sw.WriteLine("Сервис ОК, сейчас {0:HH:mm:ss.fff}", e.SignalTime);
    }
}

public enum DayName
{
    Monday,
    Tuesday,
    Wednesday,
    Thursday,
    Friday,
    Saturday,
    Sunday
}

public class Day
{
    public Day(DayName name, ushort number)
    {
        Name = name;
        Number = number;
    }

    public DayName Name { get; }
    public ushort Number { get; }
}

public enum MonthName
{
    January,
    February,
    March,
    April,
    May,
    June,
    July,
    August,
    September,
    October,
    November,
    December
}

public class Month
{
    public Month(MonthName monthName, uint year)
    {
        Name = monthName;
        Year = year;
        var leapyear = Year % 4 == 0;
        var days = Name switch
        {
            MonthName.February => leapyear ? 29 : 28,
            MonthName.April or MonthName.June or MonthName.September or MonthName.November => 30,
            _ => 31
        };
        Days = new Day[days];
        var firstday = year * 365 + (leapyear ? -1 : 0) + (year - year % 4) / 4 - (year - year % 400) / 400 + 3;
        var month = (int)monthName;
        firstday += month < 1 ? 0 : 31;
        firstday += month < 2 ? 0 : leapyear ? 29 : 28;
        firstday += month < 3 ? 0 : 31;
        firstday += month < 4 ? 0 : 30;
        firstday += month < 5 ? 0 : 31;
        firstday += month < 6 ? 0 : 30;
        firstday += month < 7 ? 0 : 31;
        firstday += month < 8 ? 0 : 31;
        firstday += month < 9 ? 0 : 30;
        firstday += month < 10 ? 0 : 31;
        firstday += month < 11 ? 0 : 30;
        firstday %= 7;
        for (var i = 0; i < Days.Length; i++) Days[i] = new Day((DayName)((i + firstday) % 7), (ushort)(i + 1));
    }

    public uint Year { get; }
    public MonthName Name { get; }
    public Day[] Days { get; }

    public ushort Weeks
    {
        get
        {
            var days = (int)Days[0].Name + Days.Length - 1;
            return (ushort)((days - days % 7) / 7 + (days % 7 > 0 ? 1 : 0));
        }
    }
}