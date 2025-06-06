using System.ComponentModel.DataAnnotations;

namespace THLTW.Models
{
    public class ProductViewHistory
    {
        public int Id { get; set; }
        
        public string? UserId { get; set; } // Nullable for anonymous users
        
        public int ProductId { get; set; }
        
        public DateTime ViewedAt { get; set; } = DateTime.Now;
        
        [StringLength(100)]
        public string? SessionId { get; set; } // For anonymous tracking
        
        [StringLength(50)]
        public string? IpAddress { get; set; }
        
        public int ViewDurationSeconds { get; set; } = 0; // Time spent viewing
        
        // Navigation properties
        public ApplicationUser? User { get; set; }
        public Product Product { get; set; } = null!;
    }
}
