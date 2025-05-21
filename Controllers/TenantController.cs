using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Erp_System.Models;
using Erp_System.Data;

namespace Erp_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenantsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TenantsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Tenants
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tenant>>> GetTenants()
        {
            return await _context.Tenants
                .Where(t => t.DeletedAt == null)
                .ToListAsync();
        }

        // GET: api/Tenants/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tenant>> GetTenant(long id)
        {
            var tenant = await _context.Tenants
                .Include(t => t.Users)
                .Include(t => t.Products)
                .Include(t => t.Orders)
                .Include(t => t.ActivityLogs)
                .FirstOrDefaultAsync(t => t.Id == id && t.DeletedAt == null);

            if (tenant == null)
                return NotFound();

            return tenant;
        }

        // POST: api/Tenants
        [HttpPost]
        public async Task<ActionResult<Tenant>> PostTenant(Tenant tenant)
        {
            tenant.CreatedAt = DateTime.UtcNow;
            tenant.UpdatedAt = DateTime.UtcNow;

            _context.Tenants.Add(tenant);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTenant), new { id = tenant.Id }, tenant);
        }

        // PUT: api/Tenants/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTenant(long id, Tenant updatedTenant)
        {
            if (id != updatedTenant.Id)
                return BadRequest();

            var existingTenant = await _context.Tenants.FindAsync(id);
            if (existingTenant == null || existingTenant.DeletedAt != null)
                return NotFound();

            existingTenant.Name = updatedTenant.Name;
            existingTenant.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Tenants/5 (soft delete)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTenant(long id)
        {
            var tenant = await _context.Tenants.FindAsync(id);
            if (tenant == null || tenant.DeletedAt != null)
                return NotFound();

            tenant.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TenantExists(long id)
        {
            return _context.Tenants.Any(t => t.Id == id && t.DeletedAt == null);
        }
    }
}
