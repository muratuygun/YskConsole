namespace YskConsole.Models
{
    public class MilletVekiliSecimBilgileri : AspNetBaseCrawler
    {
        public string TcKimlikNo { get; set; }
        public string Il { get; set; }
        public string Ilce { get; set; }
        public string SandikNo { get; set; }

        public string tbMvKayitliSecmenSayisi { get; set; }
        public string tbMvOyKullananKayitliSecmenSayisi { get; set; }
        public string tbMvKanunGeregiOyKullananSayisi { get; set; }
        public string tbMvKullanilanToplamOy { get; set; }
        public string tbMvItirazsizGecerliOySayisi { get; set; }
        public string tbMvItirazliGecerliOySayisi { get; set; }
        public string tbMvGecerliOySayisi { get; set; }
        public string tbMvGecersizOySayisi { get; set; }
        public string txtCHP { get; set; }
        public string txtAkp { get; set; }
        public string txtIyi { get; set; }
        public string txtYesilSol { get; set; }
        public string txtMhp { get; set; }
        public string txtDiger { get; set; }
        public string txtCumhur { get; set; }
        public string txtMillet { get; set; }
        public string txtAciklama { get; set; }
        public string KalanGoster { get; set; }
        public string txtAdSoyad { get; set; }
        public string txtTelefon { get; set; }
        public string txtEPosta { get; set; }
        public string tbMvOyKullanmaOrani { get; set; }
        public string tbMvToplamOySayisi { get; set; }
        public string tbMvItirazsizSonucSayisi { get; set; }
        public string tbMvKesinSonucSayisi { get; set; }
        public string tbMvBaskanSayisi { get; set; }
        public string tbMvBaskanVekiliSayisi { get; set; }
        public string tbMvUyeSayisi { get; set; }
        public string tbMvKatiplerSayisi { get; set; }
        public string btnSikayetEt { get; internal set; }
        public string btnMV { get; internal set; }
        public string btnCb { get; internal set; }
        public string lblIlIlceBaslik { get; internal set; }
        public string lblOzetIlIlce { get; internal set; }
        public string lblMvOzetSandikAlani { get; internal set; }
        public string lblMvGelisZamani { get; internal set; }
    }
}
