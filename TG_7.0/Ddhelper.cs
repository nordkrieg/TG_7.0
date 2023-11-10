using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Telegram.BotAPI.AvailableTypes;
namespace TG_7._0;
internal abstract class Ddhelper
{
    public static async Task Dbreq(Message message, CancellationToken cancellationToken)
    {
        var moscowTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time"));
        await using var db = new ApplicationContext();
        var existingRequestTask = db.Requests.FirstOrDefaultAsync(r => r.IDChat == message.Chat.Id, cancellationToken: cancellationToken);
        if (existingRequestTask != null)
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
                IDChat = message.Chat.Id,
                Login = message.Chat.Username,
                Name = message.Chat.FirstName,
                Surnameame = message.Chat.LastName,
                ReqDateTime = moscowTime,
                Req = message.Text
            };
            db.Requests.Add(newRequest);
        }
        await db.SaveChangesAsync(cancellationToken); }
    public sealed class ApplicationContext : DbContext {
        public DbSet<Request> Requests => Set<Request>();
        public ApplicationContext() => Database.EnsureCreated();
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { optionsBuilder.UseSqlite("Data Source=../../../Fold_data/masterdb.db"); }
    }
    public class Request
    {
        [Key]
        public long IDChat { get; set; }
        public string? Login { get; set; }
        public string? Name { get; set; }
        public string? Surnameame { get; set; }
        public DateTime? ReqDateTime { get; set; }
        public string? Req { get; set; }
    }
}
