using ConvertApiDotNet;

namespace TG_7._0
{
    public class OthersMethods
    {
         protected static string SchFold => "../../../Fold_data/sch_fold/";
        protected static async Task<bool> CheckUrl(string url)
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
        /*//Check for Saturday
         public static bool CheckDateSat(DateTime date)
         {
             return date.DayOfWeek.ToString() == "Saturday";
         }

         //Check for Sunday
         public static bool CheckDateSun(DateTime date)
         {
             return date.DayOfWeek.ToString() == "Sunday";
         }*/
        protected static async Task DownLoad(string url, string path, DateTime date)
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
        protected static async void ConvertFile(string path, DateTime date)
        {
            var day = date.Day.ToString();
            var month = date.Month.ToString();
            var year = date.Year.ToString();
            if (Convert.ToInt32(month) < 10) month = "0" + month;
            if (day[0] == '0') day = day.TrimStart('0');
            var convertApi = new ConvertApi("de94yzMvTMSKKeEc");
            var convert = await convertApi.ConvertAsync("pdf", "jpg",
                new ConvertApiFileParam("File", $"{path}{day}.{month}.{year}.pdf")
            );
            await convert.SaveFilesAsync(path);
        }
        protected static void DeletePdf()
        {
            var pdfList = Directory.GetFiles(SchFold, "*.pdf");
            foreach (var f in pdfList)
            {
                File.Delete(f);
            }
        }
    }
}