using System.ComponentModel.DataAnnotations;

namespace WebAppInventario.Models
{
    public class Rol
    {
        [Key]
        public int idRol { get; set; }
        public string rol { get; set; }
  
        public ICollection<Empleado>? Empleado { get; set; }
        
    }
}
