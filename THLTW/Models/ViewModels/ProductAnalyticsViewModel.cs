using THLTW.Models;

namespace THLTW.Models.ViewModels
{
    public class ProductAnalyticsViewModel
    {
        public int? SelectedProductId { get; set; }
        public Product? SelectedProduct { get; set; }
        public List<Product>? AllProducts { get; set; }
        
        public int TotalViews { get; set; }
        public int UniqueViewers { get; set; }
        public int TotalReviews { get; set; }
        public double AverageRating { get; set; }
        public int TotalSold { get; set; }
        
        public Dictionary<string, int>? ViewsByDay { get; set; }
        public List<ProductViewHistoryDto>? RecentViews { get; set; }
        public List<ProductReviewDto>? RecentReviews { get; set; }
        
        public double ConversionRate { get; set; }
        public double AverageViewDuration { get; set; }
        public double BounceRate { get; set; }
        public int ViewTrend { get; set; }
    }

    public class ProductViewHistoryDto
    {
        public DateTime ViewedAt { get; set; }
        public TimeSpan? ViewDuration { get; set; }
        public string? SessionId { get; set; }
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public string? IpAddress { get; set; }
    }
}
