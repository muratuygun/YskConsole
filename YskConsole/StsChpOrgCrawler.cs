using BeetleX;
using BeetleX.Http.Clients;
using BeetleX.Http.WebSockets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YskConsole.Database;
using YskConsole.Models;

namespace YskConsole
{
    public class StsChpOrgCrawler
    {
        IPAddress SelectedIpAddress;
        HttpClient httpClient;
        public StsChpOrgCrawler()
        {
            var Adresses = DiscoverInterfaces();

            SelectedIpAddress = Adresses[int.Parse(Console.ReadLine())];


            var socketsHandler = new System.Net.Http.SocketsHttpHandler() { AllowAutoRedirect = false, MaxConnectionsPerServer = 20, };
            socketsHandler.ConnectCallback = async (context, token) =>
            {
                Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                s.Bind(new IPEndPoint(SelectedIpAddress, 0));
                
                await s.ConnectAsync(context.DnsEndPoint, token);
                s.NoDelay = true;

                return new NetworkStream(s, ownsSocket: true);
            };
            httpClient = new HttpClient(socketsHandler);
        }

        public List<IPAddress> DiscoverInterfaces()
        {
            var IPAddresses = new List<IPAddress>();
            foreach (var @interface in NetworkInterface.GetAllNetworkInterfaces())
            {
                var properties = @interface.GetIPProperties();

                if (@interface.NetworkInterfaceType == NetworkInterfaceType.Loopback || 
                    @interface.OperationalStatus != OperationalStatus.Up ||
                    !@interface.Supports(NetworkInterfaceComponent.IPv4) || 
                    properties.GatewayAddresses.Count == 0 || 
                    properties.DnsAddresses.Count == 0)
                {
                    continue;
                }

                var addresses = @interface.GetIPProperties().UnicastAddresses.Where(x => x.Address.AddressFamily == AddressFamily.InterNetwork);

                foreach (var entry in addresses)
                {
                    IPAddresses.Add(entry.Address);
                    Console.WriteLine($"{IPAddresses.IndexOf(entry.Address)} : {entry.Address}");
                }
            }
            return IPAddresses;
        }

        public async Task<StsChpOrgGetModel> Get(string tckimlikno)
        {
            var defaultPage = await GetDefault();

            if (defaultPage == null) return null;

            var locationTcKimlikNo = await GetDefault(defaultPage, tckimlikno);

            if (locationTcKimlikNo == null)
            {
                return null;
            }

            if (locationTcKimlikNo.Bulunamadi == true)
            {
                return new StsChpOrgGetModel()
                {
                    locationUrlCrawler = locationTcKimlikNo,
                    
                };
            }

            if (locationTcKimlikNo.Hatali == true)
            {
                //Console.WriteLine($"Hatalı {tckimlikno}");
                return new StsChpOrgGetModel()
                {
                    locationUrlCrawler = locationTcKimlikNo,
                };
            }
            var milletvekilligi = await GetSonucMilletVekili(locationTcKimlikNo);
            //var cumhurbaskanligi = 
            var cumhurBaskani = await GetSonucCumhurBaskani(locationTcKimlikNo, milletvekilligi);


            return new StsChpOrgGetModel()
            {
                locationUrlCrawler = locationTcKimlikNo,
                milletVekiliSecimBilgileri = milletvekilligi,
                cumgurbaskanligiSecimBilgileri = cumhurBaskani
            };
        }

        public async Task<AspNetBaseCrawler?> GetDefault()
        {
            for (int i = 0; i < 15; i++)
            {
                try
                {
                    //var response = await ("https://sts.chp.org.tr/Default.aspx")
                    //    .FormUrlRequest()
                    //    .SetHeaders("Upgrade-Insecure-Requests", "1")
                    //    .SetHeaders("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Safari/537.36")
                    //    .Get();

                    //var htmlContent = response.Body.ToString();

                    HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "https://sts.chp.org.tr/Default.aspx");

                    httpRequestMessage.Headers.Add("Upgrade-Insecure-Requests", "1");
                    httpRequestMessage.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Safari/537.36");

                    var response = await httpClient.SendAsync(httpRequestMessage);
                    var htmlContent = await response.Content.ReadAsStringAsync();
                    string viewState = Regex.Match(htmlContent, @"id=""__VIEWSTATE"" value=""(.*?)""").Groups[1].Value;
                    string viewStateGenerator = Regex.Match(htmlContent, @"id=""__VIEWSTATEGENERATOR"" value=""(.*?)""").Groups[1].Value;
                    string eventValidation = Regex.Match(htmlContent, @"id=""__EVENTVALIDATION"" value=""(.*?)""").Groups[1].Value;

                    return new AspNetBaseCrawler()
                    {
                        eventValidation = eventValidation,
                        viewState = viewState,
                        viewStateGenerator = viewStateGenerator
                    };
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                await Task.Delay(100);
            }
            return null;
        }

        public async Task<LocationUrlCrawler?> GetDefault(AspNetBaseCrawler aspNetBaseCrawler, string tcKimlikNo)
        {
            for (int i = 0; i < 5; i++)
            {
                try
                {

                    //var host = new HttpHost("sts.chp.org.tr");
                    //HttpFormUrlClient client = new HttpFormUrlClient("https://sts.chp.org.tr/Default.aspx");

                    //var response = await ("https://sts.chp.org.tr/Default.aspx")
                    //    .FormUrlRequest()

                    //    .SetBodyFields("__EVENTTARGET", "")
                    //    .SetBodyFields("__EVENTARGUMENT", "")
                    //    .SetBodyFields("__LASTFOCUS", "")
                    //    .SetBodyFields("rdveriKaynagi", "")
                    //    .SetBodyFields("txtTCKN", tcKimlikNo)
                    //    .SetBodyFields("btnSorgula", "SORGULA")
                    //    .SetBodyFields("__VIEWSTATE", aspNetBaseCrawler.viewState)
                    //    .SetBodyFields("__VIEWSTATEGENERATOR", aspNetBaseCrawler.viewStateGenerator)
                    //    .SetBodyFields("__EVENTVALIDATION", aspNetBaseCrawler.eventValidation)

                    //    .SetHeaders("Upgrade-Insecure-Requests", "1")
                    //    .SetHeaders("Origin", "https://sts.chp.org.tr")
                    //    .SetHeaders("Referer", "https://sts.chp.org.tr/Default.aspx")
                    //    .SetHeaders("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Safari/537.36")
                    //.Post();


                    //var htmlContent = response.Body.ToString();

                    HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "https://sts.chp.org.tr/Default.aspx");

                    var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("__EVENTTARGET", ""),
                        new KeyValuePair<string, string>("__EVENTARGUMENT", ""),
                        new KeyValuePair<string, string>("__LASTFOCUS", ""),
                        new KeyValuePair<string, string>("rdveriKaynagi", "1"),
                        new KeyValuePair<string, string>("txtTCKN", tcKimlikNo),
                        new KeyValuePair<string, string>("btnSorgula", "SORGULA"),

                        new KeyValuePair<string, string>("__VIEWSTATE", aspNetBaseCrawler.viewState),
                        new KeyValuePair<string, string>("__VIEWSTATEGENERATOR", aspNetBaseCrawler.viewStateGenerator),
                        new KeyValuePair<string, string>("__EVENTVALIDATION", aspNetBaseCrawler.eventValidation)
                    };

                    var content = new FormUrlEncodedContent(values);

                    content.Headers.Add("Upgrade-Insecure-Requests", "1");
                    content.Headers.Add("Origin", "https://sts.chp.org.tr");
                    content.Headers.TryAddWithoutValidation("Referer", "https://sts.chp.org.tr/Default.aspx");
                    content.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Safari/537.36");

                    httpRequestMessage.Content = content;

                    var response = await httpClient.SendAsync(httpRequestMessage);
                    var htmlContent = await response.Content.ReadAsStringAsync();


                    string viewState = Regex.Match(htmlContent, @"id=""__VIEWSTATE"" value=""(.*?)""").Groups[1].Value;
                    string viewStateGenerator = Regex.Match(htmlContent, @"id=""__VIEWSTATEGENERATOR"" value=""(.*?)""").Groups[1].Value;
                    string eventValidation = Regex.Match(htmlContent, @"id=""__EVENTVALIDATION"" value=""(.*?)""").Groups[1].Value;

                    if (response == null || response.Headers == null)
                        throw new Exception("Response Bulunamadı");

                    if (htmlContent.Contains("Girmiş olduğunuz Tc kimlik numarası seçmen listesinde bulunamamıştır!"))
                    {
                        return new LocationUrlCrawler()
                        {
                            TcKimlikNo = tcKimlikNo,
                            Bulunamadi = true,
                            Hatali = true,
                        };
                    }

                    if (response.StatusCode != System.Net.HttpStatusCode.Found && !response.IsSuccessStatusCode)
                    {
                        return new LocationUrlCrawler()
                        {
                            TcKimlikNo = tcKimlikNo,
                            Hatali = true
                        };
                    }else if (response.Headers.Location == null)
                    {
                        return new LocationUrlCrawler()
                        {
                            TcKimlikNo = tcKimlikNo,
                            Hatali = true
                        };

                    }

                    return new LocationUrlCrawler()
                    {
                        TcKimlikNo = tcKimlikNo,
                        eventValidation = eventValidation,
                        viewState = viewState,
                        viewStateGenerator = viewStateGenerator,
                        LocationUrl = response.Headers.Location.ToString()
                    };
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

            }
            return null;
        }

        public async Task<MilletVekiliSecimBilgileri> GetSonucMilletVekili(LocationUrlCrawler locationUrlCrawler)
        {
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    //var response = await ("https://sts.chp.org.tr/" + locationUrlCrawler.LocationUrl)
                    //    .FormUrlRequest()
                    //    .SetHeaders("Upgrade-Insecure-Requests", "1")
                    //    .SetHeaders("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Safari/537.36")
                    //.Get();

                    //var htmlContent = response.Body.ToString();
                    HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "https://sts.chp.org.tr/" + locationUrlCrawler.LocationUrl);

                    httpRequestMessage.Headers.Add("Upgrade-Insecure-Requests", "1");
                    httpRequestMessage.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Safari/537.36");

                    var response = await httpClient.SendAsync(httpRequestMessage);
                    var htmlContent = await response.Content.ReadAsStringAsync();

                    return SonucDetayParse(locationUrlCrawler, htmlContent);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

            }
            return null;
        }

        private MilletVekiliSecimBilgileri SonucDetayParse(LocationUrlCrawler locationUrlCrawler, string html)
        {
            var regex = new Regex(@"<input[^>]*(id|name)=\""(?<id>[^\""]*)\""[^>]*value=\""(?<value>[^\""]*)\""[^>]*>", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            var secimBilgileri = new MilletVekiliSecimBilgileri();

            secimBilgileri.TcKimlikNo = locationUrlCrawler.TcKimlikNo;

            var matches = regex.Matches(html);
            foreach (Match item in matches)
            {
                var key = item.Groups[2].Value;
                var value = item.Groups[3].Value;
                switch (key)
                {
                    case "tbMvKayitliSecmenSayisi":
                        secimBilgileri.tbMvKayitliSecmenSayisi = value;
                        break;
                    case "tbMvOyKullananKayitliSecmenSayisi":
                        secimBilgileri.tbMvOyKullananKayitliSecmenSayisi = value;
                        break;
                    case "tbMvKanunGeregiOyKullananSayisi":
                        secimBilgileri.tbMvKanunGeregiOyKullananSayisi = value;
                        break;
                    case "tbMvOyKullanmaOrani":
                        secimBilgileri.tbMvOyKullanmaOrani = value;
                        break;
                    case "tbMvGecerliOySayisi":
                        secimBilgileri.tbMvGecerliOySayisi = value;
                        break;
                    case "tbMvGecersizOySayisi":
                        secimBilgileri.tbMvGecersizOySayisi = value;
                        break;
                    case "tbMvToplamOySayisi":
                        secimBilgileri.tbMvToplamOySayisi = value;
                        break;
                    case "tbMvItirazsizSonucSayisi":
                        secimBilgileri.tbMvItirazsizSonucSayisi = value;
                        break;
                    case "tbMvKesinSonucSayisi":
                        secimBilgileri.tbMvKesinSonucSayisi = value;
                        break;
                    case "tbMvBaskanSayisi":
                        secimBilgileri.tbMvBaskanSayisi = value;
                        break;
                    case "tbMvBaskanVekiliSayisi":
                        secimBilgileri.tbMvBaskanVekiliSayisi = value;
                        break;
                    case "tbMvUyeSayisi":
                        secimBilgileri.tbMvUyeSayisi = value;
                        break;
                    case "tbMvKatiplerSayisi":
                        secimBilgileri.tbMvKatiplerSayisi = value;
                        break;
                    case "tbMvKullanilanToplamOy":
                        secimBilgileri.tbMvKullanilanToplamOy = value;
                        break;
                    case "tbMvItirazsizGecerliOySayisi":
                        secimBilgileri.tbMvItirazsizGecerliOySayisi = value;
                        break;

                    case "tbMvItirazliGecerliOySayisi":
                        secimBilgileri.tbMvItirazliGecerliOySayisi = value;
                        break;

                    case "txtCHP":
                        secimBilgileri.txtCHP = value;
                        break;

                    case "txtAkp":
                        secimBilgileri.txtAkp = value;
                        break;

                    case "txtIyi":
                        secimBilgileri.txtIyi = value;
                        break;

                    case "txtYesilSol":
                        secimBilgileri.txtYesilSol = value;
                        break;

                    case "txtMhp":
                        secimBilgileri.txtMhp = value;
                        break;

                    case "txtDiger":
                        secimBilgileri.txtDiger = value;
                        break;

                    case "txtCumhur":
                        secimBilgileri.txtCumhur = value;
                        break;

                    case "txtMillet":
                        secimBilgileri.txtMillet = value;
                        break;

                    case "KalanGoster":
                        secimBilgileri.KalanGoster = value;
                        break;

                    case "btnSikayetEt":
                        secimBilgileri.btnSikayetEt = value;
                        break;

                    case "btnMV":
                        secimBilgileri.btnMV = value;
                        break;

                    case "btnCb":
                        secimBilgileri.btnCb = value;
                        break;

                    case "__EVENTTARGET":
                        secimBilgileri.eventTarget = value;
                        break;

                    case "__EVENTARGUMENT":
                        secimBilgileri.eventArgument = value;
                        break;

                    case "__VIEWSTATE":
                        secimBilgileri.viewState = value;
                        break;

                    case "__VIEWSTATEGENERATOR":
                        secimBilgileri.viewStateGenerator = value;
                        break;

                    case "__EVENTVALIDATION":
                        secimBilgileri.eventValidation = value;
                        break;

                    default:
                        Console.WriteLine(key + " /// " + value);

                        break;
                }
            }
            string pattern = "<span[^>]*id=\"(?<id>lblIlIlceBaslik|lblOzetIlIlce|lblMvOzetSandikAlani|lblMvGelisZamani)\"[^>]*>(?<value>[^<]*)</span>";

            Regex regexlabel = new Regex(pattern);
            MatchCollection matcheslabel = regexlabel.Matches(html);


            foreach (Match match in matcheslabel)
            {
                string id = match.Groups["id"].Value;
                string value = match.Groups["value"].Value;

                switch (id)
                {
                    case "lblIlIlceBaslik":
                        var split = value.Split('/');
                        if (split.Length == 1) break;
                        secimBilgileri.Il = split[0];
                        secimBilgileri.Ilce = split[1];
                        secimBilgileri.SandikNo = split[2];
                        secimBilgileri.lblIlIlceBaslik = value;
                        break;
                    case "lblOzetIlIlce":
                        secimBilgileri.lblOzetIlIlce = value;
                        break;
                    case "lblMvOzetSandikAlani":
                        secimBilgileri.lblMvOzetSandikAlani = value;
                        break;
                    case "lblMvGelisZamani":
                        secimBilgileri.lblMvGelisZamani = value;
                        break;
                }
            }
            return secimBilgileri;
        }

        public async Task<CumgurbaskanligiSecimBilgileri> GetSonucCumhurBaskani(LocationUrlCrawler locationUrlCrawler, MilletVekiliSecimBilgileri milletVekiliSecimBilgileri)
        {

            for (int i = 0; i < 5; i++)
            {
                try
                {

                    //var response = await ("https://sts.chp.org.tr/" + locationUrlCrawler.LocationUrl)
                    //    .FormUrlRequest()
                    //    .SetBodyFields("__EVENTTARGET", "")
                    //    .SetBodyFields("__EVENTARGUMENT", "")
                    //    .SetBodyFields("btnCb", "CUMHURBAŞKANI SEÇİM SONUÇLARI")
                    //    .SetBodyFields("__VIEWSTATE", milletVekiliSecimBilgileri.viewState)
                    //    .SetBodyFields("__VIEWSTATEGENERATOR", milletVekiliSecimBilgileri.viewStateGenerator)
                    //    .SetBodyFields("__EVENTVALIDATION", milletVekiliSecimBilgileri.eventValidation)

                    //    .SetHeaders("Upgrade-Insecure-Requests", "1")
                    //    .SetHeaders("Origin", "https://sts.chp.org.tr")
                    //    .SetHeaders("Referer", "https://sts.chp.org.tr/Default.aspx")
                    //    .SetHeaders("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Safari/537.36")
                    //.Post();


                    //var htmlContent = response.Body.ToString();

                    HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "https://sts.chp.org.tr/" + locationUrlCrawler.LocationUrl);

                    var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("__EVENTTARGET", ""),
                        new KeyValuePair<string, string>("__EVENTARGUMENT", ""),
                        new KeyValuePair<string, string>("btnCb", "CUMHURBAŞKANI SEÇİM SONUÇLARI"),

                        new KeyValuePair<string, string>("__VIEWSTATE", milletVekiliSecimBilgileri.viewState),
                        new KeyValuePair<string, string>("__VIEWSTATEGENERATOR", milletVekiliSecimBilgileri.viewStateGenerator),
                        new KeyValuePair<string, string>("__EVENTVALIDATION", milletVekiliSecimBilgileri.eventValidation)
                    };

                    var content = new FormUrlEncodedContent(values);

                    content.Headers.Add("Upgrade-Insecure-Requests", "1");
                    content.Headers.Add("Origin", "https://sts.chp.org.tr");
                    content.Headers.TryAddWithoutValidation("Referer", "https://sts.chp.org.tr/Default.aspx");
                    content.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Safari/537.36");

                    httpRequestMessage.Content = content;

                    var response = await httpClient.SendAsync(httpRequestMessage);
                    var htmlContent = await response.Content.ReadAsStringAsync();

                    if (response == null || response.Headers == null)
                        throw new Exception("Response Bulunamadı");

                    return CumhurbaskanligiParse(locationUrlCrawler, htmlContent);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

            }
            return null;
        }

        public CumgurbaskanligiSecimBilgileri CumhurbaskanligiParse(LocationUrlCrawler locationUrlCrawler, string html)
        {
            var secimBilgileri = new CumgurbaskanligiSecimBilgileri();

            secimBilgileri.TcKimlikNo = locationUrlCrawler.TcKimlikNo;

            var regex = new Regex(@"<input[^>]*(id|name)=\""(?<id>[^\""]*)\""[^>]*value=\""(?<value>[^\""]*)\""[^>]*>", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            var matches = regex.Matches(html);
            foreach (Match item in matches)
            {
                var key = item.Groups[2].Value;
                var value = item.Groups[3].Value;
                switch (key)
                {
                    case "txtCbKayitliSecmen":
                        secimBilgileri.txtCbKayitliSecmen = value;
                        break;
                    case "txtCbOyKullanan":
                        secimBilgileri.txtCbOyKullanan = value;
                        break;
                    case "txtCbKanunGeregi":
                        secimBilgileri.txtCbKanunGeregi = value;
                        break;
                    case "txtCbKullanilanToplamOy":
                        secimBilgileri.txtCbKullanilanToplamOy = value;
                        break;
                    case "txtCbItirazsizligecerli":
                        secimBilgileri.txtCbItirazsizligecerli = value;
                        break;
                    case "txtCbItirazligecerli":
                        secimBilgileri.txtCbItirazligecerli = value;
                        break;
                    case "txtCbGecerliOy":
                        secimBilgileri.txtCbGecerliOy = value;
                        break;
                    case "txtCbGercersizOy":
                        secimBilgileri.txtCbGercersizOy = value;
                        break;
                    case "txtCB1":
                        secimBilgileri.txtCB1 = value;
                        break;
                    case "txtCB2":
                        secimBilgileri.txtCB2 = value;
                        break;
                    case "txtCB3":
                        secimBilgileri.txtCB3 = value;
                        break;
                    case "txtCB4":
                        secimBilgileri.txtCB4 = value;
                        break;
                    case "txtAciklama":
                        secimBilgileri.txtAciklama = value;
                        break;
                    case "txtAdSoyad":
                        secimBilgileri.txtAdSoyad = value;
                        break;
                    case "txtTelefon":
                        secimBilgileri.txtTelefon = value;
                        break;

                    case "txtEPosta":
                        secimBilgileri.txtEPosta = value;
                        break;

                    case "KalanGoster":
                        secimBilgileri.KalanGoster = value;
                        break;

                    case "btnSikayetEt":
                        secimBilgileri.btnSikayetEt = value;
                        break;

                    case "btnMV":
                        secimBilgileri.btnMV = value;
                        break;

                    case "btnCb":
                        secimBilgileri.btnCb = value;
                        break;

                    case "__EVENTTARGET":
                        secimBilgileri.eventTarget = value;
                        break;

                    case "__EVENTARGUMENT":
                        secimBilgileri.eventArgument = value;
                        break;

                    case "__VIEWSTATE":
                        secimBilgileri.viewState = value;
                        break;

                    case "__VIEWSTATEGENERATOR":
                        secimBilgileri.viewStateGenerator = value;
                        break;

                    case "__EVENTVALIDATION":
                        secimBilgileri.eventValidation = value;
                        break;

                    default:
                        Console.WriteLine(key + " /// " + value);

                        break;
                }
            }
            string pattern = "<span[^>]*id\\s*=\\s*\"(lblCbIlIlceBaslik|lblCbSandikAlani|lblCbYskZamani)\"[^>]*>(.*?)</span>";


            Regex regexlabel = new Regex(pattern);
            MatchCollection matcheslabel = regexlabel.Matches(html);


            foreach (Match match in matcheslabel)
            {
                string id = match.Groups[1].Value;
                string value = match.Groups[2].Value;

                switch (id)
                {
                    case "lblCbIlIlceBaslik":
                        value = value.Replace("<b>", "").Replace("</b>", "");
                        var split = value.Split('/');
                        if (split.Length == 1) break;
                        
                        secimBilgileri.Il = split[0];
                        secimBilgileri.Ilce = split[1];
                        secimBilgileri.SandikNo = split[2];
                        secimBilgileri.lblCbIlIlceBaslik = value;
                        break;
                    case "lblCbSandikAlani":
                        secimBilgileri.lblCbSandikAlani = value;
                        break;
                    case "lblCbYskZamani":
                        secimBilgileri.lblCbYskZamani = value;
                        break;
                }
            }


            return secimBilgileri;
        }
    }
}
