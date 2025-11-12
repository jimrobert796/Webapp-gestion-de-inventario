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
    public class DevolucionesController : ControllerBase
    {
        private readonly MyDbContext _context;

        public DevolucionesController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/Devoluciones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Devolucion>>> GetDevoluciones()
        {
            try
            {
                var devoluciones = await _context.Devoluciones
                    .Include(d => d.Factura)
                    .Include(d => d.Empleado)
                    .Select(d => new
                    {
                        d.idDevolucion,
                        d.idFactura,
                        d.idEmpleado,
                        numeroFactura = d.Factura != null ? d.Factura.numeroFactura : "Sin número",
                        empleadoNombre = d.Empleado != null ? d.Empleado.nombre : "Sin empleado",
                        d.cantidad,
                        d.fechaDevolucion,
                        d.horaDevolucion,
                        d.totalDevolucion
                    })
                    .ToListAsync();

                if (!devoluciones.Any())
                    return NotFound(new { mensaje = "No hay devoluciones registradas." });

                return Ok(devoluciones);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener las devoluciones.", error = ex.Message });
            }
        }
        // GET: api/Devoluciones/buscar-devoluciones?buscar=Juan
        [HttpGet("buscar-devoluciones")]
        public async Task<ActionResult<IEnumerable<object>>> BuscarDevoluciones([FromQuery] BusquedaDevolucionesParametros parametros)
        {
            var consulta = _context.Devoluciones
                .Include(d => d.Empleado)
                .Include(d => d.Factura)
                .AsQueryable();

            if (!string.IsNullOrEmpty(parametros.buscar))
            {
                string texto = parametros.buscar.ToLower();
                consulta = consulta.Where(d =>
                    d.idFactura.ToString().Contains(texto) ||
                    d.Factura.numeroFactura.ToString().Contains(texto) ||
                    (d.Empleado != null && d.Empleado.nombre.ToLower().Contains(texto))
                );
            }

            var devoluciones = await consulta
                .Select(d => new
                {
                    d.idDevolucion,
                    d.idFactura,
                    d.idEmpleado,
                    d.Factura.numeroFactura,
                    d.Empleado.nombre,
                    d.cantidad,
                    d.fechaDevolucion,
                    d.horaDevolucion,
                    d.totalDevolucion
                })
                .ToListAsync();

            if (devoluciones.Count == 0)
                return NotFound("No se encontraron devoluciones con esos criterios.");

            return Ok(devoluciones);
        }

        // GET: api/Devoluciones/buscar-por-fecha?fecha=2024-01-15
        [HttpGet("buscar-por-fecha")]
        public async Task<ActionResult<IEnumerable<object>>> BuscarDevolucionesPorFecha([FromQuery] BusquedaFechaDevoluciones parametros)
        {
            if (!parametros.fecha.HasValue)
                return BadRequest("La fecha es requerida");

            var consulta = _context.Devoluciones
                .Include(d => d.Empleado)
                .Include(d => d.Factura)
                .Where(d => d.fechaDevolucion == parametros.fecha.Value);

            var devoluciones = await consulta
                .Select(d => new
                {
                    d.idDevolucion,
                    d.idFactura,
                    d.idEmpleado,
                    d.Factura.numeroFactura,
                    d.Empleado.nombre,
                    d.cantidad,
                    d.fechaDevolucion,
                    d.horaDevolucion,
                    d.totalDevolucion
                })
                .ToListAsync();

            if (devoluciones.Count == 0)
                return NotFound($"No se encontraron devoluciones para la fecha {parametros.fecha:yyyy-MM-dd}");

            return Ok(devoluciones);
        }


        // GET: api/Devoluciones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Devolucion>> GetDevolucion(int id)
        {
            var devolucion = await _context.Devoluciones.FindAsync(id);

            if (devolucion == null)
            {
                return NotFound();
            }

            return devolucion;
        }

        // PUT: api/Devoluciones/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDevolucion(int id, Devolucion devolucion)
        {
            if (id != devolucion.idDevolucion)
            {
                return BadRequest();
            }

            _context.Entry(devolucion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DevolucionExists(id))
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

        // POST: api/Devoluciones
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Devolucion>> PostDevolucion(Devolucion devolucion)
        {
            _context.Devoluciones.Add(devolucion);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDevolucion", new { id = devolucion.idDevolucion }, devolucion);
        }

        // DELETE: api/Devoluciones/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDevolucion(int id)
        {
            var devolucion = await _context.Devoluciones.FindAsync(id);
            if (devolucion == null)
            {
                return NotFound();
            }

            _context.Devoluciones.Remove(devolucion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DevolucionExists(int id)
        {
            return _context.Devoluciones.Any(e => e.idDevolucion == id);
        }
    }
}
