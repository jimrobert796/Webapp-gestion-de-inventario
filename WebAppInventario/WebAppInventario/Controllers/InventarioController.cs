using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppInventario.Models;

namespace WebAppInventario.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    // RECUERA ESTE ES UNICO PARA PRECIO COSTO CANTIDAD Y ACTUALIZACION
    public class InventarioController : ControllerBase
    {
        private readonly MyDbContext _context;

        public InventarioController(MyDbContext context)
        {
            _context = context;
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
                idProducto = i.idProducto,
                i.precio,
                i.costo,
                i.cantidad,
                i.ubicacion,
                i.ultimaActualizacion,
                productoNombre = i.Producto != null ? i.Producto.nombre : "Sin nombre",
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
            idProducto = i.idProducto,  // ¡Incluido explícitamente!
            i.precio,
            i.costo,
            i.cantidad,
            i.ubicacion,
            i.ultimaActualizacion,
            productoNombre = i.Producto != null ? i.Producto.nombre : "Sin nombre",
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
