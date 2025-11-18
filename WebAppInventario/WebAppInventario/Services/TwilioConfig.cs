namespace WebAppInventario.Services
{
    public class TwilioConfig
    {
        public string AccountSid { get; set; } = null!;
        public string AuthToken { get; set; } = null!;
        public string WhatsAppSandboxNumber { get; set; } = null!;
        public string AdminWhatsAppNumber { get; set; } = null!;
        public int StockMinimo { get; set; } // <-- Propiedad añadida
    }
}