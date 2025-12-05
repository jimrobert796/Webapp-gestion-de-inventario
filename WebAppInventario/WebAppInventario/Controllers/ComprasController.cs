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
    public class ComprasController : ControllerBase
    {
        private readonly MyDbContext _context;

        public ComprasController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/Compras
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Compra>>> GetCompras()
        {
            var compras = await _context.Compras
       .Include(c => c.Empleado)   // Incluye información del empleado
       .Include(c => c.Proveedor)  // Incluye información del proveedor
       .Select(c => new
       {
           c.idCompra,
           c.idEmpleado,
           c.idProveedor,
           nombreEmpleado = c.Empleado.nombre,     // Nombre del empleado
           nombreProveedor = c.Proveedor.nombre,   // Nombre del proveedor
           c.numeroCompra,
           c.fechaCompra,
           c.horaCompra,
           c.total,
           c.subtotal,
           c.iva,
           c.cantidad,
           c.metodoPago,
           c.estado,
           c.motivoAnulacion,
           c.fechaAnulacion,
           c.idEmpleadoAnulacion
       })
       .ToListAsync();

            return Ok(compras);
        }

        // GET: api/Compras/buscar?buscar=texto
        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<object>>> BuscarCompras([FromQuery] BusquedaComprasParametros parametros)
        {
            var consulta = _context.Compras
                .Include(c => c.Empleado)
                .Include(c => c.Proveedor)
                .AsQueryable();

            if (!string.IsNullOrEmpty(parametros.buscar))
            {
                string texto = parametros.buscar.ToLower();
                consulta = consulta.Where(c =>
                    c.numeroCompra.ToLower().Contains(texto) ||
                    (c.Empleado != null && c.Empleado.nombre.ToLower().Contains(texto)) ||
                    (c.Proveedor != null && c.Proveedor.nombre.ToLower().Contains(texto))
                );
            }

            var compras = await consulta
                .Select(c => new
                {
                    c.idCompra,
                    c.idEmpleado,
                    c.idProveedor,
                    nombreEmpleado = c.Empleado != null ? c.Empleado.nombre : "Sin empleado",
                    nombreProveedor = c.Proveedor != null ? c.Proveedor.nombre : "Sin proveedor",
                    c.numeroCompra,
                    c.fechaCompra,
                    c.horaCompra,
                    c.total,
                    c.subtotal,
                    c.iva,
                    c.cantidad,
                    c.metodoPago,
                    c.estado,
                    c.motivoAnulacion,
                    c.fechaAnulacion,
                    c.idEmpleadoAnulacion
                })
                .ToListAsync();

            if (compras.Count == 0)
                return NotFound("No se encontraron compras con esos criterios.");

            return Ok(compras);
        }
        // GET: api/Compras/buscar-por-fecha?fecha=2025-11-11
        [HttpGet("buscar-por-fecha")]
        public async Task<ActionResult<IEnumerable<object>>> BuscarComprasPorFecha([FromQuery] BusquedaFechaCompras parametros)
        {
            if (!parametros.fecha.HasValue)
                return BadRequest("La fecha es requerida");

            var consulta = _context.Compras
                .Include(c => c.Empleado)
                .Include(c => c.Proveedor)
                .Where(c => c.fechaCompra == parametros.fecha.Value);

            var compras = await consulta
                .Select(c => new
                {
                    c.idCompra,
                    c.idEmpleado,
                    c.idProveedor,
                    nombreEmpleado = c.Empleado != null ? c.Empleado.nombre : "Sin empleado",
                    nombreProveedor = c.Proveedor != null ? c.Proveedor.nombre : "Sin proveedor",
                    c.numeroCompra,
                    c.fechaCompra,
                    c.horaCompra,
                    c.total,
                    c.subtotal,
                    c.iva,
                    c.cantidad,
                    c.metodoPago,
                    c.estado,
                    c.motivoAnulacion,
                    c.fechaAnulacion,
                    c.idEmpleadoAnulacion
                })
                .ToListAsync();

            if (compras.Count == 0)
                return NotFound($"No se encontraron compras para la fecha {parametros.fecha:yyyy-MM-dd}");

            return Ok(compras);
        }


        /// GET: api/Compras/buscar-por-proveedor?idProveedor=1
        [HttpGet("buscar-por-proveedor")]
        public async Task<ActionResult<IEnumerable<object>>> BuscarComprasPorProveedor([FromQuery] BusquedaProveedorCompras parametros)
        {
            if (!parametros.idProveedor.HasValue)
                return BadRequest("El ID del proveedor es requerido");

            var consulta = _context.Compras
                .Include(c => c.Empleado)
                .Include(c => c.Proveedor)
                .Where(c => c.idProveedor == parametros.idProveedor.Value);

            var compras = await consulta
                .Select(c => new
                {
                    c.idCompra,
                    c.idEmpleado,
                    c.idProveedor,
                    nombreEmpleado = c.Empleado != null ? c.Empleado.nombre : "Sin empleado",
                    nombreProveedor = c.Proveedor != null ? c.Proveedor.nombre : "Sin proveedor",
                    c.numeroCompra,
                    c.fechaCompra,
                    c.horaCompra,
                    c.total,
                    c.subtotal,
                    c.iva,
                    c.cantidad,
                    c.metodoPago,
                    c.estado,
                    c.motivoAnulacion,
                    c.fechaAnulacion,
                    c.idEmpleadoAnulacion
                })
                .ToListAsync();

            if (compras.Count == 0)
                return NotFound($"No se encontraron compras para el proveedor con ID {parametros.idProveedor}");

            return Ok(compras);
        }

        [HttpPut("anular-auto/{id}")]
        public async Task<ActionResult> AnularCompraAuto(int id, [FromBody] anulacionAuto anulacionData)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var compra = await _context.Compras
                    .Include(c => c.ComprasDetalles)
                    .FirstOrDefaultAsync(c => c.idCompra == id);

                if (compra == null)
                    return NotFound(new { success = false, message = "Compra no encontrada" });

                if (compra.estado == "anulado")
                    return BadRequest(new { success = false, message = "La compra ya está anulada" });

                // 🔁 Revertir stock de inventario
                foreach (var detalle in compra.ComprasDetalles)
                {
                    var inventario = await _context.Inventario
                        .FirstOrDefaultAsync(i => i.idInventario == detalle.idInventario);
                    Console.WriteLine(detalle.idInventario);

                    if (inventario != null)
                    {
                        // ✅ Restar y asegurar que no quede negativo
                        inventario.cantidad = Math.Max(inventario.cantidad - detalle.cantidad, 0);
                        inventario.costo = detalle.costoAnterior;
                        inventario.precio = detalle.precioAnterior;
                        inventario.ultimaActualizacion = DateTime.Now;

                        //Forzar el seguimiento
                        _context.Entry(inventario).State = EntityState.Modified;
                    }
                }

                //Marcar compra como anulada
                compra.estado = "Anulado";
                compra.motivoAnulacion = string.IsNullOrWhiteSpace(anulacionData.motivo)
                    ? "Anulación automática"
                    : anulacionData.motivo;
                compra.fechaAnulacion = DateTime.Now;
                compra.idEmpleadoAnulacion = anulacionData.idEmpleado;

                _context.Entry(compra).State = EntityState.Modified;

                //Guardar cambios antes de commit
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                //Devolver también el nombre del empleado de anulación
                var empleado = await _context.Empleados
                    .FirstOrDefaultAsync(e => e.idEmpleado == compra.idEmpleadoAnulacion);

                return Ok(new
                {
                    success = true,
                    message = "Compra anulada exitosamente.",
                    compra.idCompra,
                    compra.estado,
                    compra.motivoAnulacion,
                    empleadoAnulacion = empleado != null ? empleado.nombre : "Desconocido"
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error al anular la compra automáticamente.",
                    error = ex.Message
                });
            }
        }







        // GET: api/Compras/buscar-anidado?fecha=2025-11-11&proveedor=5&estado=activo&buscar=ORD001
        [HttpGet("buscar-anidado")]
        public async Task<ActionResult<IEnumerable<object>>> BuscarComprasAnidado(
            [FromQuery] DateOnly? fecha,
            [FromQuery] int? proveedor,
            [FromQuery] string? estado,
            [FromQuery] string? buscar)
        {
            var consulta = _context.Compras
                .Include(c => c.Empleado)
                .Include(c => c.Proveedor)
                .Include(c => c.EmpleadoAnulacion)
                .AsQueryable();

            // 🔸 Filtros opcionales y combinables
            if (fecha.HasValue)
                consulta = consulta.Where(c => c.fechaCompra == fecha);

            if (proveedor.HasValue)
                consulta = consulta.Where(c => c.idProveedor == proveedor.Value);

            if (!string.IsNullOrWhiteSpace(estado))
                consulta = consulta.Where(c => c.estado.ToLower().Trim() == estado.ToLower().Trim());

            if (!string.IsNullOrWhiteSpace(buscar))
                consulta = consulta.Where(c =>
                    c.numeroCompra.Contains(buscar) ||
                    (c.Empleado != null && c.Empleado.nombre.Contains(buscar)) ||
                    (c.Proveedor != null && c.Proveedor.nombre.Contains(buscar))
                );

            var compras = await consulta
                .Select(c => new
                {
                    c.idCompra,
                    c.numeroCompra,
                    c.fechaCompra,
                    c.horaCompra,
                    c.total,
                    c.subtotal,
                    c.iva,
                    c.cantidad,
                    c.metodoPago,
                    c.estado,
                    c.motivoAnulacion,
                    c.fechaAnulacion,
                    c.idProveedor,
                    nombreProveedor = c.Proveedor != null ? c.Proveedor.nombre : "Sin proveedor",
                    c.idEmpleado,
                    nombreEmpleado = c.Empleado != null ? c.Empleado.nombre : "Sin empleado",
                    c.idEmpleadoAnulacion,
                    nombreEmpleadoAnulacion = c.EmpleadoAnulacion.nombre
                })
                .ToListAsync();

            if (!compras.Any())
                return NotFound("No se encontraron compras con los criterios especificados.");

            return Ok(compras);
        }




        // GET: api/Compras/nueva-compra
        [HttpGet("nueva-compra")]
        public async Task<ActionResult<string>> GetSiguienteNumeroCompra()
        {
            // Obtener el último número de compra existente (formato COM000001)
            var ultimoNumeroStr = await _context.Compras
                .Select(c => c.numeroCompra)
                .OrderByDescending(n => n)
                .FirstOrDefaultAsync();

            int siguienteNumero = 1;

            if (!string.IsNullOrEmpty(ultimoNumeroStr) && ultimoNumeroStr.Length > 3)
            {
                // Extrae los últimos 6 dígitos (parte numérica)
                var parteNumerica = ultimoNumeroStr.Substring(3);
                if (int.TryParse(parteNumerica, out int numero))
                {
                    siguienteNumero = numero + 1;
                }
            }

            // Devuelve el nuevo número formateado
            string numeroFormateado = $"COM{siguienteNumero:D6}";
            return Ok(numeroFormateado);
        }




        // GET: api/Compras/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Compra>> GetCompra(int id)
        {
            var compra = await _context.Compras.FindAsync(id);

            if (compra == null)
            {
                return NotFound();
            }

            return compra;
        }

        // PUT: api/Compras/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompra(int id, Compra compra)
        {
            if (id != compra.idCompra)
            {
                return BadRequest();
            }

            _context.Entry(compra).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompraExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCompra", new { id = compra.idCompra }, compra);
        }

        // POST: api/Compras
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Compra>> PostCompra(Compra compra)
        {
            _context.Compras.Add(compra);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCompra", new { id = compra.idCompra }, compra);
        }

        // DELETE: api/Compras/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompra(int id)
        {
            var compra = await _context.Compras.FindAsync(id);
            if (compra == null)
            {
                return NotFound();
            }

            _context.Compras.Remove(compra);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CompraExists(int id)
        {
            return _context.Compras.Any(e => e.idCompra == id);
        }
    }
}
