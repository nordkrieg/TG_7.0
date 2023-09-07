using ConvertApiDotNet;
using System.Net;

namespace TelegramBotFor3P_v0._1
{
    public class Others_Methods
    {
        private static string version_bot = "v0.2";
        private static string sch_fold = "../../../Fold_data/sch_fold/";
        public static readonly string[] access_user = { "1362885017", "1358678174", "595981163", "707667309" };
        public static readonly long[] id_us = { 1362885017, 1358678174, 595981163, 707667309 };
        public static readonly string[] value = { "/start", "/info", "/news", "/call_schedule", "/schedule_today", "/schedule_tomorrow", "/schedule_session", "/capybara", "/support", "/update", "/techwork", "привет", "спасибо", "/debug", "/kill", "copy", "/future_updates", "/bugs", "/restart", "/teacher_list", "/debugPath", "/custom_sch", "/stop_test", "/st_test", "/tech", "/return", "/stop_test_tod" };
        public static string Version_bot { get => version_bot; set => version_bot = value; }
        public static string Sch_fold { get => sch_fold; set => sch_fold = value; }

        //checks the URL for existence
        public static bool CheckURL(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            WebRequest request = WebRequest.Create(url);
            try
            {
                HttpWebResponse res = request.GetResponse() as HttpWebResponse;

                if (res.StatusDescription == "OK")
                    return true;
            }
            catch
            {
            }
            return false;
        }

        //checks the path for existence 
        public static bool CheckPath(string path)
        {
            FileInfo infopath = new(path);

            if (infopath.Exists)
                return true;
            else return false;
        }

        //Check for Saturday
        public static bool CheckDateSat(DateTime date)
        {
            if (date.DayOfWeek.ToString() == "Saturday")
                return true;
            else return false;
        }

        //Check for Sunday
        public static bool CheckDateSun(DateTime date)
        {
            if (date.DayOfWeek.ToString() == "Sunday")
                return true;
            else return false;
        }

        public static void DownLoad(string url, string path, DateTime date)
        {
            string day = date.DayOfYear.ToString();
            string month = date.Month.ToString();

            if (Convert.ToInt32(month) < 10)
                _ = "0" + month;

            if (day[0] == '0')
                _ = day.TrimStart('0');

            using var client = new WebClient();
            if (CheckURL(url))
                client.DownloadFile(url, path);
        }

        public static async void ConvertFile(string path, DateTime date)
        {
            string day = date.Day.ToString();
            string month = date.Month.ToString();
            string year = date.Year.ToString();
            if (Convert.ToInt32(month) < 10) month = "0" + month;
            if (day[0] == '0') day = day.TrimStart('0');
            var convertApi = new ConvertApi("de94yzMvTMSKKeEc");
            var convert = await convertApi.ConvertAsync("pdf", "jpg",
                new ConvertApiFileParam("File", @$"{path}{day}.{month}.{year}.pdf")
            );
            await convert.SaveFilesAsync(path);
        }

        public static void DeletePDF()
        {
            string[] pdfList = Directory.GetFiles(Sch_fold, "*.pdf");
            foreach (string f in pdfList)
            {
                File.Delete(f);
            }
        }
    }
}
/*
    000000000000000000000000000000000000000000000000000000000000
    0000000000+++++++00000000+00000000+00000000000+++++++00000000000000
    0000000000+00000000000000+00000000+00000000000+000000000000000000000
    0000000000+00000000000000+00000000+00000000000+00000000000000000000
    0000000000+00000000000000+00000000+00000000000+00000000000000000000
    0000000000+++++++00000000+00000000+00000000000+++++++000000000000000
    0000000000000000+00000000+00000000+00000000000000000+00000000000000
    0000000000000000+000000000+000000+000000000000000000+00000000000000
    0000000000000000+0000000000+0000+0000000000000000000+00000000000000
    0000000000+++++++0000000000++++++0000000000000+++++++00000000000000
    000000000000000000000000000000000000000000000000000000000000++++++++++++++++++++++++++++++
        ++++++++++++++++++++++++++++++
        ++++++++++++++++++++++++++++++
        +++++++++######+++++++++++++++
        ++++++++####000000++++++++++++
        ++++++######0000000+++++++++++
        ++++++######000000++++++++++++
        ++++++##########++++++++++++++
        +++++++++#######++++++++++++++
        +++++++++#######++++++++++++++
        +++++++++##++##+++++++++++++
        ++++++++++++++++++++++++++++++
    000000000000000000000000000000000000000000000000000000000000
    000000000000000000000000000000000000000000000000000000000000
    000000000000000000000000000000000000000000000000000000000000
    000000000000000000000000000000000000000000000000000000000000
 
 
 
 
 
 
 
 
 
 shlepok
 */