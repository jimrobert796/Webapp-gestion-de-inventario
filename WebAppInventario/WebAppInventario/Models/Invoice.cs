using System.Collections.Generic;

namespace FacturacionElectronica.Models
{
    public class Invoice
    {
        public string InvoiceNumber { get; set; } = string.Empty;
        public Customer Customer { get; set; } = new Customer();
        public List<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
        public decimal Subtotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public string Currency { get; set; } = "USD";
    }
}
