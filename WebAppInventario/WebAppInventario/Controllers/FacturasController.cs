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
    public class FacturasController : ControllerBase
    {
        private readonly MyDbContext _context;

        public FacturasController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/Facturas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Factura>>> GetFacturas()
        {
            var facturas = await _context.Facturas
            .Select(f => new
            {
                f.idFactura,
                f.idCliente,
                f.idEmpleado,
                f.numeroFactura,
                f.metodoPago,
                clienteNombre = f.Cliente != null ? f.Cliente.nombre : "Sin cliente",
                cajeroNombre = f.Empleado != null ? f.Empleado.nombre : "Sin cajero",
                f.iva,
                f.fecha,
                f.hora,
                f.total

            })
            .ToListAsync(); 
            return Ok(facturas);
        }

        // GET: api/Facturas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Factura>> GetFactura(int id)
        {
            var factura = await _context.Facturas
           .Include(f => f.Cliente)
           .Include(f => f.Empleado)
           .Where(f => f.idFactura == id)
           .Select(f => new
           {
               f.idFactura,
               f.idCliente,
               f.idEmpleado,
               f.numeroFactura,
               f.metodoPago,
               clienteNombre = f.Cliente != null ? f.Cliente.nombre : "Sin cliente",
               cajeroNombre = f.Empleado != null ? f.Empleado.nombre : "Sin cajero",
               f.iva,
               f.fecha,
               f.hora,
               f.total
           })
           .FirstOrDefaultAsync();

            if (factura == null)
            {
                return NotFound();
            }

            return Ok(factura);
        }

        // PUT: api/Facturas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFactura(int id, Factura factura)
        {
            if (id != factura.idFactura)
            {
                return BadRequest();
            }

            _context.Entry(factura).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FacturaExists(id))
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

        // POST: api/Facturas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Factura>> PostFactura(Factura factura)
        {
            _context.Facturas.Add(factura);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFactura", new { id = factura.idFactura }, factura);
        }

        // DELETE: api/Facturas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFactura(int id)
        {
            var factura = await _context.Facturas.FindAsync(id);
            if (factura == null)
            {
                return NotFound();
            }

            _context.Facturas.Remove(factura);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FacturaExists(int id)
        {
            return _context.Facturas.Any(e => e.idFactura == id);
        }
    }
}
