using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppInventario.Models
{
    public class Devolucion
    {
        [Key]
        public int idDevolucion { get; set; }

        
        public int idFactura { get; set; }

        public int idEmpleado { get; set; }

        public DateOnly fechaDevolucion { get; set; }

        public TimeOnly horaDevolucion { get; set; }
        
        public int cantidad { get; set; }

        public decimal totalDevolucion { get; set; }

        public Empleado? Empleado { get; set; }
        public Factura? Factura { get; set; }

        public ICollection<DevolucionDetalle>? DevolucionDetalle { get; set; }


    }
}
