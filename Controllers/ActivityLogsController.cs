using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Erp_System.Models;
using Erp_System.Data;


namespace Erp_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityLogsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ActivityLogsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/ActivityLogs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ActivityLog>>> GetActivityLogs()
        {
            return await _context.ActivityLogs
                .Include(a => a.Tenant)
                .Include(a => a.User)
                .ToListAsync();
        }

        // GET: api/ActivityLogs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ActivityLog>> GetActivityLog(long id)
        {
            var activityLog = await _context.ActivityLogs
                .Include(a => a.Tenant)
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (activityLog == null)
            {
                return NotFound();
            }

            return activityLog;
        }

        // POST: api/ActivityLogs
        [HttpPost]
        public async Task<ActionResult<ActivityLog>> PostActivityLog(ActivityLog activityLog)
        {
            activityLog.CreatedAt = DateTime.UtcNow;
            _context.ActivityLogs.Add(activityLog);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetActivityLog), new { id = activityLog.Id }, activityLog);
        }

        // PUT: api/ActivityLogs/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutActivityLog(long id, ActivityLog activityLog)
        {
            if (id != activityLog.Id)
            {
                return BadRequest();
            }

            _context.Entry(activityLog).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActivityLogExists(id))
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

        // DELETE: api/ActivityLogs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActivityLog(long id)
        {
            var activityLog = await _context.ActivityLogs.FindAsync(id);
            if (activityLog == null)
            {
                return NotFound();
            }

            _context.ActivityLogs.Remove(activityLog);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ActivityLogExists(long id)
        {
            return _context.ActivityLogs.Any(e => e.Id == id);
        }
    }
}