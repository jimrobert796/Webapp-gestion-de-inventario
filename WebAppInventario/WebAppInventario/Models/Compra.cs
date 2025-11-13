using System.ComponentModel.DataAnnotations;

namespace WebAppInventario.Models
{
    public class Compra
    {
        [Key]
        public int idCompra { get; set; }
        public int idEmpleado { get; set; }
        public int idProveedor { get; set; }
        public string numeroCompra { get; set; }
        public DateOnly fechaCompra { get; set; }
        public TimeOnly horaCompra { get; set; }
        public string metodoPago { get; set; }
        public int cantidad { get; set; }
        public decimal total { get; set; }
        public decimal subtotal { get; set; }
        public decimal iva { get; set; }
        public string estado { get; set; }
        public string? motivoAnulacion { get; set; }
        public DateTime? fechaAnulacion { get; set; }
        public int? idEmpleadoAnulacion { get; set; }



        // relaciones 
        public Empleado? Empleado { get; set; }
        public Empleado? EmpleadoAnulacion { get; set; } // 🔹 Esta propiedad es la clave
        public Proveedor? Proveedor { get; set; }

        public ICollection<CompraDetalle>? ComprasDetalles { get; set; }
    }
}
