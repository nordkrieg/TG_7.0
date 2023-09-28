using Telegram.Bot.Polling;
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

    private static void Main()
    {
        Console.WriteLine("Ужики я жив....");
        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        var receiverOptions = new ReceiverOptions();
        var updates = Bot.GetUpdates();
        while (true)
        {
            if (updates.Length > 0)
            {
                foreach (var update in updates)
                {
                    switch (update.Type)
                    {
                        case UpdateType.Message: OnMessage(update.Message, Bot, cancellationToken);
                            break;
                        case UpdateType.CallbackQuery: OnCallbackQuery(update.CallbackQuery);
                            break;
                    }
                }
                updates = Bot.GetUpdates(offset: updates.Max(u => u.UpdateId) + 1);
            }
            else
            {
                updates = Bot.GetUpdates();
            }
        }
    }
    private static async Task OnMessage(Message message, BotClient bot, CancellationToken cancellationToken)
    {
        //var aba = await UserCh.Task(message, bot, cancellationToken);
        //if (aba) return;
        var moscowTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time"));
        Console.WriteLine($"User: {message.Chat.Username}" + "\n" + $"Name: {message.Chat.FirstName}" + "\n" +
                          $"Surnameame: {message.Chat.LastName}" + "\n" + $"ID Chat: {message.Chat.Id}" + "\n" +
                          $"Time: {moscowTime}" + "\n" + $"Text: {message.Text}" + "\n");
        switch (message.Text)
        {
            case "/start":
                {
                    var keyboard = new ReplyKeyboardMarkup
                    {
                        Keyboard = new[]
                        {
                            new[]{
                                new KeyboardButton("Расписание пар"),
                                new KeyboardButton("Инфа")
                            },
                            new[]{
                                new KeyboardButton("Расписание звонков"),
                                new KeyboardButton("Капибара"),
                                new KeyboardButton("Календарь")
                            }
                        },
                        ResizeKeyboard = true
                    };
                    await bot.SendMessageAsync(message.Chat.Id, "OK", replyMarkup: keyboard, cancellationToken: cancellationToken);
                    break;
                }
            case "Расписание звонков":
                await bot.SendPhotoAsync(message.Chat.Id, "https://sun9-77.userapi.com/impg/as1MA-6kTJiBgNaTzlJchVz9WIdRuTZt9uNJpQ/2kp1pa0vxL4.jpg?size=994x467&quality=96&sign=87102e4153f1c047a2012aa21487f1cb&type=album", cancellationToken: cancellationToken);
                return;
            case "Расписание пар":
                {
                    var keyboard = new ReplyKeyboardMarkup
                    {
                        Keyboard = new[]
                        {
                            new[]{
                                new KeyboardButton("Пары на сегодня"),
                                new KeyboardButton("Пары на завтра"),
                                new KeyboardButton("Назад"),
                            },
                        },
                        ResizeKeyboard = true
                    };
                    await bot.SendMessageAsync(message.Chat.Id, "OK", replyMarkup: keyboard, cancellationToken: cancellationToken);
                    break;
                }
            case "Пары на завтра":
                //await Pari(bot, cancellationToken, message, 1);
                break;
            case "Пары на сегодня":
                //await Pari(bot, cancellationToken, message, 0);
                break;
            case "Капибара":
                {
                    var x = new Random();
                    var rand = x.Next(1, 45);
                    var link = File.ReadLines("../../../Fold_data/LinkCapybara.txt").ElementAtOrDefault(rand);
                    if (link != null)
                        await bot.SendPhotoAsync(message.Chat.Id, link, caption: $"{rand}/45", cancellationToken: cancellationToken);
                    break;
                }
            case "Назад":
                {
                    var keyboard = new ReplyKeyboardMarkup
                    {
                        Keyboard = new[]
                        {
                            new[]{
                                new KeyboardButton("Расписание пар"),
                                new KeyboardButton("Инфа")
                            },
                            new[]{
                                new KeyboardButton("Расписание звонков"),
                                new KeyboardButton("Капибара")
                            }
                        },
                        ResizeKeyboard = true
                    };
                    await bot.SendMessageAsync(message.Chat.Id, "OK", replyMarkup: keyboard, cancellationToken: cancellationToken);
                    break;
                }
            case "Поддержка":
                await bot.SendMessageAsync(message.Chat.Id, "Поддержать разработчика: \n\n" + "СберБанк: `5469 4100 1429 4908`\n" + "ВТБ: `2200 2460 4327 6560`\n\n", parseMode: ParseMode.MarkdownV2, cancellationToken: cancellationToken);
                break;
            case "Сообщить о баге":
                await bot.SendMessageAsync(message.Chat.Id, "Сообщить об ошибке:\nTG: @n0rd_kr1eg\n" + "VK: https://vk.com/n0rd_kr1eg" + "\n\nВремя ответа: 5-15 минут", parseMode: ParseMode.MarkdownV2, cancellationToken: cancellationToken);
                break;
            case "Инфа":
                {
                    var keyboard = new ReplyKeyboardMarkup
                    {
                        Keyboard = new[]
                        {
                            new[]{
                                new KeyboardButton("Поддержка"),
                                new KeyboardButton("Сообщить о баге"),
                                new KeyboardButton("Назад"),
                            },
                        },
                        ResizeKeyboard = true
                    };
                    await bot.SendMessageAsync(message.Chat.Id, "OK", replyMarkup: keyboard, cancellationToken: cancellationToken);
                    break;
                }
            case "Календарь":
                var rm = new InlineKeyboardMarkup
                {
                    InlineKeyboard = CreateCalendar(2023)
                };
                await bot.SendMessageAsync(message.Chat.Id, "🗓 <b>Telegram Bot Calendar</b> 🗓", parseMode: ParseMode.HTML, replyMarkup: rm, cancellationToken: cancellationToken);
                break;
        }
    }
    private static void OnCallbackQuery(CallbackQuery query)
    {
        if (query.Data == null) return;
        var cbargs = query.Data.Split(' ');
        switch (cbargs[0])
        { 
            case "month":
                var month = new Month((MonthName)Enum.Parse(typeof(MonthName), cbargs[2]), uint.Parse(cbargs[1]));
                var mkeyboard = new InlineKeyboardMarkup
                {
                    InlineKeyboard = CreateCalendar(month)
                };
                Bot.EditMessageReplyMarkup<Message>(new EditMessageReplyMarkup
                {
                    ChatId = query.Message.Chat.Id,
                    MessageId = query.Message.MessageId,
                    ReplyMarkup = mkeyboard
                });
                break;
            case "year":
                var ykeyboard = new InlineKeyboardMarkup
                {
                    InlineKeyboard = CreateCalendar(uint.Parse(cbargs[1]))
                };
                Bot.EditMessageReplyMarkup<Message>(new EditMessageReplyMarkup
                {
                    ChatId = query.Message.Chat.Id,
                    MessageId = query.Message.MessageId,
                    ReplyMarkup = ykeyboard
                });
                break;
            case "day":
                var selectedDay = cbargs[3];
                Console.WriteLine($"Selected Day: {selectedDay}");
                foreach (string строка in cbargs)
                {
                    Console.WriteLine(строка);
                }
                break;
        }
    }
    private static IEnumerable<InlineKeyboardButton[]> CreateCalendar(Month mon)
    {
        var calendar = new InlineKeyboardButton[mon.Weeks + 3][];
        var pos = 0;
        calendar[0] = new InlineKeyboardButton[1]
        {
            InlineKeyboardButton.SetCallbackData($"{mon.Name} {mon.Year}", $"year {mon.Year}")
        };
        var days = new[] { "Mo", "Tu", "We", "Th", "Fr", "Sa", "Su" };
        calendar[1] = new InlineKeyboardButton[7];
        for (int i = 0; i < 7; i++)
        {
            calendar[1][i] = InlineKeyboardButton.SetCallbackData(days[i], $"{((DayName)i)}");
        }
        for (int i = 2; i < mon.Weeks + 2; i++)
        {
            calendar[i] = new InlineKeyboardButton[7];
            for (int j = 0; j < 7; j++)
            {
                if (pos < mon.Days.Length)
                {
                    if ((int)mon.Days[pos].Name == j)
                    {
                        var day = mon.Days[pos];
                        calendar[i][j] = InlineKeyboardButton.SetCallbackData(
                            $"{day.Number}",
                            $"day {mon.Year} {(ushort)mon.Name} {day.Number}"
                        );
                            //calendar[i][j] = InlineKeyboardButton.SetCallbackData($"{mon.Days[pos].Number}", $"{mon.Days[pos].Name}, {mon.Name} {mon.Days[pos].Number}");
                        pos++;
                    }
                    else
                    {
                        calendar[i][j] = InlineKeyboardButton.SetCallbackData("*", "Empty day");
                    }
                }
                else
                {
                    calendar[i][j] = InlineKeyboardButton.SetCallbackData("*", "Empty day");
                }
            }
        }
        calendar[^1] = new InlineKeyboardButton[2];
        var previousmonth = mon.Name == MonthName.January ? MonthName.December : mon.Name - 1;
        var nextmonth = mon.Name == MonthName.December ? MonthName.January : mon.Name + 1;
        var previousyear = previousmonth == MonthName.December ? mon.Year - 1 : mon.Year;
        var nextyear = nextmonth == MonthName.January ? mon.Year + 1 : mon.Year;
        calendar[^1][0] = InlineKeyboardButton.SetCallbackData($"{previousmonth}", $"month {previousyear} {((ushort)previousmonth)}");
        calendar[^1][1] = InlineKeyboardButton.SetCallbackData($"{nextmonth}", $"month {nextyear} {((ushort)nextmonth)}");

        return calendar;
    }
    private static InlineKeyboardButton[][] CreateCalendar(uint year)
    {
        var keyboard = new InlineKeyboardButton[6][];
        keyboard[0] = new InlineKeyboardButton[1]{
            InlineKeyboardButton.SetCallbackData($"{year}", $"Year {year}")
        };
        for (int i = 1, n = 0; i < 5; i++)
        {
            keyboard[i] = new InlineKeyboardButton[3];
            for (int j = 0; j < 3; j++, n++)
            {
                var month = (MonthName)n;
                keyboard[i][j] = new InlineKeyboardButton
                {
                    Text = $"{month}",
                    CallbackData = $"month {year} {n}"
                };
            }
        }
        keyboard[5] = new InlineKeyboardButton[2]{
            InlineKeyboardButton.SetCallbackData($"{year - 1}",$"year {year - 1}"),
            InlineKeyboardButton.SetCallbackData($"{year + 1}",$"year {year + 1}")
        };
        return keyboard;
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
        Name = name; Number = number;
    }
    public DayName Name { get; set; }
    public ushort Number { get; set; }
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
        var days = Name == MonthName.February ? (leapyear ? 29 : 28) : (Name == MonthName.April || Name == MonthName.June || Name == MonthName.September || Name == MonthName.November ? 30 : 31);
        Days = new Day[days];
        var firstday = year * 365 + (leapyear ? -1 : 0) + (((year - (year % 4)) / 4)) - (((year - (year % 400)) / 400)) + 3;
        var month = (int)monthName;
        firstday += month < 1 ? 0 : 31;
        firstday += month < 2 ? 0 : (leapyear ? 29 : 28);
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
        for (int i = 0; i < Days.Length; i++)
            Days[i] = new Day((DayName)((i + firstday) % 7), (ushort)(i + 1));
    }
    public uint Year { get; set; }
    public MonthName Name { get; set; }
    public Day[] Days { get; set; }
    public ushort Weeks
    {
        get
        {
            var days = (int)Days[0].Name + Days.Length - 1;
            return (ushort)(((days - (days % 7)) / 7) + (days % 7 > 0 ? 1 : 0));
        }
    }
}