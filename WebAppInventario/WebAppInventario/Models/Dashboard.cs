namespace WebAppInventario.Models
{
    public class Dashboard
    {
        public int ProductosTotales { get; set; }
        public decimal VentasHoy { get; set; }
        public decimal ComprasMes { get; set; }
        public int Clientes { get; set; }
        public int Proveedores { get; set; }
        public int DevolucionesMes { get; set; }
        public int StockBajo { get; set; }
        public int FacturasHoy { get; set; }
        public decimal VentasTotalesMes { get; set; }
        public decimal PromedioDiario { get; set; }
    }
}
