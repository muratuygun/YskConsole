namespace YskConsole.Models
{
    public class LocationUrlCrawler : AspNetBaseCrawler
    {
        public string TcKimlikNo { get; set; }
        public bool Bulunamadi { get; set; }
        public string LocationUrl { get; set; }
        public bool Hatali { get; internal set; }
    }
}
