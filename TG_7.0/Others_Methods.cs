using PdfLibCore;
using PdfLibCore.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using File = System.IO.File;

namespace TG_7._0
{
    public class OthersMethods
    {
        private static string SchFold => "../../../Fold_data/sch_fold/";
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
        protected static async Task Pari(ITelegramBotClient botClient, CancellationToken cancellationToken, Message message, int x)
        {
            var moscowTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time"));
            var day = Convert.ToString(Convert.ToInt32(moscowTime.Day.ToString()) + x);
            var month = Convert.ToString(Convert.ToInt32(moscowTime.Month.ToString()));
            var year = Convert.ToString(Convert.ToInt32(moscowTime.Year.ToString()));
            if (month.Length != 2) month = "0" + month;
            if (day[0] == '0') day = day.TrimStart('0');
            var pt = SchFold + day + "." + month + "." + year + "/";
            while (true)
            {
                if (Directory.Exists(pt))
                {
                    Console.WriteLine($"{pt}{day}.{month}.{year}-0.png");
                    Stream stream = File.OpenRead($"{pt}{day}.{month}.{year}-0.png");
                    Stream stream2 = File.OpenRead($"{pt}{day}.{month}.{year}-1.png");
                    IAlbumInputMedia[] streamArray =
                    {
                        new InputMediaPhoto(InputFile.FromStream(stream, $"{pt}{day}.{month}.{year}-0.png")),
                        new InputMediaPhoto(InputFile.FromStream(stream2, $"{pt}{day}.{month}.{year}-1.png")),
                    };
                    await botClient.SendMediaGroupAsync(message.Chat.Id, streamArray, cancellationToken: cancellationToken);
                    break;
                }

                var urlCheckResult = await CheckUrl($"https://mkeiit.ru/wp-content/uploads/{year}/{month}/{day}.{month}.{year}.pdf");
                if (urlCheckResult == true)
                {
                    Directory.CreateDirectory(pt);
                    await DownLoad($"https://mkeiit.ru/wp-content/uploads/{year}/{month}/{day}.{month}.{year}.pdf", $"{pt}{day}.{month}.{year}.pdf", moscowTime);
                    await ConvertPdFtoHojas($"{pt}", day, month, year); 
                    continue;
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Расписания на {moscowTime.AddDays(x).ToShortDateString()} нет", cancellationToken: cancellationToken);
                }
                break;
            }
        }
        private static async Task ConvertPdFtoHojas(string path, string day, string month, string year)
        {
            var pathsave = path;
            path += day + "." + month + "." + year + ".pdf";
            using var pdfDocument = new PdfDocument(File.Open(path, FileMode.Open));
            using var pagesi = pdfDocument;
            var dpiX = 300D;
            var dpiY = 300D;
            for (var i = 0; i < pagesi.Pages.Count; i++)
            {
                var pageWidth = (int)(300 * pagesi.Pages[i].Size.Width / 72);
                var pageHeight = (int)(300 * pagesi.Pages[i].Size.Height / 72);
                using var bitmap = new PdfiumBitmap(pageWidth, pageHeight, true);
                pagesi.Pages[i].Render(bitmap, PageOrientations.Normal, RenderingFlags.LcdText);
                byte[] byteArray;
                using (var memoryStream = new MemoryStream())
                {
                    await bitmap.AsBmpStream(dpiX, dpiY).CopyToAsync(memoryStream);
                    byteArray = memoryStream.ToArray();
                }
                await File.WriteAllBytesAsync((pathsave + day + "." + month + "." + year + $"-{i}.png"), byteArray);
            }
        }

    }
}