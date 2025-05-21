namespace Erp_System.Models
{
    public class ActivityLog
    {
        public long Id { get; set; }
        public long TenantId { get; set; }
        public long? UserId { get; set; }
        public string ActionType { get; set; }
        public string? EntityType { get; set; }
        public long? EntityId { get; set; }
        public string? Description { get; set; }
        public string? Data { get; set; } // JSON formatında string
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public DateTime CreatedAt { get; set; }

        public Tenant Tenant { get; set; }
        public User? User { get; set; }
    }
}