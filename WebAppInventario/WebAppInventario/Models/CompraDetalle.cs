using System.ComponentModel.DataAnnotations;

namespace WebAppInventario.Models
{
    public class CompraDetalle
    {
        [Key]
        public int idCompraDetalle { get; set; }
        public int idCompra { get; set; }
        public int idInventario { get; set; }
        
        public int cantidad { get; set; }
        public decimal precio { get; set; }
        public decimal costo { get; set; }
        public decimal subtotal { get; set; }


        // Viajar a esos datos
        public Inventario? Inventario { get; set; } // Nombre de producto y id para poder aumentar su stock en el inventario
        public Compra? Compra { get; set; }
    }
}
