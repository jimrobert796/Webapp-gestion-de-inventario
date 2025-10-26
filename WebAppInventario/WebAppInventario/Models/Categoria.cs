using System.ComponentModel.DataAnnotations;

namespace WebAppInventario.Models
{
    public class Categoria
    {
        [Key]
        public int idCategoria { get; set; }
        public string nombre { get; set; }
        // Una categoría tiene muchos productos
        public ICollection<Producto>? Productos { get; set; }

    }
}
