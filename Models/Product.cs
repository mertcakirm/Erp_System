namespace Erp_System.Models;

public class Product
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Sku { get; set; }
    public decimal? Price { get; set; }
    public int StockQuantity { get; set; }
    public string? Attributes { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public Tenant Tenant { get; set; } = null!;
}