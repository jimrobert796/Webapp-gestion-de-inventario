namespace WebAppInventario.Models
{
    public class Dashboard
    {
        public int productosTotales { get; set; }
        public decimal valorInventario { get; set; }
        public int stockBajo { get; set; }
        public int facturasHoy { get; set; }
        public decimal ventasHoy { get; set; }
        public decimal comprasMes { get; set; }
        public int comprasRealizadasMes { get; set; }
        public int devolucionesMes { get; set; }
        public int proveedores { get; set; }
        public int clientes { get; set; }
        public int empleados { get; set; }
        public int categorias { get; set; }
        public decimal ventasTotalesMes { get; set; }
        public decimal promedioDiario { get; set; }
    }
}
