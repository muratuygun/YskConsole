namespace YskConsole.Models
{
    public class CumgurbaskanligiSecimBilgileri : AspNetBaseCrawler
    {
        public string TcKimlikNo { get; set; }

        public string Il { get; set; }
        public string Ilce { get; set; }
        public string SandikNo { get; set; }

        public string btnMV { get; set; }
        public string btnCb { get; set; }
        public string txtCbKayitliSecmen { get; set; }
        public string txtCbOyKullanan { get; set; }
        public string txtCbKanunGeregi { get; set; }
        public string txtCbKullanilanToplamOy { get; set; }
        public string txtCbItirazsizligecerli { get; set; }
        public string txtCbItirazligecerli { get; set; }
        public string txtCbGecerliOy { get; set; }
        public string txtCbGercersizOy { get; set; }
        public string txtCB1 { get; set; }
        public string txtCB2 { get; set; }
        public string txtCB3 { get; set; }
        public string txtCB4 { get; set; }
        public string txtAciklama { get; set; }
        public string txtAdSoyad { get; set; }
        public string txtTelefon { get; set; }
        public string txtEPosta { get; set; }
        public string btnSikayetEt { get; set; }
        public string KalanGoster { get; set; }


        public string lblCbIlIlceBaslik { get; internal set; }
        public string lblCbSandikAlani { get; internal set; }
        public string lblCbYskZamani { get; internal set; }
    }
}
