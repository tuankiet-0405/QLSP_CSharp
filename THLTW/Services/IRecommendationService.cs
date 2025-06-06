using THLTW.Models;

namespace THLTW.Services
{
    public interface IRecommendationService
    {
        Task<List<Product>> GetRecommendedProductsAsync(string? userId, int count = 5);
        Task<List<Product>> GetSimilarProductsAsync(int productId, int count = 5);
        Task<List<Product>> GetTrendingProductsAsync(int count = 10);
        Task<List<Product>> GetPersonalizedRecommendationsAsync(string userId, int count = 5);
    }
}
