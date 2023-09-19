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
        private static async Task ConvertFile(string path, string day, string month, string year)
        {
            var pathsave = path;
            path += day + "." + month + "." + year + ".pdf";
            var dd = await File.ReadAllBytesAsync(path);
            for (var i = 1; i < 3; i++)
            {
                var pngByte = Freeware.Pdf2Png.Convert(dd, i);
                await File.WriteAllBytesAsync(Path.Combine(pathsave, day + "." + month + "." + year + $"-{i}.png"), pngByte);
            }
            var pdfList = Directory.GetFiles(pathsave, "*.pdf");
            foreach (var f in pdfList)
            {
                File.Delete(f);
            }
        }
        protected static async Task Pari(ITelegramBotClient botClient, CancellationToken cancellationToken, Message message, int X)
        {
            var moscowTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time"));
            var day = Convert.ToString(Convert.ToInt32(moscowTime.Day.ToString()) + X);
            var month = Convert.ToString(Convert.ToInt32(moscowTime.Month.ToString()));
            var year = Convert.ToString(Convert.ToInt32(moscowTime.Year.ToString()));
            if (month.Length != 2) month = "0" + month;
            if (day[0] == '0') day = day.TrimStart('0');
            var pt = SchFold + "/" + day + "." + month + "." + year + "/";
            while (true)
            {
                if (Directory.Exists(pt))
                {
                    Stream stream = File.OpenRead($"{pt}{day}.{month}.{year}-1.png"); 
                    Stream stream2 = File.OpenRead($"{pt}{day}.{month}.{year}-2.png");
                    await botClient.SendMediaGroupAsync(message.Chat.Id, new IAlbumInputMedia[] { new InputMediaPhoto(InputFile.FromStream(stream, $"{pt}{day}.{month}.{year}-1.png")), new InputMediaPhoto(InputFile.FromStream(stream2, $"{pt}{day}.{month}.{year}-2.png")) }, cancellationToken: cancellationToken);
                    break;
                }

                var urlCheckResult = await CheckUrl($"https://mkeiit.ru/wp-content/uploads/{year}/{month}/{day}.{month}.{year}.pdf");
                if (urlCheckResult == true)
                {
                    Directory.CreateDirectory(pt);
                    await DownLoad($"https://mkeiit.ru/wp-content/uploads/{year}/{month}/{day}.{month}.{year}.pdf", $"{pt}{day}.{month}.{year}.pdf", moscowTime);
                    await ConvertFile($"{pt}", day, month, year);
                        //Thread.Sleep(2000);
                    continue;
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Расписания на {moscowTime.AddDays(X).ToShortDateString()} нет", cancellationToken: cancellationToken);
                }
                break;
            }
        }
    }
}