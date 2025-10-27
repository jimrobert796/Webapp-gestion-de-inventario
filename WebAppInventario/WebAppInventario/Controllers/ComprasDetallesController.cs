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
    public class ComprasDetallesController : ControllerBase
    {
        private readonly MyDbContext _context;

        public ComprasDetallesController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/ComprasDetalles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompraDetalle>>> GetComprasDetalles()
        {
            var detalles = await _context.ComprasDetalles
        .Include(cd => cd.Inventario)
            .ThenInclude(i => i.Producto)
                .ThenInclude(p => p.Proveedor)
        .Include(cd => cd.Compra)
            .ThenInclude(c => c.Empleado)
        .Select(cd => new
        {
            cd.idCompraDetalle,
            cd.idCompra,
            cd.idInventario,
            cd.Compra.numeroCompra,
            cd.cantidad,
            cd.precio,
            cd.subtotal,
            productoNombre = cd.Inventario.Producto != null ? cd.Inventario.Producto.nombre : "Sin producto",
            proveedorNombre = cd.Inventario.Producto != null && cd.Inventario.Producto.Proveedor != null
                             ? cd.Inventario.Producto.Proveedor.nombre
                             : "Sin proveedor",
            empleadoNombre = cd.Compra != null && cd.Compra.Empleado != null
                             ? cd.Compra.Empleado.nombre
                             : "Sin empleado"
        })
        .ToListAsync();

            return Ok(detalles);
        }




        // GET: api/ComprasDetalles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CompraDetalle>> GetCompraDetalle(int id)
        {
            var detalle = await _context.ComprasDetalles
        .Include(cd => cd.Inventario)
            .ThenInclude(i => i.Producto)
                .ThenInclude(p => p.Proveedor)
        .Include(cd => cd.Compra)
            .ThenInclude(c => c.Empleado)
        .Where(cd => cd.idCompraDetalle == id)
        .Select(cd => new
        {
            cd.idCompraDetalle,
            cd.idCompra,
            cd.idInventario,
            cd.Compra.numeroCompra,
            cd.cantidad,
            cd.precio,
            cd.subtotal,
            productoNombre = cd.Inventario.Producto != null ? cd.Inventario.Producto.nombre : "Sin producto",
            proveedorNombre = cd.Inventario.Producto != null && cd.Inventario.Producto.Proveedor != null
                             ? cd.Inventario.Producto.Proveedor.nombre
                             : "Sin proveedor",
            empleadoNombre = cd.Compra != null && cd.Compra.Empleado != null
                             ? cd.Compra.Empleado.nombre
                             : "Sin empleado"
        })
        .FirstOrDefaultAsync();

    if (detalle == null)
        return NotFound();

    return Ok(detalle);
        }

        // PUT: api/ComprasDetalles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompraDetalle(int id, CompraDetalle compraDetalle)
        {
            if (id != compraDetalle.idCompraDetalle)
            {
                return BadRequest();
            }

            _context.Entry(compraDetalle).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompraDetalleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ComprasDetalles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CompraDetalle>> PostCompraDetalle(CompraDetalle compraDetalle)
        {
            _context.ComprasDetalles.Add(compraDetalle);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCompraDetalle", new { id = compraDetalle.idCompraDetalle }, compraDetalle);
        }

        // DELETE: api/ComprasDetalles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompraDetalle(int id)
        {
            var compraDetalle = await _context.ComprasDetalles.FindAsync(id);
            if (compraDetalle == null)
            {
                return NotFound();
            }

            _context.ComprasDetalles.Remove(compraDetalle);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CompraDetalleExists(int id)
        {
            return _context.ComprasDetalles.Any(e => e.idCompraDetalle == id);
        }
    }
}
