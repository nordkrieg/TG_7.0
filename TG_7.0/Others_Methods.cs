using System.Net;
using System.Net.Sockets;
using Telegram.Bot;
using Telegram.Bot.Types;
using File = System.IO.File;

namespace TG_7._0
{
    public class OthersMethods
    {
        protected static DateTime GetNetworkTime()
        {
            const string ntpServer = "0.ru.pool.ntp.org";
            var ntpData = new byte[48];
            ntpData[0] = 0x1B;
            try
            {
                var addresses = Dns.GetHostEntry(ntpServer).AddressList;
                var ipEndPoint = new IPEndPoint(addresses[0], 123);
                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                socket.ReceiveTimeout = 4000;
                socket.Connect(ipEndPoint);
                socket.Send(ntpData);
                socket.Receive(ntpData);
                socket.Close();
                var intPart = ((ulong)ntpData[40] << 24) | ((ulong)ntpData[41] << 16) | ((ulong)ntpData[42] << 8) |
                              ntpData[43];
                var fractPart = ((ulong)ntpData[44] << 24) | ((ulong)ntpData[45] << 16) | ((ulong)ntpData[46] << 8) |
                                ntpData[47];
                var milliseconds = intPart * 1000 + fractPart * 1000 / 0x100000000L + 10798900;
                var networkDateTime = new DateTime(1900, 1, 1).AddMilliseconds((long)milliseconds);
                return networkDateTime;
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Ошибка соксета при получении времени: " + ex.Message);
                return DateTime.Now;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка при получении времени: " + ex.Message);
                return DateTime.Now;
            }
        }
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
        private static async void ConvertFile(string path, string day, string month, string year)
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
        protected static async Task Pari(string day, string month, string year, ITelegramBotClient botClient, CancellationToken cancellationToken, Message message, int X)
        {
            day = Convert.ToString(Convert.ToInt32(day) + X);
            Console.WriteLine(day);
            if (month.Length != 2) month = "0" + month;
            if (day[0] == '0') day = day.TrimStart('0');
            var pt = SchFold + "/" + day + "." + month + "." + year + "/";
            while (true)
            {
                if (Directory.Exists(pt))
                {
                    Stream stream = File.OpenRead($"{pt}{day}.{month}.{year}-1.png"); 
                    Stream stream2 = File.OpenRead($"{pt}{day}.{month}.{year}-2.png");
                    await botClient.SendMediaGroupAsync(message.Chat.Id, new IAlbumInputMedia[] { new InputMediaPhoto(InputFile.FromStream(stream, $"{pt}{day}.{month}.{GetNetworkTime().Year}-1.png")), new InputMediaPhoto(InputFile.FromStream(stream2, $"{pt}{day}.{month}.{GetNetworkTime().Year}-2.png")) }, cancellationToken: cancellationToken);
                    break;
                }

                var urlCheckResult = await CheckUrl($"https://mkeiit.ru/wp-content/uploads/{GetNetworkTime().Year}/{month}/{day}.{month}.{GetNetworkTime().Year}.pdf");
                if (urlCheckResult == true)
                {
                    Directory.CreateDirectory(pt);
                    await DownLoad($"https://mkeiit.ru/wp-content/uploads/{year}/{month}/{day}.{month}.{year}.pdf", $"{pt}{day}.{month}.{year}.pdf", GetNetworkTime());
                    ConvertFile($"{pt}", day, month, year);
                    Thread.Sleep(1000);
                    continue;
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Расписания на {GetNetworkTime().AddDays(X).ToShortDateString()} нет", cancellationToken: cancellationToken);
                }
                break;
            }
        }
    }
}