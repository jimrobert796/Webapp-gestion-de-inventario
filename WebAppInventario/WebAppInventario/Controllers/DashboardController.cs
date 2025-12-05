using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppInventario.Models;

namespace WebAppInventario.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {

        private readonly MyDbContext _context;

        public DashboardController(MyDbContext context)
        {
            _context = context;
        }

        [HttpGet("kpis")]
        public async Task<ActionResult<Dashboard>> GetKpis()
        {
            try
            {
                var hoy = DateOnly.FromDateTime(DateTime.Now);
                var inicioMes = new DateOnly(hoy.Year, hoy.Month, 1);
                var finMes = inicioMes.AddMonths(1).AddDays(-1);

                var hoyFecha = DateOnly.FromDateTime(DateTime.Now);

                // Consultas principales
                var productosTotales = await _context.Productos .Where(p => p.estado == true).CountAsync();

                var categorias = await _context.Categorias.Where(p => p.estado == true).CountAsync();
                var clientes = await _context.Clientes.Where(p => p.estado == true).CountAsync();
                var empleados = await _context.Empleados.Where(p => p.estado == true).CountAsync();
                var proveedores = await _context.Proveedores.Where(p => p.estado == true).CountAsync();

                
                // Stock bajo
                var stockBajo = await _context.Inventario
                    .Where(p => p.cantidad <= 20 && p.Producto.estado == true)
                    .CountAsync();
                 
                // Valor del inventario
                var valorInventario = await _context.Inventario
                    .Where(p=> p.Producto.estado == true)
                    .SumAsync(p => p.cantidad * p.precio );
                
                //Ventas de hoy
                var ventasHoy = await _context.Facturas
                    .Where(f => f.fecha == hoyFecha)
                    .SumAsync(f => f.total);
               
                // Facturas de hoy
                var facturasHoy = await _context.Facturas
                    .Where(f => f.fecha == hoyFecha)
                    .CountAsync();
                
               // Compras del mes
               var comprasMes = await _context.Compras
                   .Where(c => c.fechaCompra >= inicioMes && c.fechaCompra <= finMes)
                   .SumAsync(c => c.total);
                
               // Cantidad de compras del mes
               var comprasRealizadasMes = await _context.Compras
                   .Where(c => c.fechaCompra >= inicioMes && c.fechaCompra <= finMes && c.estado.Trim() == "Activo")
                   .CountAsync();
               
               // Devoluciones del mes
               var devolucionesMes = await _context.Devoluciones
                   .Where(d => d.fechaDevolucion >= inicioMes && d.fechaDevolucion <= finMes)
                   .CountAsync();

                //Obtener cuanto fue de devoluciones en el mes 
                var devolucionesMesTotales = await _context.Devoluciones
                    .Where(d => d.fechaDevolucion >= inicioMes && d.fechaDevolucion <= finMes)
                    .SumAsync(d => d.totalDevolucion);

                // Ventas totales del mes
                var ventasTotalesMes = await _context.Facturas
                  .Where(f => f.fecha >= inicioMes && f.fecha <= finMes)
                  .SumAsync(f => f.total);

                var ventasNetasMes = ventasTotalesMes - devolucionesMesTotales;



                var kpis = new Dashboard
                {
                    productosTotales = productosTotales,
                    valorInventario = valorInventario,
                    stockBajo = stockBajo,
                    facturasHoy = facturasHoy,
                    ventasHoy = ventasHoy,
                    comprasMes = comprasMes,
                    comprasRealizadasMes = comprasRealizadasMes,
                    devolucionesMes = devolucionesMes,
                    proveedores = proveedores,
                    clientes = clientes,
                    empleados = empleados,
                    categorias = categorias,
                    ventasTotalesMes = ventasNetasMes,
                    promedioDiario = ventasTotalesMes / DateTime.DaysInMonth(hoy.Year, hoy.Month)
                };

                return Ok(kpis);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error interno: {ex.Message}" });
            }
        }

        [HttpGet("ventas-mensuales")]
        public async Task<ActionResult> GetVentasMensuales()
        {
            try
            {
                var hoy = DateOnly.FromDateTime(DateTime.Now);
                var inicioMes = new DateOnly(hoy.Year, hoy.Month, 1);
                var finMes = inicioMes.AddMonths(1).AddDays(-1);

                var ventasPorDia = await _context.Facturas
                    .Where(f => f.fecha >= inicioMes && f.fecha <= finMes)
                    .GroupBy(f => f.fecha.Day)
                    .Select(g => new ventaDiaria
                    {
                        dia = g.Key,
                        ventas = g.Sum(f => f.total)
                    })
                    .OrderBy(x => x.dia)
                    .ToListAsync();

                // Rellenar días sin ventas
                var resultado = new List<ventaDiaria>();
                for (int dia = 1; dia <= finMes.Day; dia++)
                {
                    var ventaDia = ventasPorDia.FirstOrDefault(v => v.dia == dia);
                    resultado.Add(new ventaDiaria
                    {
                        dia = dia,
                        ventas = ventaDia?.ventas ?? 0
                    });
                }

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error interno: {ex.Message}" });
            }
        }

        [HttpGet("actividad-reciente")]
        public async Task<ActionResult> GetActividadReciente()
        {
            try
            {
                var hoy = DateOnly.FromDateTime(DateTime.Now);
                var ayer = DateOnly.FromDateTime(DateTime.Now.AddDays(-1));

                var ultimas24Horas = await _context.Compras
                    .Where(c => c.fechaCompra >= ayer && c.fechaCompra <= hoy)
                    .ToListAsync();

                var actividades = new List<actividadReciente>();

                // 1. Últimas compras (últimas 24 horas)
                var comprasRecientes = await _context.Compras
                    .Include(c => c.Proveedor)
                    .Where(c => c.fechaCompra >= ayer && c.fechaCompra <= hoy)
                    .OrderByDescending(c => c.fechaCompra)
                    .Take(5)
                    .ToListAsync();

                foreach (var compra in comprasRecientes)
                {
                    actividades.Add(new actividadReciente
                    {
                        tipo = "compra",
                        titulo = "Compra recibida",
                        descripcion = $"Proveedor: {compra.Proveedor.nombre}",
                        fecha = compra.fechaCompra,
                        icono = "bi-bag-check",
                        color = "primary"
                    });
                }

                // 2. Últimas facturas (últimas 24 horas)
                var facturasRecientes = await _context.Facturas
                    .Where(f => f.fecha >= ayer && f.fecha <= hoy)
                    .OrderByDescending(f => f.fecha)
                    .Take(5)
                    .ToListAsync();

                foreach (var factura in facturasRecientes)
                {
                    actividades.Add(new actividadReciente
                    {
                        tipo = "venta",
                        titulo = "Venta en caja",
                        descripcion = $"Factura {factura.numeroFactura}",
                        fecha = factura.fecha,
                        icono = "bi-cart-check",
                        color = "success"
                    });
                }

                // 3. Últimas devoluciones (últimas 24 horas)
                var devolucionesRecientes = await _context.Devoluciones
                    .Include(d => d.Factura)
                    .Where(d => d.fechaDevolucion >= ayer && d.fechaDevolucion <= hoy)
                    .OrderByDescending(d => d.fechaDevolucion)
                    .Take(5)
                    .ToListAsync();

                foreach (var devolucion in devolucionesRecientes)
                {
                    actividades.Add(new actividadReciente
                    {
                        tipo = "devolucion",
                        titulo = "Devolución registrada",
                        descripcion = $"Factura {devolucion.Factura.numeroFactura}",
                        fecha = devolucion.fechaDevolucion,
                        icono = "bi-arrow-return-left",
                        color = "warning"
                    });
                }

                // Ordenar por fecha (más reciente primero) y tomar solo 3
                var resultado = actividades
                    .OrderByDescending(a => a.fecha)
                    .Take(3)
                    .ToList();

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error interno: {ex.Message}" });
            }
        }


        [HttpGet("top-productos")]
        public async Task<ActionResult> GetTopProductos()
        {
            try
            {
                var topProductos = await _context.FacturasDetalles
                    .Include(fd => fd.inventario)
                    .ThenInclude(i => i.Producto)
                    .GroupBy(fd => new { fd.inventario.Producto.idProducto, fd.inventario.Producto.nombre })
                    .Select(g => new topProducto
                    {
                        nombre = g.Key.nombre,
                        ventas = g.Sum(fd => fd.cantidad)
                    })
                    .OrderByDescending(x => x.ventas)
                    .Take(5)
                    .ToListAsync();

                return Ok(topProductos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error interno: {ex.Message}" });
            }
        }
        [HttpGet("ultimas-compras")]
        public async Task<ActionResult> GetUltimasCompras()
        {
            try
            {
                var ultimasCompras = await _context.Compras
                    .Include(c => c.Proveedor)
                    .Where(c => c.estado.Trim() == "Activo")   // Filtrar solo activas
                    .OrderByDescending(c => c.fechaCompra)
                    .ThenByDescending(c => c.idCompra) // asegura orden correcto
                    .Take(3)
                    .Select(c => new ultimaCompra
                    {
                        fecha = c.fechaCompra,
                        proveedor = c.Proveedor.nombre,
                        total = c.total
                    })
                    .ToListAsync();

                return Ok(ultimasCompras);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error interno: {ex.Message}" });
            }
        }


        [HttpGet("ultimas-facturas")]
        public async Task<ActionResult> GetUltimasFacturas()
        {
            try
            {
                var ultimasFacturas = await _context.Facturas
                    .OrderByDescending(f => f.fecha)
                    .ThenByDescending(c => c.idFactura) // asegura orden correcto
                    .Take(3)
                    .Select(f => new ultimaFactura
                    {
                        fecha = f.fecha,
                        numeroFactura = f.numeroFactura,
                        total = f.total
                    })
                    .ToListAsync();

                return Ok(ultimasFacturas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error interno: {ex.Message}" });
            }
        }

    }
}
