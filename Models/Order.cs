namespace Erp_System.Models;

public class Order
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long? UserId { get; set; }
    public DateTime? OrderDate { get; set; }
    public string? Status { get; set; } = "pending";
    public decimal? TotalAmount { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public Tenant Tenant { get; set; } = null!;
    public User? User { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}