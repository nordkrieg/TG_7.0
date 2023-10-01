using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableMethods.FormattingOptions;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;
using Telegram.BotAPI.UpdatingMessages;
using BotClient = Telegram.BotAPI.BotClient;
using File = System.IO.File;
namespace TG_7._0;
internal abstract class Program
{
    private static readonly BotClient Bot = new("6348440231:AAFO28UNHkVkNAw6JQ5kKg8_kdeo-7MjCsE");
    private static async Task Main()
    {
        Console.WriteLine("Ужики я жив....");
        var cts = new CancellationTokenSource();
        while (true)
        {
            var updates = await Bot.GetUpdatesAsync().ConfigureAwait(true);
            if (updates.Length > 0)
            {
                foreach (var update in updates)
                {
                    switch (update.Type)
                    {
                        case UpdateType.Message:
                            await OnMessage(update.Message, cts.Token).ConfigureAwait(true);
                            break;
                        case UpdateType.CallbackQuery:
                            await OnCallbackQuery(update.CallbackQuery, cts.Token).ConfigureAwait(true);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                await Bot.GetUpdatesAsync(offset: updates.Max(u => u.UpdateId) + 1, cancellationToken: cts.Token).ConfigureAwait(true);
            }
            else await Bot.GetUpdatesAsync().ConfigureAwait(true);
        }
    }
    private static async Task OnMessage(Message message, CancellationToken cancellationToken)
    {
        if (await UserCh.Task(message, cancellationToken, Bot).ConfigureAwait(true)) return;
        var moscowTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
            TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time"));
        Console.WriteLine($"User: {message.Chat.Username}\n" + $"Name: {message.Chat.FirstName}\n" +
                          $"Surnameame: {message.Chat.LastName}\n" +
                          $"ID Chat: {message.Chat.Id}\n" + $"Time: {moscowTime}\n" + $"Text: {message.Text}\n");
        switch (message.Text)
        {
            case "/start":
                await HandleStartCommand(message.Chat.Id, cancellationToken).ConfigureAwait(true);
                break;
            case "Расписание звонков":
                await SendCallSchedule(message.Chat.Id, cancellationToken).ConfigureAwait(true);
                break;
            case "Расписание пар":
                await HandleScheduleCommand(message.Chat.Id, cancellationToken).ConfigureAwait(true);
                break;
            case "Пары на завтра":
                await OthersMethods.Pari(Bot, cancellationToken, message, 1, null).ConfigureAwait(true);
                break;
            case "Пары на сегодня":
                await OthersMethods.Pari(Bot, cancellationToken, message, 0, null).ConfigureAwait(true);
                break;
            case "Капибара":
                await SendRandomImage(message.Chat.Id, "LinkCapybara.txt", cancellationToken).ConfigureAwait(true);
                break;
            case "Шлёпа":
                await SendRandomImage(message.Chat.Id, "LinkBigRussianCat.txt", cancellationToken).ConfigureAwait(true);
                break;
            case "Рофлс":
                await HandleRoflsCommand(message.Chat.Id, cancellationToken).ConfigureAwait(true);
                break;
            case "Назад":
                await HandleStartCommand(message.Chat.Id, cancellationToken).ConfigureAwait(true);
                break;
            case "Поддержка":
                await SendSupportInfo(message.Chat.Id, cancellationToken).ConfigureAwait(true);
                break;
            case "Сообщить о баге":
                await SendBugReportInfo(message.Chat.Id, cancellationToken).ConfigureAwait(true);
                break;
            case "Инфа":
                await HandleInfoCommand(message.Chat.Id, cancellationToken).ConfigureAwait(true);
                break;
            case "Расписание пар на определённый день":
                await SendCalendar(message.Chat.Id, cancellationToken).ConfigureAwait(true);
                break;
        }
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
        await Bot.SendMessageAsync(chatId, "OK", replyMarkup: keyboard, cancellationToken: cancellationToken).ConfigureAwait(true);
    }
    private static async Task SendCallSchedule(long chatId, CancellationToken cancellationToken)
    {
        await Bot.SendPhotoAsync(chatId,
            "https://sun9-77.userapi.com/impg/as1MA-6kTJiBgNaTzlJchVz9WIdRuTZt9uNJpQ/2kp1pa0vxL4.jpg?size=994x467&quality=96&sign=87102e4153f1c047a2012aa21487f1cb&type=album",
            cancellationToken: cancellationToken).ConfigureAwait(true);
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
        await Bot.SendMessageAsync(chatId, "OK", replyMarkup: keyboard, cancellationToken: cancellationToken).ConfigureAwait(true);
    }
    private static async Task SendRandomImage(long chatId, string fileName, CancellationToken cancellationToken)
    {
        var x = new Random();
        var rand = x.Next(1, 45);
        var link = File.ReadLines($"../../../Fold_data/{fileName}").ElementAtOrDefault(rand);
        if (link != null)
            await Bot.SendPhotoAsync(chatId, link, caption: $"{rand}/45", cancellationToken: cancellationToken).ConfigureAwait(true);
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
        await Bot.SendMessageAsync(chatId, "несмешно", replyMarkup: keyboard, cancellationToken: cancellationToken).ConfigureAwait(true);
    }
    private static async Task SendSupportInfo(long chatId, CancellationToken cancellationToken)
    {
        await Bot.SendMessageAsync(chatId, "Поддержать разработчика: \n\n" + "СберБанк: `5469 4100 1429 4908`\n" + "ВТБ: `2200 2460 4327 6560`\n\n", parseMode: ParseMode.MarkdownV2,
            cancellationToken: cancellationToken).ConfigureAwait(true);
    }
    private static async Task SendBugReportInfo(long chatId, CancellationToken cancellationToken)
    {
        const string bugReportInfo = "Сообщить об ошибке:\nTG: @n0rd_kr1eg\n" + "VK: https://vk.com/n0rd_kr1eg\n\n" +
                                     "Время ответа: 5-15 минут";
        await Bot.SendMessageAsync(chatId, bugReportInfo, cancellationToken: cancellationToken).ConfigureAwait(true);
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
        await Bot.SendMessageAsync(chatId, "OK", replyMarkup: keyboard, cancellationToken: cancellationToken).ConfigureAwait(true);
    }
    private static async Task SendCalendar(long chatId, CancellationToken cancellationToken)
    {
        var calendarMarkup = CreateCalendarMarkup(2023);
        await Bot.SendMessageAsync(chatId, "🗓 <b>Telegram Bot Calendar</b> 🗓", parseMode: ParseMode.HTML, replyMarkup: calendarMarkup, cancellationToken: cancellationToken).ConfigureAwait(true);
    }
    private static async Task OnCallbackQuery(CallbackQuery query, CancellationToken cancellationToken)
    {
        if (query.Data == null) return;
        var cbargs = query.Data.Split(' ');
        switch (cbargs[0])
        {
            case "month":
                var month = new Month((MonthName)Enum.Parse(typeof(MonthName), cbargs[2]), uint.Parse(cbargs[1]));
                var monthCalendarMarkup = CreateCalendarMarkup(month);
                await EditMessageReplyMarkup(query, monthCalendarMarkup).ConfigureAwait(true);
                break;
            case "year":
                var year = uint.Parse(cbargs[1]);
                var yearCalendarMarkup = CreateCalendarMarkup(year);
                await EditMessageReplyMarkup(query, yearCalendarMarkup).ConfigureAwait(true);
                break;
            case "day":
                await OthersMethods.Pari(Bot, cancellationToken, query.Message, 0, cbargs).ConfigureAwait(false);
                break;
        }
    }
    private static Task EditMessageReplyMarkup(CallbackQuery query, InlineKeyboardMarkup keyboardMarkup)
    {
        var editMessageOptions = new EditMessageReplyMarkup
        {
            ChatId = query.Message!.Chat.Id,
            MessageId = query.Message.MessageId,
            ReplyMarkup = keyboardMarkup
        };
        Bot.EditMessageReplyMarkup<Message>(editMessageOptions);
        return Task.CompletedTask;
    }
    private static InlineKeyboardMarkup CreateCalendarMarkup(uint year)
    {
        var calendar = new InlineKeyboardButton[6][];
        calendar[0] = new[] { InlineButtonBuilder.SetCallbackData($"{year}", $"Year {year}") };

        for (int i = 1, n = 0; i < 5; i++)
        {
            calendar[i] = new InlineKeyboardButton[3];
            for (var j = 0; j < 3; j++, n++)
            {
                var month = (MonthName)n;
                calendar[i][j] = new InlineKeyboardButton
                {
                    Text = $"{month}",
                    CallbackData = $"month {year} {n}"
                };
            }
        }

        calendar[5] = new[]
        {
            InlineButtonBuilder.SetCallbackData($"{year - 1}", $"year {year - 1}"),
            InlineButtonBuilder.SetCallbackData($"{year + 1}", $"year {year + 1}")
        };
        return new InlineKeyboardMarkup { InlineKeyboard = calendar };
    }
    private static InlineKeyboardMarkup CreateCalendarMarkup(Month month)
    {
        var calendar = new InlineKeyboardButton[month.Weeks + 3][];
        var pos = 0;
        calendar[0] = new[]
        {
            InlineButtonBuilder.SetCallbackData($"{month.Name} {month.Year}", $"year {month.Year}")
        };
        var days = new[] { "Mo", "Tu", "We", "Th", "Fr", "Sa", "Su" };
        calendar[1] = new InlineKeyboardButton[7];
        for (var i = 0; i < 7; i++)
        {
            calendar[1][i] = InlineButtonBuilder.SetCallbackData(days[i], $"{(DayName)i}");
        }

        for (var i = 2; i < month.Weeks + 2; i++)
        {
            calendar[i] = new InlineKeyboardButton[7];
            for (var j = 0; j < 7; j++)
            {
                if (pos < month.Days.Length)
                {
                    if ((int)month.Days[pos].Name == j)
                    {
                        var day = month.Days[pos];
                        calendar[i][j] = InlineButtonBuilder.SetCallbackData(
                            $"{day.Number}",
                            $"day {month.Year} {(ushort)month.Name} {day.Number}"
                        );
                        pos++;
                    }
                    else calendar[i][j] = InlineButtonBuilder.SetCallbackData("*", "Empty day");
                }
                else calendar[i][j] = InlineButtonBuilder.SetCallbackData("*", "Empty day");
            }
        }

        calendar[^1] = new InlineKeyboardButton[2];
        var previousmonth = month.Name == MonthName.January ? MonthName.December : month.Name - 1;
        var nextmonth = month.Name == MonthName.December ? MonthName.January : month.Name + 1;
        var previousyear = previousmonth == MonthName.December ? month.Year - 1 : month.Year;
        var nextyear = nextmonth == MonthName.January ? month.Year + 1 : month.Year;
        calendar[^1][0] =
            InlineButtonBuilder.SetCallbackData($"{previousmonth}", $"month {previousyear} {(ushort)previousmonth}");
        calendar[^1][1] =
            InlineButtonBuilder.SetCallbackData($"{nextmonth}", $"month {nextyear} {(ushort)nextmonth}");
        return new InlineKeyboardMarkup { InlineKeyboard = calendar };
    }
    private enum DayName
    {
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
        Sunday
    }
    private class Day
    {
        public Day(DayName name, ushort number)
        {
            Name = name;
            Number = number;
        }

        public DayName Name { get; }
        public ushort Number { get; }
    }
    private enum MonthName
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
    private class Month
    {
        public Month(MonthName monthName, uint year)
        {
            Name = monthName;
            Year = year;
            var leapyear = Year % 4 == 0;
            var days = Name switch
            {
                MonthName.February => leapyear ? 29 : 28,
                MonthName.April => 30,
                MonthName.June => 30,
                MonthName.September => 30,
                MonthName.November => 30,
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
            for (var i = 0; i < Days.Length; i++)
                Days[i] = new Day((DayName)((i + firstday) % 7), (ushort)(i + 1));
        }
        public uint Year { get; }
        public MonthName Name { get; }
        public Day[] Days { get; }
        public ushort Weeks
        {
            get
            {
                var days = (int)Days[0].Name + Days.Length - 1;
                return (ushort)(days -days % 7 / 7 + (days % 7 > 0 ? 1 : 0));
            }
        }
    }
}