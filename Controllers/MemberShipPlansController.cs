using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Erp_System.Models;
using Erp_System.Data;

namespace Erp_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembershipPlansController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MembershipPlansController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/MembershipPlans
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MembershipPlan>>> GetMembershipPlans()
        {
            return await _context.MembershipPlans
                .Where(mp => mp.DeletedAt == null)
                .ToListAsync();
        }

        // GET: api/MembershipPlans/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MembershipPlan>> GetMembershipPlan(long id)
        {
            var membershipPlan = await _context.MembershipPlans
                .FirstOrDefaultAsync(mp => mp.Id == id && mp.DeletedAt == null);

            if (membershipPlan == null)
            {
                return NotFound();
            }

            return membershipPlan;
        }

        // POST: api/MembershipPlans
        [HttpPost]
        public async Task<ActionResult<MembershipPlan>> PostMembershipPlan(MembershipPlan membershipPlan)
        {
            membershipPlan.CreatedAt = DateTime.UtcNow;
            membershipPlan.UpdatedAt = DateTime.UtcNow;

            _context.MembershipPlans.Add(membershipPlan);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMembershipPlan), new { id = membershipPlan.Id }, membershipPlan);
        }

        // PUT: api/MembershipPlans/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMembershipPlan(long id, MembershipPlan membershipPlan)
        {
            if (id != membershipPlan.Id)
            {
                return BadRequest();
            }

            var existingPlan = await _context.MembershipPlans.FindAsync(id);
            if (existingPlan == null || existingPlan.DeletedAt != null)
            {
                return NotFound();
            }

            // Alanları güncelle
            existingPlan.Name = membershipPlan.Name;
            existingPlan.Description = membershipPlan.Description;
            existingPlan.Price = membershipPlan.Price;
            existingPlan.BillingCycle = membershipPlan.BillingCycle;
            existingPlan.Features = membershipPlan.Features;
            existingPlan.IsActive = membershipPlan.IsActive;
            existingPlan.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/MembershipPlans/5 (Soft Delete)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMembershipPlan(long id)
        {
            var membershipPlan = await _context.MembershipPlans.FindAsync(id);
            if (membershipPlan == null || membershipPlan.DeletedAt != null)
            {
                return NotFound();
            }

            membershipPlan.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MembershipPlanExists(long id)
        {
            return _context.MembershipPlans.Any(e => e.Id == id && e.DeletedAt == null);
        }
    }
}
