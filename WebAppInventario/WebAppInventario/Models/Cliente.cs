using System.ComponentModel.DataAnnotations;

namespace WebAppInventario.Models
{
    public class Cliente
    {
        [Key]
        public int idCliente { get; set; }
        public string nombre { get; set; }
        public string email { get; set; }
        public string direccion { get; set; }
        public string telefono { get; set; }
        public DateTime creaccion { get; set; }
        public bool estado { get; set; } = true;

        
        public ICollection<Factura>? Facturas { get; set; }
    }
}
