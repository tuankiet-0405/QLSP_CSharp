using System.ComponentModel.DataAnnotations;

namespace THLTW.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public required string Name { get; set; }        public decimal Price { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public int StockQuantity { get; set; } = 0;

        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public List<ProductAdditionalImage>? AdditionalImages { get; set; }
        
        // AI Training Navigation Properties
        public List<ProductReview> Reviews { get; set; } = new List<ProductReview>();
        public List<ProductViewHistory> ViewHistory { get; set; } = new List<ProductViewHistory>();
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        
        // Computed properties for AI features
        public double AverageRating => Reviews.Any() ? Reviews.Average(r => r.Rating) : 0;
        public int TotalReviews => Reviews.Count;
        public int TotalViews => ViewHistory.Count;
        public int TotalSold => OrderItems.Sum(oi => oi.Quantity);
    }
}
