using PdfLibCore;
using PdfLibCore.Enums;
using SixLabors.ImageSharp.Formats.Jpeg;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.AvailableMethods;
using File = System.IO.File;
using Image = SixLabors.ImageSharp.Image;
using InputFile = Telegram.BotAPI.AvailableTypes.InputFile;

namespace TG_7._0;
public abstract class OthersMethods
{
    private static async Task<bool> CheckUrl(string url)
    {
        using var client = new HttpClient();
        try
        {
            var response = await client.GetAsync(url).ConfigureAwait(true);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException)
        {
            return false;
        }
    }
    private static async Task DownLoad(string url, string path, DateTime date)
    {
        var day = date.DayOfYear.ToString();
        using var client = new HttpClient();
        var month = date.Month.ToString();
        if (Convert.ToInt32(month) < 10) _ = "0" + month;
        if (day[0] == '0')  _ = day.TrimStart('0');
        if (await CheckUrl(url).ConfigureAwait(true))
        {
            var fileBytes = await client.GetByteArrayAsync(url).ConfigureAwait(true);
            await File.WriteAllBytesAsync(path, fileBytes).ConfigureAwait(true);
        }
    }

    public static async Task Pari(BotClient botClient, CancellationToken cancellationToken, Message message, int x, string[] days)
    {
        string day, month, year;
        var moscowTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time"));
        if (days != null)
        {
            day = days[3];
            month = Convert.ToString(Convert.ToInt32(days[2]) + 1);
            if (month.Length != 2) month = "0" + month;
            if (day[0] == '0') day = day.TrimStart('0');
            year = days[1];
        }
        else
        {
            day = Convert.ToString(Convert.ToInt32(moscowTime.Day.ToString()) + x);
            month = Convert.ToString(Convert.ToInt32(moscowTime.Month.ToString()));
            year = Convert.ToString(Convert.ToInt32(moscowTime.Year.ToString()));
            if (month.Length != 2) month = "0" + month;
            if (day[0] == '0') day = day.TrimStart('0');
        }
        var pt = "../../../Fold_data/sch_fold/" + day + "." + month + "." + year + "/";
        var temnMonth = month switch
        {
            "10" => "09",
            "11" => "10",
            _ => month
        };
        while (true)
        {
            if (Directory.Exists(pt))
            {
                if (File.Exists($"{pt}{day}.{month}.{year}-1.jpg"))
                {
                    var fsArr = new[] { new FileStream($"{pt}{day}.{month}.{year}-0.jpg", FileMode.Open, FileAccess.Read), new FileStream($"{pt}{day}.{month}.{year}-1.jpg", FileMode.Open, FileAccess.Read) };
                    var brArr = new[] { new BinaryReader(fsArr[0]), new BinaryReader(fsArr[1]) };
                    var filebytesArr = new[] { brArr[0].ReadBytes((int)fsArr[0].Length), brArr[1].ReadBytes((int)fsArr[1].Length) };
                    var file1 = new InputFile(filebytesArr[0], $"{pt}{day}.{month}.{year}-0.jpg");
                    var file2 = new InputFile(filebytesArr[1], $"{pt}{day}.{month}.{year}-1.jpg");
                    var files = new[]
                    {
                        new AttachedFile($"{pt}{day}.{month}.{year}-0.jpg", file1),
                        new AttachedFile($"{pt}{day}.{month}.{year}-1.jpg", file2)
                    };
                    await botClient.SendMediaGroupAsync(message.Chat.Id, new[]
                    {
                        new InputMediaPhoto($"attach://{pt}{day}.{month}.{year}-0.jpg"), 
                        new InputMediaPhoto($"attach://{pt}{day}.{month}.{year}-1.jpg")
                    }, 
                        attachedFiles: files, cancellationToken: cancellationToken).ConfigureAwait(true);
                    break;
                }
                await botClient.SendPhotoAsync(message.Chat.Id, new InputFile(await File.ReadAllBytesAsync($"{pt}{day}.{month}.{year}-0.jpg", cancellationToken).ConfigureAwait(true), $"{pt}{day}.{month}.{year}-0.jpg"), cancellationToken: cancellationToken);
            }
            else{
                var urlCheckResult = await CheckUrl($"https://mkeiit.ru/wp-content/uploads/{year}/{temnMonth}/{day}.{month}.{year}.pdf").ConfigureAwait(true);
                if (urlCheckResult)
                {
                    Directory.CreateDirectory(pt);
                    await DownLoad($"https://mkeiit.ru/wp-content/uploads/{year}/{temnMonth}/{day}.{month}.{year}.pdf", $"{pt}{day}.{month}.{year}.pdf", moscowTime).ConfigureAwait(true);
                    await ConvertPdFtoHojas($"{pt}", day, month, year).ConfigureAwait(true);
                    continue;
                }
                await botClient.SendMessageAsync(message.Chat.Id, "Расписания на " + day + "." + month + "." + year + " нет", cancellationToken: cancellationToken).ConfigureAwait(true);
            }
            break;
        }
    }
    private static async Task ConvertPdFtoHojas(string path, string day, string month, string year)
    {
        using var pdfDocument = new PdfDocument(File.Open(path + day + "." + month + "." + year + ".pdf", FileMode.Open));
        using var pagesi = pdfDocument;
        for (var i = 0; i < pagesi.Pages.Count; i++)
        {
            var pageWidth = pagesi.Pages[i].Size.Width;
            var pageHeight = pagesi.Pages[i].Size.Height;
            using var bitmap = new PdfiumBitmap((int)pageWidth, (int)pageHeight, false);
            pagesi.Pages[i].Render(bitmap, PageOrientations.Normal, RenderingFlags.LcdText);
            byte[] byteArray;
            using (var memoryStream = new MemoryStream())
            {
                await bitmap.AsBmpStream(1 , 1).CopyToAsync(memoryStream).ConfigureAwait(true);
                byteArray = memoryStream.ToArray();
            }
            await File.WriteAllBytesAsync(path + day + "." + month + "." + year + $"-{i}.png", byteArray).ConfigureAwait(true);
            var image1 = await Image.LoadAsync($"{path}{day}.{month}.{year}-{i}.png").ConfigureAwait(true);
            await image1.SaveAsync($"{path}{day}.{month}.{year}-{i}.jpg", new JpegEncoder { Quality = 100 }).ConfigureAwait(true);
            File.Delete(path + day + "." + month + "." + year + $"-{i}.png");
        }
        File.Delete(path + day + "." + month + "." + year + ".pdf");
    }
}