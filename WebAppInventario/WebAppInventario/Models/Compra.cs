using System.ComponentModel.DataAnnotations;

namespace WebAppInventario.Models
{
    public class Compra
    {
        [Key]
        public int idCompra { get; set; }
        public int idEmpleado { get; set; }
        public int numeroCompra { get; set; }
        public DateOnly fechaCompra { get; set; }
        public TimeOnly horaCompra { get; set; }

        public decimal total { get; set; }

        // relaciones 
        public Empleado? Empleado { get; set; }

        public ICollection<CompraDetalle>? ComprasDetalles { get; set; }
    }
}
