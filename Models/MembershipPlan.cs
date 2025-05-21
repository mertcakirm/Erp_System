namespace Erp_System.Models
{
    public class MembershipPlan
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string BillingCycle { get; set; } // enum('monthly','yearly','weekly')
        public string? Features { get; set; } // JSON olarak string tutulacak
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}