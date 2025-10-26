using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppInventario.Models
{
    public class Inventario
    {
        [Key]
        public int idInventario { get; set; }
        public int idProducto { get; set; }

        public decimal precio { get; set; }
        public decimal costo { get; set; }
        public int cantidad { get; set; }
        public string ubicacion { get; set; }
        public DateOnly ultimaActualizacion { get; set; }
        public Producto? Producto { get; set; }  // Corregido
        public ICollection<CompraDetalle>? compraDetalles { get; set; }
        public ICollection<FacturaDetalle>? facturaDetalles { get; set; }
    }
}
