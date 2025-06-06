using THLTW.Models;

namespace THLTW.Models.ViewModels
{
    public class UserAnalyticsViewModel
    {
        public string? SelectedUserId { get; set; }
        public ApplicationUser? SelectedUser { get; set; }
        public List<ApplicationUser>? AllUsers { get; set; }
        
        public int TotalActivities { get; set; }
        public int ProductsViewed { get; set; }
        public int ReviewsWritten { get; set; }
        public double AverageSessionDuration { get; set; }
        public DateTime? LastActivity { get; set; }
        
        public Dictionary<string, int>? ActivityBreakdown { get; set; }
        public List<UserActivityDto>? RecentActivities { get; set; }
        public Dictionary<string, int>? FavoriteCategories { get; set; }
        public List<ProductReviewDto>? RecentReviews { get; set; }
    }
}
