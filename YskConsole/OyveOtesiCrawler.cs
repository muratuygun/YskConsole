
using Newtonsoft.Json;
using System.Net;
using YskConsole.Models;

namespace YskConsole
{
    public class OyveOtesiCrawler
    {
        HttpClient httpClient;
        public OyveOtesiCrawler()
        {
            RefreshHttpClient();
        }

        private void RefreshHttpClient()
        {
            var socketsHandler = new SocketsHttpHandler() { AllowAutoRedirect = false, MaxConnectionsPerServer = 20, AutomaticDecompression = System.Net.DecompressionMethods.All };
            socketsHandler.UseProxy = true;
            socketsHandler.Proxy = new WebProxy("zproxy.lum-superproxy.io:22225");
            socketsHandler.Proxy.Credentials = new NetworkCredential("brd-customer-hl_edb94c60-zone-zone4", "gsfayrfr61cw");
            httpClient = new HttpClient(socketsHandler);
        }

        public async Task<List<OyveOtesiCities>?> GetCities()
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "https://api-sonuc.oyveotesi.org/api/v1/cities");

            httpRequestMessage.Headers.Add("Origin", "https://tutanak.oyveotesi.org");
            httpRequestMessage.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Safari/537.36");

            var response = await httpClient.SendAsync(httpRequestMessage);
            var htmlContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<OyveOtesiCities>>(htmlContent);
        }


        public async Task<List<OyveOtesiDistricts>?> GetDistricts(string CityId)
        {
            for (int i = 0; i < 10; i++)
            {
                try
                {

                    HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, $"https://api-sonuc.oyveotesi.org/api/v1/cities/{CityId}/districts");

                    httpRequestMessage.Headers.Add("Origin", "https://tutanak.oyveotesi.org");
                    httpRequestMessage.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Safari/537.36");

                    var response = await httpClient.SendAsync(httpRequestMessage);
                    var htmlContent = await response.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<List<OyveOtesiDistricts>>(htmlContent);
                }
                catch (Exception)
                {

                }
                RefreshHttpClient();
                await Task.Delay(1000);
            }
            return null;
        }

        //https://api-sonuc.oyveotesi.org/api/v1/cities/57/districts/732/neighborhoods

        public async Task<List<OyveOtesiNeighborhoods>?> GetNeighborhoods(string CityId,string districtId)
        {
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, $"https://api-sonuc.oyveotesi.org/api/v1/cities/{CityId}/districts/{districtId}/neighborhoods");

                    httpRequestMessage.Headers.Add("Origin", "https://tutanak.oyveotesi.org");
                    httpRequestMessage.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Safari/537.36");

                    var response = await httpClient.SendAsync(httpRequestMessage);
                    var htmlContent = await response.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<List<OyveOtesiNeighborhoods>>(htmlContent);
                }
                catch (Exception)
                {

                }
                RefreshHttpClient();
                await Task.Delay(5000);
            }

            return null;
        }

        public async Task<List<OyveItesiOylar>?> GetOylarrr(string neighborhoodId)
        {
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, $"https://api-sonuc.oyveotesi.org/api/v1/submission/neighborhood/{neighborhoodId}");


                    httpRequestMessage.Headers.TryAddWithoutValidation("Connection", "keep-alive");
                    httpRequestMessage.Headers.TryAddWithoutValidation("sec-ch-ua", "\"Google Chrome\";v=\"113\", \"Chromium\";v=\"113\", \"Not-A.Brand\";v=\"24\"");
                    httpRequestMessage.Headers.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
                    httpRequestMessage.Headers.TryAddWithoutValidation("sec-ch-ua-mobile", "?0");
                    httpRequestMessage.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Safari/537.36");
                    httpRequestMessage.Headers.TryAddWithoutValidation("sec-ch-ua-platform", "\"Windows\"");
                    httpRequestMessage.Headers.TryAddWithoutValidation("Origin", "https://tutanak.oyveotesi.org");
                    httpRequestMessage.Headers.TryAddWithoutValidation("Sec-Fetch-Site", "none");
                    httpRequestMessage.Headers.TryAddWithoutValidation("Sec-Fetch-Mode", "navigate");
                    httpRequestMessage.Headers.TryAddWithoutValidation("Sec-Fetch-Dest", "document");
                    httpRequestMessage.Headers.TryAddWithoutValidation("Sec-Fetch-User", "?1");

                    httpRequestMessage.Headers.TryAddWithoutValidation("Referer", "https://tutanak.oyveotesi.org/");
                    httpRequestMessage.Headers.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate, br");
                    httpRequestMessage.Headers.TryAddWithoutValidation("Accept-Language", "tr-TR,tr;q=0.9,en-US;q=0.8,en;q=0.7");

                    httpRequestMessage.Headers.TryAddWithoutValidation("Upgrade-Insecure-Requests", "1");

                    var response = await httpClient.SendAsync(httpRequestMessage);
                    var htmlContent = await response.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<List<OyveItesiOylar>>(htmlContent);
                }
                catch (Exception)
                {

                }
                RefreshHttpClient();
                await Task.Delay(1000);
            }

            return null;
        }

    }
}
