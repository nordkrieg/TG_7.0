using PdfLibCore;
using PdfLibCore.Enums;
using SixLabors.ImageSharp.Formats.Jpeg;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.AvailableMethods;
using File = System.IO.File;
using Image = SixLabors.ImageSharp.Image;
using InputFile = Telegram.BotAPI.AvailableTypes.InputFile;
namespace TG_7._0;
public abstract class WebService
{
    private static async Task<bool> CheckUrl(string url)
    {
        using var client = new HttpClient();
        try
        {
            var response = await client.GetAsync(url);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException)
        {
            return false;
        }
    }
    private static async Task DownLoad(string url, string path, DateTime date) {
        var day = date.DayOfYear.ToString();
        using var client = new HttpClient();
        var month = date.Month.ToString();
        if (Convert.ToInt32(month) < 10) _ = "0" + month;
        if (day[0] == '0')  _ = day.TrimStart('0');
        if (await CheckUrl(url)) {
            var fileBytes = await client.GetByteArrayAsync(url);
            await File.WriteAllBytesAsync(path, fileBytes); }
    }
    public static async Task Pari(BotClient botClient, CancellationToken cancellationToken, Message message, int x, string[] days)
    {
        var moscowTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time"));
        var targetDate = days != null ? new DateTime(int.Parse(days[1]), int.Parse(days[2]) + 1, int.Parse(days[3])) : moscowTime.AddDays(x);
        var pt = Path.Combine("../../../Fold_data/sch_fold", targetDate.ToString("dd.MM.yyyy") + "/");
        while (true)
        {
            if (Directory.Exists(pt))
            {
                await SendSchedule(botClient, cancellationToken, message, pt, targetDate);
                break;
            }
            if (await TryDownloadAndConvertSchedule(pt, targetDate)) continue;
            await botClient.SendMessageAsync(message.Chat.Id, "Расписания на " + targetDate.ToString("dd.MM.yyyy") + " нет", cancellationToken: cancellationToken);
            break;
        }
    }
    private static async Task<bool> TryDownloadAndConvertSchedule(string pt, DateTime targetDate)
    {
        var url = $"https://mkeiit.ru/wp-content/uploads/{targetDate:yyyy/MM/dd}.pdf";
        if (!await CheckUrl(url)) return false;
        Directory.CreateDirectory(pt);
        await DownLoad(url, $"{pt}{targetDate:dd.MM.yyyy}.pdf", DateTime.Now);
        await ConvertPdFtoHojas(pt, targetDate.ToString("dd"), targetDate.ToString("MM"), targetDate.ToString("yyyy"));
        return true;
    }
    private static async Task SendSchedule(BotClient botClient, CancellationToken cancellationToken, Message message, string pt, DateTime targetDate)
    {
        if (File.Exists($"{pt}{targetDate:dd.MM.yyyy}-1.jpg")) {
            var file1 = new InputFile(await File.ReadAllBytesAsync($"{pt}{targetDate:dd.MM.yyyy}-0.jpg", cancellationToken), "odin.jpg");
            var file2 = new InputFile(await File.ReadAllBytesAsync($"{pt}{targetDate:dd.MM.yyyy}-1.jpg", cancellationToken), "dva.jpg");
            var files = new[] {
            new AttachedFile("odin.jpg", file1),
            new AttachedFile("dva.jpg", file2) };
            await botClient.SendMediaGroupAsync(message.Chat.Id, new[] {
            new InputMediaPhoto("attach://odin.jpg"),
            new InputMediaPhoto("attach://dva.jpg") }, attachedFiles: files, cancellationToken: cancellationToken);
        }
        else await botClient.SendPhotoAsync(message.Chat.Id, new InputFile(await File.ReadAllBytesAsync($"{pt}{targetDate:dd.MM.yyyy}-0.jpg", cancellationToken), $"{pt}{targetDate:dd.MM.yyyy}-0.jpg"), cancellationToken: cancellationToken);
    }
    private static async Task ConvertPdFtoHojas(string path, string day, string month, string year)
    {
        using var pdfDocument = new PdfDocument(File.Open(path + day + "." + month + "." + year + ".pdf", FileMode.Open));
        using var pagesi = pdfDocument;
        for (var i = 0; i < pagesi.Pages.Count; i++)
        {
            using var bitmap = new PdfiumBitmap((int)pagesi.Pages[i].Size.Width, (int)pagesi.Pages[i].Size.Height, false);
            pagesi.Pages[i].Render(bitmap, PageOrientations.Normal, RenderingFlags.LcdText);
            byte[] byteArray;
            using (var memoryStream = new MemoryStream()) {
                await bitmap.AsBmpStream(1 , 1).CopyToAsync(memoryStream);
                byteArray = memoryStream.ToArray(); }
            await File.WriteAllBytesAsync(path + day + "." + month + "." + year + $"-{i}.png", byteArray);
            var image1 = await Image.LoadAsync($"{path}{day}.{month}.{year}-{i}.png");
            var encoder = new JpegEncoder { Quality = 100 };
            await image1.SaveAsync($"{path}{day}.{month}.{year}-{i}.jpg", encoder);
            File.Delete(path + day + "." + month + "." + year + $"-{i}.png");
        }
        File.Delete(path + day + "." + month + "." + year + ".pdf");
    }
}