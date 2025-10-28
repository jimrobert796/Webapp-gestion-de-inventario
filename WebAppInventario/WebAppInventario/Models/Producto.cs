using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppInventario.Models
{
    public class Producto
    {
        [Key]
        public int idProducto { get; set; }
        public int idProveedor { get; set; }
        public int idCategoria { get; set; }
        
        public string codigo { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public bool estado { get; set; } = true;
        public DateOnly? fechaProd { get; set; }
        public DateOnly? fechaVenc { get; set; }
        // Propiedad de navegación hacia la categoría 
        public Categoria? Categoria { get; set; }
        public Proveedor? Proveedor { get; set; }
        public ICollection<Inventario>? Inventario { get; set; }


    }
}
