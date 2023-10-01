using System.Collections.Concurrent;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableMethods.FormattingOptions;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;
using Telegram.BotAPI.UpdatingMessages;
namespace TG_7._0;
internal abstract class UserCh { 
    private static readonly HashSet<int> BannedUserIds = new() { 626421947, 917027444};
    private static readonly ConcurrentDictionary<long, Queue<DateTime>> MessageTimes = new();
    private const int MaxMessageCount = 5;
    private static readonly TimeSpan MessageTimeWindow = TimeSpan.FromSeconds(5);
    private static readonly ConcurrentDictionary<long, DateTime> UserBlockedUntil = new();
    public static async Task<bool> Task(Message message, CancellationToken cancellationToken, BotClient botClient)
    {
        if (message == null) return true;
        if (BannedUserIds.Contains((int)message.Chat.Id))
        {
            await botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId, cancellationToken: cancellationToken);
            await botClient.SendMessageAsync(message.Chat.Id, "Ваше сообщение было удалено, а вы заблокированы", cancellationToken: cancellationToken);
            Console.WriteLine("Сообщение от забаненного пользователя: " + message.Chat.Id);
            return true;
        }
        if (IsUserBlocked(message.From.Id))
        {
            Console.WriteLine($"Сообщение от замученого пользователя {message.From.Id} не обрабатывается: {message.Text}");
            return true;
        }
        if (IsSpamming(message))
        {
            Console.WriteLine($"Сообщение от пользователя {message.From.Id} отклонено из-за спама: {message.Text}");
            BlockUser(message.From.Id);
            await botClient.SendPhotoAsync(message.Chat.Id, "https://steamuserimages-a.akamaihd.net/ugc/956346433289890009/369C4E7EA8C212D161EDF7840539C0F3F8FFE505/?imw=5000&imh=5000&ima=fit&impolicy=Letterbox&imcolor=%23000000&letterbox=false", cancellationToken: cancellationToken);
            await BlockAndDeleteMessageAsync(message, botClient);
            return true;
        }
        return false;
    }
    private static bool IsSpamming(Message message)
    {
        if (!MessageTimes.TryGetValue(message.From!.Id, out var messageQueue))
        {
            messageQueue = new Queue<DateTime>();
            MessageTimes[message.From.Id] = messageQueue;
        }
        while (messageQueue.Count > 0 && DateTime.Now - messageQueue.Peek() > MessageTimeWindow)
        {
            messageQueue.Dequeue();
        }
        messageQueue.Enqueue(DateTime.Now);
        return messageQueue.Count > MaxMessageCount;
    }
    private static bool IsUserBlocked(long userId)
    {
        if (UserBlockedUntil.TryGetValue(userId, out var blockUntil))
        {
            return DateTime.Now <= blockUntil;
        }
        return false;
    }
    private static void BlockUser(long userId)
    {
        var blockUntil = DateTime.Now.Add(TimeSpan.FromMinutes(1));
        UserBlockedUntil[userId] = blockUntil;
    }
    private static async Task BlockAndDeleteMessageAsync(Message message, BotClient botClient)
    {
        BlockUser(message.From.Id);
        await botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId);
        await botClient.SendMessageAsync(message.Chat.Id, "Ваше сообщение было удалено, и вы временно заблокированы");
    }

}