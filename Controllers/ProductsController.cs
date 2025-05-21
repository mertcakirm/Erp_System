using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Erp_System.Models;
using Erp_System.Data;
using Microsoft.AspNetCore.Authorization;

namespace Erp_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Products
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products
                .Where(p => p.DeletedAt == null)
                .Include(p => p.Tenant)
                .ToListAsync();
        }

        // GET: api/Products/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(long id)
        {
            var product = await _context.Products
                .Include(p => p.Tenant)
                .FirstOrDefaultAsync(p => p.Id == id && p.DeletedAt == null);

            if (product == null)
                return NotFound();

            return product;
        }

        // POST: api/Products
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            product.CreatedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        // PUT: api/Products/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(long id, Product updatedProduct)
        {
            if (id != updatedProduct.Id)
                return BadRequest();

            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null || existingProduct.DeletedAt != null)
                return NotFound();

            // Alanları güncelle
            existingProduct.Name = updatedProduct.Name;
            existingProduct.Description = updatedProduct.Description;
            existingProduct.Sku = updatedProduct.Sku;
            existingProduct.Price = updatedProduct.Price;
            existingProduct.StockQuantity = updatedProduct.StockQuantity;
            existingProduct.Attributes = updatedProduct.Attributes;
            existingProduct.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Products/5 (soft delete)
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(long id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null || product.DeletedAt != null)
                return NotFound();

            product.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(long id)
        {
            return _context.Products.Any(e => e.Id == id && e.DeletedAt == null);
        }
    }
}
