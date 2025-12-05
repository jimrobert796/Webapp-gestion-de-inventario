using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twilio.TwiML.Voice;
using WebAppInventario.Models;
using WebAppInventario.Services;
using Microsoft.Extensions.Options; // <-- 2. AÑADIR ESTE 'USING'

namespace WebAppInventario.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    // RECUERA ESTE ES UNICO PARA PRECIO COSTO CANTIDAD Y ACTUALIZACION
    public class InventarioController : ControllerBase
    {
        private readonly MyDbContext _context;
        private readonly WhatsAppService _whatsAppService;
        // 3. AÑADIR EL CAMPO PRIVADO PARA LA CONFIGURACIÓN
        private readonly TwilioConfig _config;

        public InventarioController(MyDbContext context,
                                  WhatsAppService whatsAppService,
                                  IOptions<TwilioConfig> config)
        {
            _context = context;
            _whatsAppService = whatsAppService;
            _config = config.Value; // Asignar la configuración
        }

        // GET: api/Inventarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Inventario>>> GetInventarios()
        {
            var inventarios = await _context.Inventario
            .Include(i => i.Producto)
            .ThenInclude(p => p.Categoria)
            .Include(i => i.Producto)
            .ThenInclude(p => p.Proveedor)
            .Where(i => i.Producto != null && i.Producto.estado) // <-- solo activos
            .Select(i => new
            {
                i.idInventario,
                i.idProducto,
                idCategoria = i.Producto != null ? i.Producto.idCategoria : (int?)null,
                idProveedor = i.Producto != null ? i.Producto.idProveedor : (int?)null,
                i.precio,
                i.costo,
                i.cantidad,
                i.ubicacion,
                i.ultimaActualizacion,
                productoNombre = i.Producto != null ? i.Producto.nombre : "Sin nombre",
                productoDescripcion = i.Producto != null ? i.Producto.descripcion : "Sin descripcion",
                productoCodigo = i.Producto != null ? i.Producto.codigo : "Sin código",
                productoEstado = i.Producto != null ? i.Producto.estado : false,
                productoFechaProd = i.Producto != null ? i.Producto.fechaProd : (DateOnly?)null,
                productoFechaVenc = i.Producto != null ? i.Producto.fechaVenc : (DateOnly?)null,
                Productocategoria = i.Producto != null && i.Producto.Categoria != null ? i.Producto.Categoria.nombre : "Sin categoría",
                Productoproveedor = i.Producto != null && i.Producto.Proveedor != null ? i.Producto.Proveedor.nombre : "Sin proveedor"
            })
            .ToListAsync();

            return Ok(inventarios);

        }
        /* Estructura Get 
         
         [
          {
            "idInventario": 1,
            "idProducto": 1,
            "precio": 12.5,
            "costo": 9,
            "cantidad": 450,
            "ubicacion": "Pasillo 15                                                                                                                                                                                              ",
            "ultimaActualizacion": "2025-10-24",
            "productoNombre": "Barniz Copal V81 Actualizado                      ",
            "productoCodigo": "P101      ",
            "productoEstado": true,
            "productoFechaProd": "2025-10-24",
            "productoFechaVenc": "2027-10-24",
            "categoria": "Herramientas                                      ",
            "proveedor": "Sherwin-Williams\r\n                                "
          }
        ]
        */

        [HttpGet("filtrar-anidado")]
        public async Task<IActionResult> FiltrarInventario(
         int? categoriaId,
         int? proveedorId,
         string? buscar,
         string? stock) // 👈 nuevo filtro
        {
            var query = _context.Inventario
                .Include(i => i.Producto)
                    .ThenInclude(p => p.Categoria)
                .Include(i => i.Producto.Proveedor)
                .Where(i => i.Producto != null && i.Producto.estado)
                .AsQueryable();

            // 📌 Filtro por categoría
            if (categoriaId.HasValue)
                query = query.Where(i => i.Producto.idCategoria == categoriaId.Value);

            // 📌 Filtro por proveedor
            if (proveedorId.HasValue)
                query = query.Where(i => i.Producto.idProveedor == proveedorId.Value);

            // 📌 Filtro general
            if (!string.IsNullOrWhiteSpace(buscar))
            {
                string b = buscar.Trim().ToLower();
                query = query.Where(i =>
                    i.Producto.nombre.ToLower().Contains(b) ||
                    i.Producto.codigo.ToLower().Contains(b) ||
                    i.Producto.descripcion.ToLower().Contains(b) ||
                    i.Producto.Categoria.nombre.ToLower().Contains(b) ||
                    i.Producto.Proveedor.nombre.ToLower().Contains(b)
                );
            }

            // 📌 Filtro por stock
            if (!string.IsNullOrWhiteSpace(stock))
            {
                switch (stock.ToLower().Trim())
                {
                    case "minimo":
                        query = query.Where(i => i.cantidad < 20);
                        break;

                    case "moderado":
                        query = query.Where(i => i.cantidad >= 20 && i.cantidad < 100);
                        break;

                    case "normal":
                        query = query.Where(i => i.cantidad >= 100 && i.cantidad <= 400);
                        break;

                    case "maximo":
                        query = query.Where(i => i.cantidad > 400 && i.cantidad <= 500);
                        break;
                }
            }

            var resultado = await query
                .Select(i => new {
                    i.idInventario,
                    i.idProducto,
                    idCategoria = i.Producto.idCategoria,
                    idProveedor = i.Producto.idProveedor,
                    i.precio,
                    i.costo,
                    i.cantidad,
                    i.ubicacion,
                    i.ultimaActualizacion,

                    productoNombre = i.Producto.nombre.Trim(),
                    productoDescripcion = i.Producto.descripcion.Trim(),
                    productoCodigo = i.Producto.codigo.Trim(),
                    productoEstado = i.Producto.estado,
                    productoFechaProd = i.Producto.fechaProd,
                    productoFechaVenc = i.Producto.fechaVenc,

                    productocategoria = i.Producto.Categoria.nombre.Trim(),
                    productoproveedor = i.Producto.Proveedor.nombre.Trim()
                })
                .OrderBy(i => i.productoNombre)
                .ToListAsync();

            return Ok(resultado);
        }






        // GET: api/Inventario/buscar-cajero?buscar=martillo
        [HttpGet("buscar-cajero")]
        public async Task<ActionResult<IEnumerable<object>>> BuscarProductosCajero([FromQuery] InventarioBusquedaParametros parametros)
        {
            var consulta = _context.Inventario
                .Include(i => i.Producto)
                    .ThenInclude(p => p.Categoria)
                .Include(i => i.Producto)
                    .ThenInclude(p => p.Proveedor)
                .Where(i => i.Producto != null && i.Producto.estado && i.cantidad > 0) // Solo productos activos y con stock
                .AsQueryable();

            if (!string.IsNullOrEmpty(parametros.buscar))
            {
                string texto = parametros.buscar.ToLower();
                consulta = consulta.Where(i =>
                    (i.Producto.nombre != null && i.Producto.nombre.ToLower().Contains(texto)) ||
                    (i.Producto.codigo != null && i.Producto.codigo.ToLower().Contains(texto)) ||
                    (i.Producto.Categoria != null && i.Producto.Categoria.nombre.ToLower().Contains(texto)) ||
                    (i.Producto.Proveedor != null && i.Producto.Proveedor.nombre.ToLower().Contains(texto))
                );
            }

            var productos = await consulta
                .Select(i => new
                {
                    i.idInventario,
                    productoNombre = i.Producto != null ? i.Producto.nombre : "Sin nombre",
                    productoCodigo = i.Producto != null ? i.Producto.codigo : "Sin código",
                    i.precio,
                    i.cantidad,
                    categoria = i.Producto != null && i.Producto.Categoria != null ? i.Producto.Categoria.nombre : "Sin categoría",
                    proveedor = i.Producto != null && i.Producto.Proveedor != null ? i.Producto.Proveedor.nombre : "Sin proveedor"
                })
                .ToListAsync();

            return Ok(productos);
        }



        // GET: api/Inventario/compra-proveedor
        [HttpGet("compra-proveedor")]
        public async Task<ActionResult<IEnumerable<object>>> BuscarProductosPorProveedor(
            [FromQuery] int idProveedor,
            [FromQuery] string? buscar)
        {
            var consulta = _context.Inventario
                .Include(i => i.Producto)
                    .ThenInclude(p => p.Categoria)
                .Include(i => i.Producto)
                .Where(i =>
                    i.Producto != null &&
                    i.Producto.estado &&
                    i.Producto.idProveedor == idProveedor &&
                    i.cantidad > 0 && i.cantidad < 500
                    )
                .AsQueryable();

            if (!string.IsNullOrEmpty(buscar))
            {
                string texto = buscar.ToLower();
                consulta = consulta.Where(i =>
                    (i.Producto.nombre != null && i.Producto.nombre.ToLower().Contains(texto)) ||
                    (i.Producto.codigo != null && i.Producto.codigo.ToLower().Contains(texto))
                );
            }

            var productos = await consulta
                .Select(i => new
                {
                    i.idInventario,
                    productoNombre = i.Producto.nombre,
                    productoCodigo = i.Producto.codigo,
                    i.precio,
                    i.costo,
                    i.cantidad,
                    categoria = i.Producto.Categoria.nombre,
                    proveedor = i.Producto.Proveedor.nombre
                })
                .ToListAsync();

            return Ok(productos);
        }





        // PUT: api/Inventario/reducir-stock/5?cantidad=3
        [HttpPut("reducir-stock/{id}")]
        public async Task<IActionResult> ReducirStock(int id, [FromQuery] int cantidad)
        {
            if (cantidad <= 0)
                return BadRequest("La cantidad a reducir debe ser mayor que cero.");

            var inventario = await _context.Inventario.FindAsync(id);
            if (inventario == null)
                return NotFound("No se encontró el producto en inventario.");

            if (inventario.cantidad < cantidad)
                return BadRequest("No hay suficiente stock disponible.");

            inventario.cantidad -= cantidad;

            // --- 5. LÓGICA DE ALERTA AÑADIDA ---

            // Leemos el umbral desde appsettings.json
            int stockMinimo = _config.StockMinimo;

            if (inventario.cantidad < stockMinimo)
            {
                try
                {
                    var producto = await _context.Productos.FindAsync(inventario.idProducto);
                    string nombreProducto = producto?.nombre ?? "Producto Desconocido";

                    _whatsAppService.EnviarAlertaStockBajo(
                        nombreProducto.Trim(),
                        inventario.cantidad
                    );
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al enviar alerta de WhatsApp: {ex.Message}");
                }
            }
            // --- FIN DE LA LÓGICA AÑADIDA ---

            // Actualizar fecha de última modificación
            inventario.ultimaActualizacion = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok(new
            {
                mensaje = "Stock reducido correctamente.",
                inventario.idInventario,
                inventario.cantidad,
                inventario.ultimaActualizacion
            });
        }





        // PUT: api/Inventario/aumentar-stock/5?cantidad=10&nuevoPrecio=15.50&nuevoCosto=12.00
        [HttpPut("aumentar-stock/{id}")]
        public async Task<IActionResult> AumentarStock(int id,
            [FromQuery] int cantidad,
            [FromQuery] decimal? nuevoPrecio = null,
            [FromQuery] decimal? nuevoCosto = null)
        {
            if (cantidad <= 0)
                return BadRequest("La cantidad a aumentar debe ser mayor que cero.");

            var inventario = await _context.Inventario.FindAsync(id);
            if (inventario == null)
                return NotFound("No se encontró el producto en inventario.");

            inventario.cantidad += cantidad;

            // Actualizar precio si se proporciona
            if (nuevoPrecio.HasValue)
                inventario.precio = nuevoPrecio.Value;

            // Actualizar costo si se proporciona
            if (nuevoCosto.HasValue)
                inventario.costo = nuevoCosto.Value;

            inventario.ultimaActualizacion = DateTime.Now;


            await _context.SaveChangesAsync();
            return Ok(new
            {
                mensaje = "Stock aumentado correctamente.",
                inventario.idInventario,
                inventario.cantidad,
                inventario.precio,
                inventario.costo
            });
        }






        // Busqueda o consulta unicamente por nombre, proveedor, categoria o codigo de producto 
        // GET: api/Inventario/buscar?buscar={TEXTO}
        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<Inventario>>> BuscarInventario([FromQuery] InventarioBusquedaParametros parametros)
        {
            var consulta = _context.Inventario
        .Include(i => i.Producto)
            .ThenInclude(p => p.Categoria)
        .Include(i => i.Producto)
            .ThenInclude(p => p.Proveedor)
        .Where(i => i.Producto != null && i.Producto.estado) // Solo productos activos
        .AsQueryable();

            if (!string.IsNullOrEmpty(parametros.buscar))
            {

                consulta = consulta.Where(i =>
                    (i.Producto.nombre != null && i.Producto.nombre.ToLower().Contains(parametros.buscar)) ||
                    (i.Producto.codigo != null && i.Producto.codigo.ToLower().Contains(parametros.buscar)) ||
                    (i.Producto.Categoria != null && i.Producto.Categoria.nombre.ToLower().Contains(parametros.buscar)) ||
                    (i.Producto.Proveedor != null && i.Producto.Proveedor.nombre.ToLower().Contains(parametros.buscar))
                );
            }

            var inventarios = await consulta
                .Select(i => new
                {
                    i.idInventario,
                    i.idProducto,
                    idCategoria = i.Producto != null ? i.Producto.idCategoria : (int?)null,
                    idProveedor = i.Producto != null ? i.Producto.idProveedor : (int?)null,
                    i.precio,
                    i.costo,
                    i.cantidad,
                    i.ubicacion,
                    i.ultimaActualizacion,
                    productoNombre = i.Producto != null ? i.Producto.nombre : "Sin nombre",
                    productoDescripcion = i.Producto != null ? i.Producto.descripcion : "Sin descripción",
                    productoCodigo = i.Producto != null ? i.Producto.codigo : "Sin código",
                    productoEstado = i.Producto != null ? i.Producto.estado : false,
                    productoFechaProd = i.Producto != null ? i.Producto.fechaProd : (DateOnly?)null,
                    productoFechaVenc = i.Producto != null ? i.Producto.fechaVenc : (DateOnly?)null,
                    Productocategoria = i.Producto != null && i.Producto.Categoria != null ? i.Producto.Categoria.nombre : "Sin categoría",
                    Productoproveedor = i.Producto != null && i.Producto.Proveedor != null ? i.Producto.Proveedor.nombre : "Sin proveedor"
                })
                .ToListAsync();

            return Ok(inventarios);
        }

        // GET: api/Inventarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Inventario>> GetInventario(int id)
        {
            var inventario = await _context.Inventario
        .Include(i => i.Producto)
        .ThenInclude(p => p.Categoria)
        .Include(i => i.Producto)
        .ThenInclude(p => p.Proveedor)

        .Where(i => i.idInventario == id && i.Producto != null && i.Producto.estado) // <-- solo activos
        .Select(i => new
        {
            i.idInventario,
            i.idProducto,
            idCategoria = i.Producto != null ? i.Producto.idCategoria : (int?)null,
            idProveedor = i.Producto != null ? i.Producto.idProveedor : (int?)null,
            i.precio,
            i.costo,
            i.cantidad,
            i.ubicacion,
            i.ultimaActualizacion,
            productoNombre = i.Producto != null ? i.Producto.nombre : "Sin nombre",
            productoDescripcion = i.Producto != null ? i.Producto.descripcion : "Sin descripcion",
            productoCodigo = i.Producto != null ? i.Producto.codigo : "Sin código",
            productoEstado = i.Producto != null ? i.Producto.estado : false,
            productoFechaProd = i.Producto != null ? i.Producto.fechaProd : (DateOnly?)null,
            productoFechaVenc = i.Producto != null ? i.Producto.fechaVenc : (DateOnly?)null,
            Productocategoria = i.Producto != null && i.Producto.Categoria != null ? i.Producto.Categoria.nombre : "Sin categoría",
            Productoproveedor = i.Producto != null && i.Producto.Proveedor != null ? i.Producto.Proveedor.nombre : "Sin proveedor"
        })
        .FirstOrDefaultAsync();
            if (inventario == null)
            {
                return NotFound();
            }
            return Ok(inventario);
        }

        // PUT: api/Inventarios/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInventario(int id, Inventario inventario)
        {
            if (id != inventario.idInventario)
            {
                return BadRequest();
            }

            _context.Entry(inventario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InventarioExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
            /*ESTRUCTURA PARA MODIFICAR 
             {
              "idInventario": 1,
              "idProducto": 1,
              "precio": 12.5,
              "costo": 9.0,
              "cantidad": 450,
              "ubicacion": "Pasillo 15",
              "ultimaActualizacion": "2025-10-24"
            }
            */
        }

        // POST: api/Inventarios
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Inventario>> PostInventario(Inventario inventario)
        {
            _context.Inventario.Add(inventario);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetInventario", new { id = inventario.idInventario }, inventario);
        }

        // DELETE: api/Inventarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventario(int id)
        {
            var inventario = await _context.Inventario.FindAsync(id);
            if (inventario == null)
            {
                return NotFound();
            }

            _context.Inventario.Remove(inventario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool InventarioExists(int id)
        {
            return _context.Inventario.Any(e => e.idInventario == id);
        }
    }
}
