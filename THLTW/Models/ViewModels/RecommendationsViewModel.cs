using THLTW.Models;

namespace THLTW.Models.ViewModels
{
    public class RecommendationsViewModel
    {
        public List<Product>? AllProducts { get; set; }
        public List<ApplicationUser>? AllUsers { get; set; }
        public List<Category>? AllCategories { get; set; }
        
        public int TotalRecommendations { get; set; }
        public int ActiveUsers { get; set; }
        
        public List<RecommendationTestResult>? TestResults { get; set; }
    }

    public class RecommendationTestResult
    {
        public string TestType { get; set; } = string.Empty;
        public string Target { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public int ResultCount { get; set; }
        public double ResponseTime { get; set; }
        public bool Success { get; set; }
        public List<Product>? Products { get; set; }
    }
}
