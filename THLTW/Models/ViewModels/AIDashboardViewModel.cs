using THLTW.Models;

namespace THLTW.Models.ViewModels
{
    public class AIDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public int TotalReviews { get; set; }
        public int TotalUserActivities { get; set; }
        public int TotalProductViews { get; set; }
        
        public List<UserActivityDto>? RecentActivities { get; set; }
        public List<Product>? TopProducts { get; set; }
        public List<ProductReviewDto>? RecentReviews { get; set; }
    }    public class UserActivityDto
    {
        public string ActivityType { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public Product? Product { get; set; }
        public string? ProductName { get; set; }
        public string? ActivityData { get; set; }
        public string? IpAddress { get; set; }
        public string? Metadata { get; set; }
    }    public class ProductReviewDto
    {
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime ReviewDate => CreatedAt; // Alias for compatibility
        public string ProductName { get; set; } = string.Empty;
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public Product? Product { get; set; }
    }
}
