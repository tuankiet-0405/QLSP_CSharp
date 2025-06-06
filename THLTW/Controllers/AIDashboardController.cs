using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using THLTW.Data;
using THLTW.Services;
using Microsoft.EntityFrameworkCore;
using THLTW.Models.ViewModels;
using THLTW.Models;

namespace THLTW.Controllers
{
    [Authorize]
    public class AIDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IRecommendationService _recommendationService;

        public AIDashboardController(ApplicationDbContext context, IRecommendationService recommendationService)
        {
            _context = context;
            _recommendationService = recommendationService;
        }        public async Task<IActionResult> Index()
        {
            var recentActivities = await _context.UserActivities
                .Include(ua => ua.User)
                .Include(ua => ua.Product)
                .OrderByDescending(ua => ua.Timestamp)
                .Take(10)
                .Select(ua => new UserActivityDto
                { 
                    ActivityType = ua.ActivityType, 
                    Timestamp = ua.Timestamp, 
                    UserId = ua.UserId,
                    User = ua.User,
                    Product = ua.Product,
                    ActivityData = ua.ActivityData
                })
                .ToListAsync();

            var topProducts = await _context.Products
                .Include(p => p.ViewHistory)
                .Include(p => p.Reviews)
                .OrderByDescending(p => p.ViewHistory.Count)
                .Take(5)
                .ToListAsync();            var recentReviews = await _context.ProductReviews
                .Include(pr => pr.Product)
                .Include(pr => pr.User)
                .OrderByDescending(pr => pr.CreatedAt)
                .Take(5)
                .Select(pr => new ProductReviewDto
                { 
                    Rating = pr.Rating,
                    Comment = pr.Comment ?? string.Empty,
                    CreatedAt = pr.CreatedAt,
                    ProductName = pr.Product != null ? pr.Product.Name : "Unknown",
                    UserId = pr.UserId,
                    User = pr.User,
                    Product = pr.Product
                })
                .ToListAsync();

            var viewModel = new AIDashboardViewModel
            {
                TotalUsers = await _context.Users.CountAsync(),
                TotalProducts = await _context.Products.CountAsync(),
                TotalOrders = await _context.Orders.CountAsync(),
                TotalReviews = await _context.ProductReviews.CountAsync(),
                TotalUserActivities = await _context.UserActivities.CountAsync(),
                TotalProductViews = await _context.ProductViewHistories.CountAsync(),
                RecentActivities = recentActivities,
                TopProducts = topProducts,
                RecentReviews = recentReviews
            };

            return View(viewModel);
        }        public async Task<IActionResult> UserAnalytics(string userId = "")
        {
            var allUsers = await _context.Users
                .Select(u => new ApplicationUser { Id = u.Id, UserName = u.UserName, Email = u.Email })
                .Take(20)
                .ToListAsync();

            if (string.IsNullOrEmpty(userId))
            {
                var viewModel = new UserAnalyticsViewModel
                {
                    AllUsers = allUsers
                };
                return View(viewModel);
            }

            var selectedUser = await _context.Users.FindAsync(userId);
            if (selectedUser == null)
            {
                var viewModel = new UserAnalyticsViewModel
                {
                    AllUsers = allUsers
                };
                return View(viewModel);
            }

            var recentActivities = await _context.UserActivities
                .Include(ua => ua.Product)
                .Where(ua => ua.UserId == userId)
                .OrderByDescending(ua => ua.Timestamp)
                .Take(20)
                .Select(ua => new UserActivityDto
                { 
                    ActivityType = ua.ActivityType, 
                    Timestamp = ua.Timestamp,
                    Product = ua.Product,
                    ActivityData = ua.ActivityData
                })
                .ToListAsync();            var favoriteCategories = await _context.UserActivities
                .Where(ua => ua.UserId == userId && ua.Product != null && ua.Product.Category != null)
                .Include(ua => ua.Product)
                .ThenInclude(p => p!.Category)
                .GroupBy(ua => ua.Product!.Category!.Name)
                .Select(g => new { CategoryName = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToDictionaryAsync(x => x.CategoryName, x => x.Count);

            var recentReviews = await _context.ProductReviews
                .Include(pr => pr.Product)
                .Where(pr => pr.UserId == userId)
                .OrderByDescending(pr => pr.CreatedAt)
                .Take(5)
                .Select(pr => new ProductReviewDto
                { 
                    Rating = pr.Rating,
                    Comment = pr.Comment ?? string.Empty,
                    CreatedAt = pr.CreatedAt,
                    ProductName = pr.Product != null ? pr.Product.Name : "Unknown",
                    Product = pr.Product
                })
                .ToListAsync();

            var activities = await _context.UserActivities
                .Where(ua => ua.UserId == userId)
                .GroupBy(ua => ua.ActivityType)
                .ToDictionaryAsync(g => g.Key, g => g.Count());

            var lastActivity = await _context.UserActivities
                .Where(ua => ua.UserId == userId)
                .OrderByDescending(ua => ua.Timestamp)
                .Select(ua => ua.Timestamp)
                .FirstOrDefaultAsync();

            var userAnalyticsViewModel = new UserAnalyticsViewModel
            {
                SelectedUserId = userId,
                SelectedUser = selectedUser,
                AllUsers = allUsers,
                TotalActivities = await _context.UserActivities.CountAsync(ua => ua.UserId == userId),
                ProductsViewed = await _context.ProductViewHistories.Where(pvh => pvh.UserId == userId).Select(pvh => pvh.ProductId).Distinct().CountAsync(),
                ReviewsWritten = await _context.ProductReviews.CountAsync(pr => pr.UserId == userId),
                AverageSessionDuration = 5.2, // Placeholder - would need session tracking
                LastActivity = lastActivity,
                ActivityBreakdown = activities,
                RecentActivities = recentActivities,
                FavoriteCategories = favoriteCategories,
                RecentReviews = recentReviews
            };

            return View(userAnalyticsViewModel);
        }        public async Task<IActionResult> ProductAnalytics(int productId = 0)
        {
            var allProducts = await _context.Products
                .Include(p => p.Category)
                .Take(20)
                .ToListAsync();

            if (productId == 0)
            {
                var viewModel = new ProductAnalyticsViewModel
                {
                    AllProducts = allProducts
                };
                return View(viewModel);
            }

            var selectedProduct = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (selectedProduct == null)
            {
                var viewModel = new ProductAnalyticsViewModel
                {
                    AllProducts = allProducts
                };
                return View(viewModel);
            }

            var totalViews = await _context.ProductViewHistories.CountAsync(pvh => pvh.ProductId == productId);
            var totalReviews = await _context.ProductReviews.CountAsync(pr => pr.ProductId == productId);
            var averageRating = await _context.ProductReviews
                .Where(pr => pr.ProductId == productId)
                .AverageAsync(pr => (double?)pr.Rating) ?? 0;
            var totalSold = await _context.OrderItems
                .Where(oi => oi.ProductId == productId)
                .SumAsync(oi => oi.Quantity);            var viewsByDay = await _context.ProductViewHistories
                .Where(pvh => pvh.ProductId == productId)
                .GroupBy(pvh => pvh.ViewedAt.Date)
                .Select(g => new 
                { 
                    Date = g.Key,
                    ViewCount = g.Count()
                })
                .OrderBy(x => x.Date)
                .Take(30)
                .ToListAsync();

            var viewsByDayDict = viewsByDay.ToDictionary(x => x.Date.ToString("yyyy-MM-dd"), x => x.ViewCount);

            var recentViews = await _context.ProductViewHistories
                .Include(pvh => pvh.User)
                .Where(pvh => pvh.ProductId == productId)
                .OrderByDescending(pvh => pvh.ViewedAt)
                .Take(10)
                .Select(pvh => new ProductViewHistoryDto
                { 
                    ViewedAt = pvh.ViewedAt,
                    ViewDuration = TimeSpan.FromSeconds(pvh.ViewDurationSeconds),
                    SessionId = pvh.SessionId,
                    UserId = pvh.UserId,
                    User = pvh.User,
                    IpAddress = pvh.IpAddress
                })
                .ToListAsync();

            var recentReviews = await _context.ProductReviews
                .Include(pr => pr.User)
                .Where(pr => pr.ProductId == productId)
                .OrderByDescending(pr => pr.CreatedAt)
                .Take(10)
                .Select(pr => new ProductReviewDto
                { 
                    Rating = pr.Rating,
                    Comment = pr.Comment ?? string.Empty,
                    CreatedAt = pr.CreatedAt,
                    ProductName = selectedProduct.Name,
                    UserId = pr.UserId,
                    User = pr.User
                })
                .ToListAsync();

            var productAnalyticsViewModel = new ProductAnalyticsViewModel
            {
                SelectedProductId = productId,
                SelectedProduct = selectedProduct,
                AllProducts = allProducts,
                TotalViews = totalViews,
                TotalReviews = totalReviews,
                AverageRating = averageRating,
                TotalSold = totalSold,
                ViewsByDay = viewsByDayDict,
                RecentViews = recentViews,
                RecentReviews = recentReviews,
                UniqueViewers = await _context.ProductViewHistories
                    .Where(pvh => pvh.ProductId == productId && pvh.UserId != null)
                    .Select(pvh => pvh.UserId)
                    .Distinct()
                    .CountAsync(),
                ConversionRate = totalViews > 0 ? (double)totalSold / totalViews * 100 : 0,
                AverageViewDuration = await _context.ProductViewHistories
                    .Where(pvh => pvh.ProductId == productId)
                    .AverageAsync(pvh => (double?)pvh.ViewDurationSeconds) ?? 0,
                BounceRate = 0, // Placeholder - would need session tracking logic
                ViewTrend = 0 // Placeholder - would compare with previous period
            };

            return View(productAnalyticsViewModel);
        }        public async Task<IActionResult> Recommendations()
        {
            var allProducts = await _context.Products
                .Include(p => p.Category)
                .Take(20)
                .ToListAsync();

            var allUsers = await _context.Users
                .Take(20)
                .ToListAsync();

            var allCategories = await _context.Categories
                .ToListAsync();

            var totalRecommendations = await _context.UserActivities
                .Where(ua => ua.ActivityType == "recommendation_view")
                .CountAsync();

            var activeUsers = await _context.UserActivities
                .Where(ua => ua.Timestamp >= DateTime.Now.AddDays(-30))
                .Select(ua => ua.UserId)
                .Distinct()
                .CountAsync();

            var viewModel = new RecommendationsViewModel
            {
                AllProducts = allProducts,
                AllUsers = allUsers,
                AllCategories = allCategories,
                TotalRecommendations = totalRecommendations,
                ActiveUsers = activeUsers,
                TestResults = new List<RecommendationTestResult>()
            };

            return View(viewModel);
        }        [HttpGet]
        public async Task<IActionResult> GetSimilarProducts(int productId)
        {
            var similar = await _recommendationService.GetSimilarProductsAsync(productId, 5);
            return Json(similar.Select(p => new { p.Id, p.Name, p.Price }));
        }

        [HttpGet]
        public IActionResult TrendingAI()
        {
            return View();
        }
    }
}
