using PdfLibCore;
using PdfLibCore.Enums;
using SixLabors.ImageSharp.Formats.Jpeg;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using File = System.IO.File;

namespace Schedule;

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
    private static async Task DownLoad(string url, string path, DateTime date)
    {
        var day = date.DayOfYear.ToString();
        using var client = new HttpClient();
        var month = date.Month.ToString();
        if (Convert.ToInt32(month) < 10) _ = "0" + month;
        if (day[0] == '0') _ = day.TrimStart('0');
        if (await CheckUrl(url))
        {
            var fileBytes = await client.GetByteArrayAsync(url);
            await File.WriteAllBytesAsync(path, fileBytes);
        }
    }
    public static async Task Pari(BotClient botClient, CancellationToken cancellationToken, Message message, int x,
        string[] days)
    {
        string day, month, year;
        var moscowTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
            TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time"));
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

        var pt = "Fold_data/sch_fold/" + day + "." + month + "." + year + "/";
        var temnMonth = month;
        while (true)
        {
            if (Directory.Exists(pt))
            {
                if (File.Exists($"{pt}{day}.{month}.{year}-1.jpg"))
                {
                    var fs1 = new FileStream($"{pt}{day}.{month}.{year}-0.jpg", FileMode.Open, FileAccess.Read);
                    var br1 = new BinaryReader(fs1);
                    var filebytes1 = br1.ReadBytes((int)fs1.Length);
                    var fs2 = new FileStream($"{pt}{day}.{month}.{year}-1.jpg", FileMode.Open, FileAccess.Read);
                    var br2 = new BinaryReader(fs2);
                    var filebytes2 = br2.ReadBytes((int)fs2.Length);
                    var file1 = new InputFile(filebytes1, "odin.jpg");
                    var file2 = new InputFile(filebytes2, "dva.jpg");
                    var files = new[]
                    {
                        new AttachedFile("odin.jpg", file1),
                        new AttachedFile("dva.jpg", file2)
                    };
                    await botClient.SendMediaGroupAsync(message.Chat.Id, new[]
                        {
                            new InputMediaPhoto("attach://odin.jpg"),
                            new InputMediaPhoto("attach://dva.jpg")
                        },
                        attachedFiles: files, cancellationToken: cancellationToken);
                    break;
                }

                await botClient.SendPhotoAsync(message.Chat.Id,
                    new InputFile(await File.ReadAllBytesAsync($"{pt}{day}.{month}.{year}-0.jpg", cancellationToken),
                        $"{pt}{day}.{month}.{year}-0.jpg"), cancellationToken: cancellationToken);
                break;
            }

            var urlCheckResult =
                await CheckUrl($"https://mkeiit.ru/wp-content/uploads/{year}/{temnMonth}/{day}.{month}.{year}.pdf");
            if (urlCheckResult)
            {
                Directory.CreateDirectory(pt);
                await DownLoad($"https://mkeiit.ru/wp-content/uploads/{year}/{temnMonth}/{day}.{month}.{year}.pdf",
                    $"{pt}{day}.{month}.{year}.pdf", moscowTime);
                await ConvertPdFtoHojas($"{pt}", day, month, year);
                continue;
            }

            temnMonth = (Convert.ToInt32(temnMonth) - 1).ToString();
            if (temnMonth.Length != 2) temnMonth = "0" + temnMonth;
            urlCheckResult =
                await CheckUrl($"https://mkeiit.ru/wp-content/uploads/{year}/{temnMonth}/{day}.{month}.{year}.pdf");
            if (urlCheckResult)
            {
                Directory.CreateDirectory(pt);
                await DownLoad($"https://mkeiit.ru/wp-content/uploads/{year}/{temnMonth}/{day}.{month}.{year}.pdf",
                    $"{pt}{day}.{month}.{year}.pdf", moscowTime);
                await ConvertPdFtoHojas($"{pt}", day, month, year);
                continue;
            }

            await botClient.SendMessageAsync(message.Chat.Id,
                "Расписания на " + day + "." + month + "." + year + " нет", cancellationToken: cancellationToken);
            break;
        }
    }

    private static async Task ConvertPdFtoHojas(string path, string day, string month, string year)
    {
        using var pdfDocument =
            new PdfDocument(File.Open(path + day + "." + month + "." + year + ".pdf", FileMode.Open));
        using var pagesi = pdfDocument;
        for (var i = 0; i < pagesi.Pages.Count; i++)
        {
            using var bitmap =
                new PdfiumBitmap((int)pagesi.Pages[i].Size.Width, (int)pagesi.Pages[i].Size.Height, false);
            pagesi.Pages[i].Render(bitmap, PageOrientations.Normal, RenderingFlags.LcdText);
            byte[] byteArray;
            using (var memoryStream = new MemoryStream())
            {
                await bitmap.AsBmpStream(1, 1).CopyToAsync(memoryStream);
                byteArray = memoryStream.ToArray();
            }

            await File.WriteAllBytesAsync(path + day + "." + month + "." + year + $"-{i}.png", byteArray);
            var image1 = await Image.LoadAsync($"{path}{day}.{month}.{year}-{i}.png");
            var encoder = new JpegEncoder { Quality = 100 };
            await image1.SaveAsync($"{path}{day}.{month}.{year}-{i}.jpg", encoder);
            File.Delete(path + day + "." + month + "." + year + $"-{i}.png");
        }

        File.Delete(path + day + "." + month + "." + year + ".pdf");
    }
}