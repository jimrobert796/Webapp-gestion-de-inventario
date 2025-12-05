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
    public class DevolucionesDetallesController : ControllerBase
    {
        private readonly MyDbContext _context;

        public DevolucionesDetallesController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/DevolucionesDetalles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DevolucionDetalle>>> GetDevolucionesDetalles()
        {
            return await _context.DevolucionesDetalles.ToListAsync();
        }

        // GET: api/DevolucionesDetalles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DevolucionDetalle>> GetDevolucionDetalle(int id)
        {
            var devolucionDetalle = await _context.DevolucionesDetalles.FindAsync(id);

            if (devolucionDetalle == null)
            {
                return NotFound();
            }

            return devolucionDetalle;
        }

        // GET: api/DevolucionesDetalles/por-factura/5
        [HttpGet("por-factura/{idFactura}")]
        public async Task<ActionResult<IEnumerable<object>>> GetDevolucionesPorFactura(int idFactura)
        {
            try
            {
                var devoluciones = await _context.DevolucionesDetalles
                    .Include(dd => dd.Devolucion)
                        .ThenInclude(d => d.Factura)
                    .Include(dd => dd.Devolucion)
                        .ThenInclude(d => d.Empleado)
                    .Include(dd => dd.FacturaDetalle)
                        .ThenInclude(fd => fd.inventario)
                        .ThenInclude(i => i.Producto)
                    .Where(dd => dd.Devolucion.idFactura == idFactura)
                    .Select(dd => new
                    {
                        dd.idDevolucionDetalle,
                        dd.idDevolucion,
                        dd.Devolucion.Factura.idFactura,
                        dd.idFacturaDetalle,

                        dd.Devolucion.Empleado.nombre,
                        productoNombre = dd.FacturaDetalle.inventario.Producto.nombre,
                        dd.cantidadDevuelta,
                        dd.motivo,
                        dd.descripcion,
                        dd.precioUnitario,
                        dd.subtotal,
                        dd.reintegrarInventario,


                    })
                    .ToListAsync();

                if (!devoluciones.Any())
                {
                    return NotFound(new { mensaje = "No se encontraron devoluciones para esta factura" });
                }

                return Ok(devoluciones);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener devoluciones", error = ex.Message });
            }
        }
        // GET: api/DevolucionesDetalles/por-devolucion/5
        [HttpGet("por-devolucion/{idDevolucion}")]
        public async Task<ActionResult<IEnumerable<object>>> GetDetallesPorDevolucion(int idDevolucion)
        {
            try
            {
                var detalles = await _context.DevolucionesDetalles
                    .Include(dd => dd.Devolucion)
                        .ThenInclude(d => d.Factura)
                    .Include(dd => dd.Devolucion)
                        .ThenInclude(d => d.Empleado)
                    .Include(dd => dd.FacturaDetalle)
                        .ThenInclude(fd => fd.inventario)
                        .ThenInclude(i => i.Producto)
                    .Where(dd => dd.idDevolucion == idDevolucion)
                    .Select(dd => new
                    {
                        dd.idDevolucionDetalle,
                        dd.idDevolucion,
                        dd.idFacturaDetalle,
                        facturaId = dd.Devolucion.Factura.idFactura,
                        clienteNombre = dd.Devolucion.Factura.Cliente.nombre,
                        empleadoNombre = dd.Devolucion.Empleado.nombre,
                        productoNombre = dd.FacturaDetalle.inventario.Producto.nombre,
                        dd.cantidadDevuelta,
                        dd.motivo,
                        dd.descripcion,
                        dd.precioUnitario,
                        dd.subtotal,
                        dd.reintegrarInventario,
                    })
                    .ToListAsync();

                if (!detalles.Any())
                {
                    return NotFound(new { mensaje = "No se encontraron detalles para esta devolución" });
                }

                return Ok(detalles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener detalles de la devolución", error = ex.Message });
            }
        }


        // PUT: api/DevolucionesDetalles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDevolucionDetalle(int id, DevolucionDetalle devolucionDetalle)
        {
            if (id != devolucionDetalle.idDevolucionDetalle)
            {
                return BadRequest();
            }

            _context.Entry(devolucionDetalle).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DevolucionDetalleExists(id))
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



        // POST: api/DevolucionesDetalles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<DevolucionDetalle>> PostDevolucionDetalle(DevolucionDetalle devolucionDetalle)
        {
            _context.DevolucionesDetalles.Add(devolucionDetalle);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDevolucionDetalle", new { id = devolucionDetalle.idDevolucionDetalle }, devolucionDetalle);
        }

        // DELETE: api/DevolucionesDetalles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDevolucionDetalle(int id)
        {
            var devolucionDetalle = await _context.DevolucionesDetalles.FindAsync(id);
            if (devolucionDetalle == null)
            {
                return NotFound();
            }

            _context.DevolucionesDetalles.Remove(devolucionDetalle);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DevolucionDetalleExists(int id)
        {
            return _context.DevolucionesDetalles.Any(e => e.idDevolucionDetalle == id);
        }
    }
}
