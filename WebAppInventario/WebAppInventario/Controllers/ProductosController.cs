using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppInventario.Models;

namespace WebAppInventario.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly MyDbContext _context;

        public ProductosController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/Productoes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            // Incluimos Categoria y Proveedor, luego proyectamos
            var productos = await _context.Productos
                .Where(p => p.estado) // 🔹 solo activos
                .Include(p => p.Categoria)
                .Include(p => p.Proveedor)
                .Select(p => new
                {
                    p.idProducto,
                    p.idProveedor,
                    p.idCategoria,
                    p.codigo,
                    p.nombre,
                    p.estado,
                    p.descripcion,
                    p.fechaProd,
                    p.fechaVenc,
                    // Mapeamos 'categoria' al nombre de la categoría (string)
                    categoria = p.Categoria != null ? p.Categoria.nombre : "Sin categoría",  // Asumiendo que Categoria tiene 'nombre'
                    // Mapeamos 'proveedor' al nombre del proveedor (string)
                    proveedor = p.Proveedor != null ? p.Proveedor.nombre : "Sin proveedor"  // Asumiendo que Proveedor tiene 'nombre'
                })
                .ToListAsync();
            return Ok(productos);
            /* ESTRUCTURA 
             [
              {
                "idProducto": 7,
                "idProveedor": 3,
                "idCategoria": 2,
                "codigo": "P102      ",
                "nombre": "Pintura Acrílica Mate                             ",
                "estado": true,
                "descripcion": "Pintura acrílica de acabado mate para interiores                                                                                                                                                        ",
                "fechaProd": "2025-10-25",
                "fechaVenc": "2028-10-25",
                "categoria": "Herramientas                                      ",
                "proveedor": "Comex                                             "
              }
            ]
             */

        }

        // GET: api/Productos/nuevo-Codigo
        [HttpGet("nuevo-Codigo")]
        public async Task<ActionResult<string>> GetNuevoCodigo()
        {
            // Busca el último código existente
            var ultimoCodigo = await _context.Productos
                .OrderByDescending(p => p.codigo)
                .Select(p => p.codigo)
                .FirstOrDefaultAsync();

            string nuevoCodigo;

            if (string.IsNullOrEmpty(ultimoCodigo))
            {
                // Si no hay productos aún
                nuevoCodigo = "PRD000001";
            }
            else
            {
                // Extrae la parte numérica del código (por ejemplo, de "PRD000056" -> 56)
                int numero = int.Parse(ultimoCodigo.Substring(3));
                // Incrementa el número
                numero++;
                // Genera el nuevo código con ceros a la izquierda
                nuevoCodigo = $"PRD{numero:D6}";
            }

            return Ok(nuevoCodigo);
        }

        // GET: api/Productos/buscar?buscar={TEXTO}
        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<Producto>>> BuscarProductos([FromQuery] ProductoBusquedaParametros parametros)
        {
            var consulta = _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Proveedor)
                .Where(p => p.estado) // Solo productos activos
                .AsQueryable();

            if (!string.IsNullOrEmpty(parametros.buscar))
            {
                var texto = parametros.buscar.Trim().ToLower();

                consulta = consulta.Where(p =>
                    (p.nombre != null && p.nombre.ToLower().Contains(parametros.buscar)) ||
                    (p.codigo != null && p.codigo.ToLower().Contains(parametros.buscar)) ||
                    (p.Categoria != null && p.Categoria.nombre.ToLower().Contains(parametros.buscar)) ||
                    (p.Proveedor != null && p.Proveedor.nombre.ToLower().Contains(parametros.buscar))
                );
            }

            var productos = await consulta
                .Select(p => new
                {
                    p.idProducto,
                    p.idProveedor,
                    p.idCategoria,
                    p.codigo,
                    p.nombre,
                    p.estado,
                    p.descripcion,
                    p.fechaProd,
                    p.fechaVenc,
                    categoria = p.Categoria != null ? p.Categoria.nombre : "Sin categoría",
                    proveedor = p.Proveedor != null ? p.Proveedor.nombre : "Sin proveedor"
                })
                .ToListAsync();

            return Ok(productos);
        }



        // GET: api/Productoes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetProducto(int id)
        {
            var producto = await _context.Productos
              .Include(p => p.Categoria)
              .Include(p => p.Proveedor)
              .Where(p => p.idProducto == id && p.estado)
              .Select(p => new
              {
                  p.idProducto,
                  p.idProveedor,
                  p.idCategoria,
                  p.codigo,
                  p.nombre,
                  p.estado,
                  p.descripcion,
                  p.fechaProd,
                  p.fechaVenc,
                  categoria = p.Categoria != null ? p.Categoria.nombre : "Sin categoría",
                  proveedor = p.Proveedor != null ? p.Proveedor.nombre : "Sin proveedor"
              })
              .FirstOrDefaultAsync();
            if (producto == null)
            {
                return NotFound();
            }
            return Ok(producto);

            /* ESTRUCTURA 
             [
              {
                "idProducto": 7,
                "idProveedor": 3,
                "idCategoria": 2,
                "codigo": "P102      ",
                "nombre": "Pintura Acrílica Mate                             ",
                "estado": true,
                "descripcion": "Pintura acrílica de acabado mate para interiores                                                                                                                                                        ",
                "fechaProd": "2025-10-25",
                "fechaVenc": "2028-10-25",
                "categoria": "Herramientas                                      ",
                "proveedor": "Comex                                             "
              }
            ]
             */

        }

        // PUT: api/Productoes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProducto(int id, Producto producto)
        {
            if (id != producto.idProducto)
            {
                return BadRequest();
            }

            _context.Entry(producto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();

            /* Estructura 
             {
              "idProducto": 1,
              "idProveedor": 2,
              "idCategoria": 2,
              "codigo": "P101",
              "nombre": "Barniz Copal V81 Actualizado",
              "descripcion": "Barniz para acabado brillante",
              "estado": true,
              "fechaProd": "2025-10-24",
              "fechaVenc": "2027-10-24"
            }
             */
        }

        // POST: api/Productoes SOFT DELLETE
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Producto>> PostProducto(Producto producto)
        {
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProducto", new { id = producto.idProducto }, producto);
        }

        // DELETE: api/Productoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound();
            }

            // 🔹 Soft delete: solo cambiar estado
            producto.estado = false;

            _context.Entry(producto).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.idProducto == id);
        }
    }
}
