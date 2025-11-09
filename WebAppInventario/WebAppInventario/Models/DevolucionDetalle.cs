using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppInventario.Models
{
    public class DevolucionDetalle
    {
        [Key]
        public int idDevolucionDetalle { get; set; }
        public int idDevolucion { get; set; }

        public int idFacturaDetalle { get; set; }

        public int cantidadDevuelta { get; set; }

        public string motivo { get; set; } = string.Empty;

        public string descripcion { get; set; } = string.Empty;
        public decimal precioUnitario { get; set; }
        public decimal subtotal { get; set; }
        public bool reintegrarInventario { get; set; }

        // Relaciones
        public Devolucion? Devolucion { get; set; }
        public FacturaDetalle? FacturaDetalle { get; set; }
    }
}
