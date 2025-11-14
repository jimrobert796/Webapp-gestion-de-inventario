namespace WebAppInventario.Models
{
    public class ultimaFactura
    {
        public DateOnly fecha { get; set; }
        public string numeroFactura { get; set; }
        public decimal total { get; set; }
    }
}
