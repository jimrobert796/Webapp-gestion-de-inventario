using System.ComponentModel.DataAnnotations;

namespace WebAppInventario.Models
{
    public class FacturaDetalle
    {
        [Key]
        public int idFacturaDetalle { get; set; }
        public int idFactura { get; set; }
        public int idInventario { get; set; }
        public int cantidad { get; set; }
        public decimal precio { get; set; }
        public decimal subtotal { get; set; }

        public Factura? Facturas { get; set; }
        public Inventario? inventario { get; set; }  // Corregido
    }
}
