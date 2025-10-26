using System.ComponentModel.DataAnnotations;

namespace WebAppInventario.Models
{
    public class Factura
    {
        [Key]
        public int idFactura { get; set; }
        public int idEmpleado { get; set; }
        public int? idCliente { get; set; }  
        public int numeroFactura { get; set; }
        public decimal subtotal { get; set; }
        public decimal total { get; set; }

        public decimal iva { get; set; }
        public string metodoPago { get; set; }

        public DateOnly fecha { get; set; }
        public TimeOnly hora { get; set; }

        // Relaciones
        public ICollection<FacturaDetalle>? FacturaDetalles { get; set; }

        public Cliente? Cliente { get; set; }
        public Empleado? Empleado { get; set; }
    }
}
