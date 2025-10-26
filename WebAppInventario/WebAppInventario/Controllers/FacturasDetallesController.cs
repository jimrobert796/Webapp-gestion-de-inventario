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
    public class FacturasDetallesController : ControllerBase
    {
        private readonly MyDbContext _context;

        public FacturasDetallesController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/FacturasDetalles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FacturaDetalle>>> GetFacturasDetalles()
        {
            var facturasDetalles = await _context.FacturasDetalles
            .Include(fd => fd.Facturas)
                .ThenInclude(f => f.Cliente)
            .Include(fd => fd.Facturas)
                .ThenInclude(f => f.Empleado)
            .Include(fd => fd.inventario)
                .ThenInclude(i => i.Producto)
            .Select(fd => new
            {
                fd.idFacturaDetalle,
                fd.idFactura,
                fd.idInventario,
                fd.Facturas.numeroFactura,
                idCliente = fd.Facturas.idCliente != null ? fd.Facturas.idCliente : null,
                clienteNombre = fd.Facturas.Cliente.nombre != null ? fd.Facturas.Cliente.nombre : "Sin cliente",
                empleadoNombre = fd.Facturas.Empleado.nombre != null ? fd.Facturas.Empleado.nombre : "Sin Empleados",
                productoNombre = fd.inventario.Producto.nombre != null ? fd.inventario.Producto.nombre : "Sin Nombre",
                fd.precio,
                fd.cantidad,
                fd.subtotal,
                fd.Facturas.fecha,
                fd.Facturas.hora
            })
            .ToListAsync();

            return Ok(facturasDetalles);
        }

        // GET: api/FacturasDetalles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FacturaDetalle>> GetFacturaDetalle(int id)
        {
            var facturaDetalle = await _context.FacturasDetalles
            .Include(fd => fd.Facturas)
                .ThenInclude(f => f.Cliente)
            .Include(fd => fd.Facturas)
                .ThenInclude(f => f.Empleado)
            .Include(fd => fd.inventario)
                .ThenInclude(i => i.Producto)
            .Where(fd => fd.idFacturaDetalle == id)
            .Select(fd => new
            {
                fd.idFacturaDetalle,
                fd.idFactura,
                fd.idInventario,
                fd.Facturas.numeroFactura,
                idCliente = fd.Facturas.idCliente != null ? fd.Facturas.idCliente : null,
                clienteNombre = fd.Facturas.Cliente.nombre != null ? fd.Facturas.Cliente.nombre : "Sin cliente",
                empleadoNombre = fd.Facturas.Empleado.nombre != null ? fd.Facturas.Empleado.nombre : "Sin Empleados",
                productoNombre = fd.inventario.Producto.nombre != null ? fd.inventario.Producto.nombre : "Sin Nombre",
                fd.precio,
                fd.cantidad,
                fd.subtotal,
                fd.Facturas.fecha,
                fd.Facturas.hora
            })
            .FirstOrDefaultAsync();

            if (facturaDetalle == null)
                return NotFound();

            return Ok(facturaDetalle);
        }

        // PUT: api/FacturasDetalles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFacturaDetalle(int id, FacturaDetalle facturaDetalle)
        {
            if (id != facturaDetalle.idFacturaDetalle)
            {
                return BadRequest();
            }

            _context.Entry(facturaDetalle).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FacturaDetalleExists(id))
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

        // POST: api/FacturasDetalles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<FacturaDetalle>> PostFacturaDetalle(FacturaDetalle facturaDetalle)
        {
            _context.FacturasDetalles.Add(facturaDetalle);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFacturaDetalle", new { id = facturaDetalle.idFacturaDetalle }, facturaDetalle);
        }

        // DELETE: api/FacturasDetalles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFacturaDetalle(int id)
        {
            var facturaDetalle = await _context.FacturasDetalles.FindAsync(id);
            if (facturaDetalle == null)
            {
                return NotFound();
            }

            _context.FacturasDetalles.Remove(facturaDetalle);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FacturaDetalleExists(int id)
        {
            return _context.FacturasDetalles.Any(e => e.idFacturaDetalle == id);
        }
    }
}
