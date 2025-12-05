namespace FacturacionElectronica.Models
{
    public class SmtpSettings
    {
        public string SmtpServer { get; set; } = string.Empty;
        public int SmtpPort { get; set; }
        public string SenderEmail { get; set; } = string.Empty;
        public string SenderPassword { get; set; } = string.Empty;
        public bool UseSsl { get; set; } = true;
    }
}
