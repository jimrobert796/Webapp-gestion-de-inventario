using System.ComponentModel.DataAnnotations;

namespace WebAppInventario.Models
{
    public class Proveedor
    {
        [Key]
        public int idProveedor { get; set; }
        public string nombre { get; set; }
        public string telefono { get; set; }
        public string email { get; set; }
        public string direccion { get; set; }
        public bool estado { get; set; } = true; // true = activo, false = inactivo


        public ICollection<Producto>? Productos { get; set; }
    }
}
