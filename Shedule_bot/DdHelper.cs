#nullable enable
#pragma warning disable CS8602 // Dereference of a possibly null reference.
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Telegram.BotAPI.AvailableTypes;

namespace Schedule;

internal abstract class Ddhelper
{
    public static async Task Dbreq(Message message, CancellationToken cancellationToken)
    {
        var moscowTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
            TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time"));
        await using var db = new ApplicationContext();
        var existingRequestTask = db.Requests.FirstOrDefaultAsync(r => r.IdChat == message.Chat.Id, cancellationToken);
        if (existingRequestTask.Result != null)
        {
            var existingRequest = await existingRequestTask;
            existingRequest.Login = message.Chat.Username;
            existingRequest.Name = message.Chat.FirstName;
            existingRequest.Surnameame = message.Chat.LastName;
            existingRequest.ReqDateTime = moscowTime;
            existingRequest.Req = message.Text;
        }
        else
        {
            var newRequest = new Request
            {
                IdChat = message.Chat.Id,
                Login = message.Chat.Username,
                Name = message.Chat.FirstName,
                Surnameame = message.Chat.LastName,
                ReqDateTime = moscowTime,
                Req = message.Text
            };
            db.Requests.Add(newRequest);
        }
        await db.SaveChangesAsync(cancellationToken);
    }
    public sealed class ApplicationContext : DbContext
    {
        public ApplicationContext()
        {
            Database.EnsureCreated();
        }

        public DbSet<Request> Requests => Set<Request>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Fold_data/masterdb.db");
        }
    }
    public class Request
    {
        [Key] public long IdChat { get; set; }

        public string? Login { get; set; }
        public string? Name { get; set; }
        public string? Surnameame { get; set; }
        public DateTime? ReqDateTime { get; set; }
        public string? Req { get; set; }
    }
}