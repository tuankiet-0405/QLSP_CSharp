using System.ComponentModel.DataAnnotations;

namespace THLTW.Models
{
    public class Order
    {
        public int Id { get; set; }
          [Required]
        public required string UserId { get; set; } // ASP.NET Identity User ID
        
        public DateTime OrderDate { get; set; } = DateTime.Now;
        
        public decimal TotalAmount { get; set; }
          [Required]
        [StringLength(20)]
        public required string Status { get; set; } = "Pending"; // Pending, Processing, Shipped, Delivered, Cancelled
        
        [StringLength(500)]
        public string? ShippingAddress { get; set; }
        
        [StringLength(100)]
        public string? CustomerName { get; set; }
        
        [StringLength(20)]
        public string? CustomerPhone { get; set; }
        
        // Navigation properties
        public ApplicationUser User { get; set; } = null!;
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
