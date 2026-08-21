using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Kartian_s_Launcher
{
    public class InternetData
    {
        public string Title;
        public string Extract;
    }
    public class InternetInfo
    {
        private CookieContainer cc;
        private HttpClientHandler ch;
        private HttpClient hc;

        public void Declare()
        {
            cc = new CookieContainer();
            ch = new HttpClientHandler();
            hc = new HttpClient(ch);
            hc.DefaultRequestHeaders.Add(
                "User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:154.0) Gecko/20100101 Firefox/154.0"
            );
        }

        public async Task<InternetData> GetInfo(string name)
        {
            HttpResponseMessage objects = await hc.GetAsync($"https://en.wikipedia.org/api/rest_v1/page/summary/{name}");
            string objectsS = await objects.Content.ReadAsStringAsync();
            InternetData data = new InternetData();
            JsonDocument dataJson = JsonSerializer.Deserialize<JsonDocument>(objectsS);

            InternetData id = new InternetData() {
                Title = dataJson.RootElement.GetProperty("title").ToString(),
                Extract = dataJson.RootElement.GetProperty("extract").ToString()
            };

            return id;
        }
    }
}
