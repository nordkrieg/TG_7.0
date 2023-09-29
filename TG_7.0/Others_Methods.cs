using PdfLibCore;
using PdfLibCore.Enums;
using SixLabors.ImageSharp.Formats.Jpeg;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.AvailableMethods;
using File = System.IO.File;
using Image = SixLabors.ImageSharp.Image;
using InputFile = Telegram.BotAPI.AvailableTypes.InputFile;
using System;

namespace TG_7._0;
public abstract class OthersMethods
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
        if (day[0] == '0')  _ = day.TrimStart('0');
        if (await CheckUrl(url))
        {
            var fileBytes = await client.GetByteArrayAsync(url);
            await File.WriteAllBytesAsync(path, fileBytes);
        }
    }

    public static async Task Pari(BotClient botClient, CancellationToken cancellationToken, Message message, int x, string[] days)
    {
        string day, month, year, temn_month;
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
        switch (month)
        {
            case "10":
                temn_month = "09";
                break;
            case "06":
                temn_month = "05";
                break;
            default:
                temn_month = month;
                break;
        }
        while (true)
        {
            if (Directory.Exists(pt))
            {
                if (File.Exists($"{pt}{day}.{month}.{year}-1.jpg"))
                {
                    await botClient.SendPhotoAsync(message.Chat.Id, new InputFile(await File.ReadAllBytesAsync($"{pt}{day}.{month}.{year}-0.jpg", cancellationToken), $"{pt}{day}.{month}.{year}-0.jpg"), cancellationToken: cancellationToken);
                    await botClient.SendPhotoAsync(message.Chat.Id, new InputFile(await File.ReadAllBytesAsync($"{pt}{day}.{month}.{year}-1.jpg", cancellationToken), $"{pt}{day}.{month}.{year}-1.jpg"), cancellationToken: cancellationToken);
                    break;
                }
                await botClient.SendPhotoAsync(message.Chat.Id, new InputFile(await File.ReadAllBytesAsync($"{pt}{day}.{month}.{year}-0.jpg", cancellationToken), $"{pt}{day}.{month}.{year}-0.jpg"), cancellationToken: cancellationToken);
                break;
            }
            else{
                var urlCheckResult = await CheckUrl($"https://mkeiit.ru/wp-content/uploads/{year}/{temn_month}/{day}.{month}.{year}.pdf");
                if (urlCheckResult)
                {
                    Directory.CreateDirectory(pt);
                    await DownLoad($"https://mkeiit.ru/wp-content/uploads/{year}/{temn_month}/{day}.{month}.{year}.pdf", $"{pt}{day}.{month}.{year}.pdf", moscowTime);
                    await ConvertPdFtoHojas($"{pt}", day, month, year);
                    continue;
                }
                await botClient.SendMessageAsync(message.Chat.Id, $"Расписания на " + day + "." + month + "." + year + " нет", cancellationToken: cancellationToken);
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
            var pageWidth = (pagesi.Pages[i].Size.Width);
            var pageHeight = (pagesi.Pages[i].Size.Height);
            using var bitmap = new PdfiumBitmap((int)pageWidth, (int)pageHeight, false);
            pagesi.Pages[i].Render(bitmap, PageOrientations.Normal, RenderingFlags.LcdText);
            byte[] byteArray;
            using (var memoryStream = new MemoryStream())
            {
                await bitmap.AsBmpStream(1 , 1).CopyToAsync(memoryStream);
                byteArray = memoryStream.ToArray();
            }
            await File.WriteAllBytesAsync((path + day + "." + month + "." + year + $"-{i}.png"), byteArray);
            var image1 = await Image.LoadAsync($"{path}{day}.{month}.{year}-{i}.png");
            var encoder = new JpegEncoder { Quality = 100 };
            await image1.SaveAsync($"{path}{day}.{month}.{year}-{i}.jpg", encoder);
            File.Delete(path + day + "." + month + "." + year + $"-{i}.png");
        }
        File.Delete(path + day + "." + month + "." + year + ".pdf");
    }
}