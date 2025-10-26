using System.ComponentModel.DataAnnotations;

namespace WebAppInventario.Models
{
    public class Empleado
    {
        [Key]
        public int idEmpleado { get; set; }
        public int idRol { get; set; }

        
        public string nombre { get; set; }
        public string contraseña { get; set; }
        public string credencial { get; set; }
        public string telefono { get; set; }
        public string email { get; set; }
        public string direccion { get; set; }
        public DateOnly fechaNacimiento { get; set; }
        public bool estado { get; set; } = true; // true = activo, false = inactivo

        public ICollection<Compra>? Compras { get; set; }
        public ICollection<Factura>? Facturas { get; set; }

        // Propiedad de navegación hacia la categoría
        public Rol? Rol { get; set; }

        



    }
}
