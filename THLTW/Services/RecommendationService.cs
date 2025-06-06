using Microsoft.EntityFrameworkCore;
using THLTW.Data;
using THLTW.Models;

namespace THLTW.Services
{
    public class RecommendationService : IRecommendationService
    {
        private readonly ApplicationDbContext _context;

        public RecommendationService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get recommended products based on user behavior and popular items
        /// </summary>
        public async Task<List<Product>> GetRecommendedProductsAsync(string? userId, int count = 5)
        {
            if (string.IsNullOrEmpty(userId))
            {
                // For anonymous users, return popular products
                return await GetTrendingProductsAsync(count);
            }

            // For logged-in users, get personalized recommendations
            return await GetPersonalizedRecommendationsAsync(userId, count);
        }

        /// <summary>
        /// Get products similar to a given product (collaborative filtering simulation)
        /// </summary>
        public async Task<List<Product>> GetSimilarProductsAsync(int productId, int count = 5)
        {
            var targetProduct = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (targetProduct == null) return new List<Product>();

            // Simple similarity: same category + similar price range
            var priceRange = targetProduct.Price * 0.3m; // 30% price tolerance
            
            var similarProducts = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .Where(p => p.Id != productId && 
                           p.CategoryId == targetProduct.CategoryId &&
                           p.Price >= targetProduct.Price - priceRange &&
                           p.Price <= targetProduct.Price + priceRange)
                .OrderByDescending(p => p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0)
                .Take(count)
                .ToListAsync();

            return similarProducts;
        }

        /// <summary>
        /// Get trending products based on views, purchases, and ratings
        /// </summary>
        public async Task<List<Product>> GetTrendingProductsAsync(int count = 10)
        {
            var lastMonth = DateTime.Now.AddDays(-30);

            var trendingProducts = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .Include(p => p.ViewHistory)
                .Include(p => p.OrderItems)
                .Select(p => new 
                {
                    Product = p,
                    RecentViews = p.ViewHistory.Count(vh => vh.ViewedAt >= lastMonth),
                    RecentSales = p.OrderItems.Where(oi => oi.Order.OrderDate >= lastMonth).Sum(oi => oi.Quantity),
                    AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0,
                    TotalReviews = p.Reviews.Count
                })
                .OrderByDescending(x => 
                    (x.RecentViews * 0.3) + 
                    (x.RecentSales * 0.4) + 
                    (x.AverageRating * x.TotalReviews * 0.3))
                .Take(count)
                .Select(x => x.Product)
                .ToListAsync();

            return trendingProducts;
        }

        /// <summary>
        /// Get personalized recommendations based on user history
        /// </summary>
        public async Task<List<Product>> GetPersonalizedRecommendationsAsync(string userId, int count = 5)
        {
            // Get user's purchase history
            var userPurchases = await _context.OrderItems
                .Include(oi => oi.Product)
                .Where(oi => oi.Order.UserId == userId)
                .Select(oi => oi.Product)
                .ToListAsync();

            // Get user's viewed products
            var userViews = await _context.ProductViewHistories
                .Include(pvh => pvh.Product)
                .Where(pvh => pvh.UserId == userId)
                .Select(pvh => pvh.Product)
                .ToListAsync();

            // Find preferred categories
            var preferredCategories = userPurchases
                .Concat(userViews)
                .Where(p => p != null)
                .GroupBy(p => p!.CategoryId)
                .OrderByDescending(g => g.Count())
                .Take(3)
                .Select(g => g.Key)
                .ToList();

            // Get average price range user is interested in
            var avgPurchasePrice = userPurchases.Any() ? userPurchases.Average(p => p.Price) : 0;
            var priceRange = Math.Max(avgPurchasePrice * 0.5m, 1000000m); // At least 1M VND range

            // Find products in preferred categories with good ratings
            var recommendations = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .Where(p => preferredCategories.Contains(p.CategoryId) &&
                           !userPurchases.Select(up => up.Id).Contains(p.Id) &&
                           p.Price >= avgPurchasePrice - priceRange &&
                           p.Price <= avgPurchasePrice + priceRange)
                .OrderByDescending(p => p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0)
                .ThenByDescending(p => p.Reviews.Count)
                .Take(count)
                .ToListAsync();

            // If not enough recommendations, fill with trending products
            if (recommendations.Count < count)
            {
                var trending = await GetTrendingProductsAsync(count - recommendations.Count);
                recommendations.AddRange(trending.Where(t => !recommendations.Select(r => r.Id).Contains(t.Id)));
            }

            return recommendations.Take(count).ToList();
        }
    }
}
