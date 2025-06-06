using System.ComponentModel.DataAnnotations;

namespace THLTW.Models
{
    public class UserActivity
    {
        public int Id { get; set; }
          [Required]
        public required string UserId { get; set; } // ASP.NET Identity User ID
        
        public int? ProductId { get; set; } // Nullable for general activities
          [Required]
        [StringLength(50)]
        public required string ActivityType { get; set; } = string.Empty; // View, AddToCart, Purchase, Search, etc.
        
        [StringLength(500)]
        public string? ActivityData { get; set; } // JSON data for search terms, filters, etc.
        
        public DateTime Timestamp { get; set; } = DateTime.Now;
        
        [StringLength(100)]
        public string? SessionId { get; set; }
        
        [StringLength(50)]
        public string? IpAddress { get; set; }
        
        // Navigation properties
        public ApplicationUser User { get; set; } = null!;
        public Product? Product { get; set; }
    }
}
