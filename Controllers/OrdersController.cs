using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Erp_System.Models;
using Erp_System.Data;

namespace Erp_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders
                .Where(o => o.DeletedAt == null)
                .Include(o => o.User)
                .Include(o => o.Tenant)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .ToListAsync();
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(long id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Tenant)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id && o.DeletedAt == null);

            if (order == null)
                return NotFound();

            return order;
        }

        // POST: api/Orders
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
            order.CreatedAt = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;

            // Opsiyonel: total hesapla
            order.TotalAmount = order.OrderItems.Sum(i => i.TotalPrice);

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        // PUT: api/Orders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(long id, Order updatedOrder)
        {
            if (id != updatedOrder.Id)
                return BadRequest();

            var existingOrder = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id && o.DeletedAt == null);

            if (existingOrder == null)
                return NotFound();

            // Alanları güncelle
            existingOrder.OrderDate = updatedOrder.OrderDate;
            existingOrder.Status = updatedOrder.Status;
            existingOrder.TotalAmount = updatedOrder.TotalAmount;
            existingOrder.UpdatedAt = DateTime.UtcNow;

            // OrderItems güncellemesi (geliştirilebilir - burada sade tutulmuştur)
            _context.OrderItems.RemoveRange(existingOrder.OrderItems);
            _context.OrderItems.AddRange(updatedOrder.OrderItems);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Orders/5 (soft delete)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(long id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null || order.DeletedAt != null)
                return NotFound();

            order.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(long id)
        {
            return _context.Orders.Any(e => e.Id == id && e.DeletedAt == null);
        }
    }
}
