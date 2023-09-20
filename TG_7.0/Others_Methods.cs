using PdfLibCore;
using PdfLibCore.Enums;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Text;
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
                    Stream stream = File.OpenRead($"{pt}{day}.{month}.{year}-3.jpg");
                    Stream stream2 = File.OpenRead($"{pt}{day}.{month}.{year}-4.jpg");
                    await botClient.SendMediaGroupAsync(message.Chat.Id, new IAlbumInputMedia[] { new InputMediaPhoto(InputFile.FromStream(stream, $"{pt}{day}.{month}.{year}-3.jpg")), 
                        new InputMediaPhoto(InputFile.FromStream(stream2, $"{pt}{day}.{month}.{year}-4.jpg")) }, cancellationToken: cancellationToken);
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
                await File.WriteAllBytesAsync((pathsave + day + "." + month + "." + year + $"-{i}.png"), byteArray);
                using var bmp1 = new Bitmap($"{pathsave}{day}.{month}.{year}-{i}.png");
                var jgpEncoder = GetEncoder(ImageFormat.Jpeg);
                var myEncoder = System.Drawing.Imaging.Encoder.Quality;
                EncoderParameters myEncoderParameters = new(1);
                var myEncoderParameter = new EncoderParameter(myEncoder, 100L);
                myEncoderParameters.Param[0] = myEncoderParameter;
                bmp1.Save($"{pathsave}{day}.{month}.{year}-{i + 3}.jpg", jgpEncoder, myEncoderParameters);
            }
        }
        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            var codecs = ImageCodecInfo.GetImageEncoders();
            return codecs.FirstOrDefault(codec => codec.FormatID == format.Guid);
        }
    }

}