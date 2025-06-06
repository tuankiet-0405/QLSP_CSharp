using System.ComponentModel.DataAnnotations;

namespace THLTW.Models
{
    public class ProductReview
    {
        public int Id { get; set; }
          [Required]
        public required string UserId { get; set; } // ASP.NET Identity User ID
        
        public int ProductId { get; set; }
        
        [Range(1, 5)]
        public int Rating { get; set; } // 1-5 stars
        
        [StringLength(1000)]
        public string? Comment { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Navigation properties
        public Product Product { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
    }
}
