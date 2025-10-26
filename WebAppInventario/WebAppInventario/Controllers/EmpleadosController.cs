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
    public class EmpleadosController : ControllerBase
    {
        private readonly MyDbContext _context;

        public EmpleadosController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/Empleados SOLAMENTE ACTIVOS
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Empleado>>> GetEmpleados()
        {
            var empleados = await _context.Empleados
       .Where(e => e.estado) // 🔹 solo activos
       .Include(e => e.Rol)
       .Select(e => new
       {
           e.idEmpleado,
           e.nombre,
           e.contraseña,
           e.credencial,
           e.telefono,
           e.email,
           e.direccion,
           e.fechaNacimiento,
           e.estado,
           rol = e.Rol != null ? e.Rol.rol : "Sin rol"
       })
       .ToListAsync();

            return Ok(empleados);
        }

        // GET: api/Empleados/5 SOLAMENTE ACTIVOS
        [HttpGet("{id}")]
        public async Task<ActionResult<Empleado>> GetEmpleado(int id)
        {
            var empleado = await _context.Empleados
        .Where(e => e.idEmpleado == id && e.estado) // 🔹 solo si está activo
        .Include(e => e.Rol)
        .Select(e => new
        {
            e.idEmpleado,
            e.nombre,
            e.contraseña,
            e.credencial,
            e.telefono,
            e.email,
            e.direccion,
            e.fechaNacimiento,
            e.estado,
            rol = e.Rol != null ? e.Rol.rol : "Sin rol"
        })
        .FirstOrDefaultAsync();

            if (empleado == null)
            {
                return NotFound(); // No existe o está inactivo
            }

            return Ok(empleado);
        }
        

        // PUT: api/Empleados/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmpleado(int id, Empleado empleado)
        {
            if (id != empleado.idEmpleado)
            {
                return BadRequest();
            }

            _context.Entry(empleado).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmpleadoExists(id))
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

        // POST: api/Empleados
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Empleado>> PostEmpleado(Empleado empleado)
        {
            _context.Empleados.Add(empleado);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEmpleado", new { id = empleado.idEmpleado }, empleado);
        }

        // DELETE: api/Empleados/5  SOLAMENTE SOFT DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmpleado(int id)
        {
            var empleado = await _context.Empleados.FindAsync(id);

            if (empleado == null)
            {
                return NotFound();
            }

            // 🔹 Soft delete: solo cambiar estado
            empleado.estado = false;

            _context.Entry(empleado).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmpleadoExists(int id)
        {
            return _context.Empleados.Any(e => e.idEmpleado == id);
        }
    }
}
