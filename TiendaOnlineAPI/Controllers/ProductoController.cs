using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaOnlineAPI.Models;

namespace TiendaOnlineAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly TiendaContext _context;

        public ProductosController(TiendaContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            return await _context.Productos.Include(p => p.Categoria).ToListAsync();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetProducto(int id)
        {
            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.ProductoId == id);
            if (producto == null)
            {
                return NotFound();
            }
            return producto;
        }
        [HttpPost]
        public async Task<ActionResult> AgregarProducto(Producto producto)
        {
            _context.Productos.Add(producto);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetProducto), new { id = producto.ProductoId }, producto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProducto(int id, [FromBody] Producto producto)
        {
            if (producto == null)
                return BadRequest("Body vacío.");

            if (id != producto.ProductoId)
                return BadRequest("El id de la URL no coincide con el ProductoId del body.");

            var existe = await _context.Productos.AnyAsync(p => p.ProductoId == id);
            if (!existe)
                return NotFound();

            _context.Entry(producto).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchProducto(int id, [FromBody] Producto producto)
        {

            var existingProduct = _context.Productos.Find(id);
            if (existingProduct == null) return NotFound();

            if (!string.IsNullOrEmpty(producto.Nombre))
                existingProduct.Nombre = producto.Nombre;

            if (producto.Precio > 0)
                existingProduct.Precio = producto.Precio;

            if (!string.IsNullOrEmpty(producto.UrlImagen))
                existingProduct.UrlImagen = producto.UrlImagen;

            if (!string.IsNullOrEmpty(producto.Descripcion))
                existingProduct.Descripcion = producto.Descripcion;

            _context.SaveChanges();
            return Ok(existingProduct);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound();

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

}