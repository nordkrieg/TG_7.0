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
            var pathsave = path;
            path += day + "." + month + "." + year + ".pdf";
            var dd = await File.ReadAllBytesAsync(path);
            for (var i = 1; i < 3; i++)
            {
                var pngByte = Freeware.Pdf2Png.Convert(dd, i);
                await File.WriteAllBytesAsync(Path.Combine(pathsave, day + "." + month + "." + year + $"-{i}.png"), pngByte);
            }
            var pdfList = Directory.GetFiles(SchFold, "*.pdf");
            foreach (var f in pdfList)
            {
                File.Delete(f);
            }
        }
    }
}