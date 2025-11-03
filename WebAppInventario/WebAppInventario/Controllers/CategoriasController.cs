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
    public class CategoriasController : ControllerBase
    {
        private readonly MyDbContext _context;

        public CategoriasController(MyDbContext context)
        {
            _context = context;
        }
        // Filtra los activos eso es perfecto
        // GET: api/Categorias
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categoria>>> GetCategorias()
        {
            var categoriasActivas = await _context.Categorias
            .Where(c => c.estado == true)
            .ToListAsync();

            return categoriasActivas;
        }

        // Busqueda o consulta unicamente por nombre de cliente 
        // GET: api/Categorias/buscar?buscar={TEXTO}
        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<Categoria>>> BuscarCategoria([FromQuery] CategoriasBusquedaParametros parametros)
        {
            var consulta = _context.Categorias
                .Where(c => c.estado) // solo los clientes activos
                .AsQueryable();
            if (!string.IsNullOrEmpty(parametros.buscar))
            {
                consulta = consulta.Where(Categoria => Categoria.nombre.Contains(parametros.buscar));
            }

            return await consulta.ToListAsync();
        }


        // GET: api/Categorias/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Categoria>> GetCategoria(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria == null)
            {
                return NotFound();
            }

            return categoria;
        }

        // PUT: api/Categorias/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategoria(int id, Categoria categoria)
        {
            if (id != categoria.idCategoria)
            {
                return BadRequest();
            }

            _context.Entry(categoria).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoriaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCategoria", new { id = categoria.idCategoria }, categoria);
        }

        // POST: api/Categorias
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Categoria>> PostCategoria(Categoria categoria)
        {
            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCategoria", new { id = categoria.idCategoria }, categoria);
        }

        // DELETE: api/Categorias/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoria(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null)
            {
                return NotFound();
            }

            categoria.estado = false; // Eliminacion lógica
            _context.Categorias.Update(categoria);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CategoriaExists(int id)
        {
            return _context.Categorias.Any(e => e.idCategoria == id);
        }
    }
}
