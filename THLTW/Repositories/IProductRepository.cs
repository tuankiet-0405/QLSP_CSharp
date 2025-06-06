using THLTW.Models;

namespace THLTW.Repositories
{    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<(IEnumerable<Product> Products, int TotalCount)> GetAllAsync(int pageNumber, int pageSize);
        Task<(IEnumerable<Product> Products, int TotalCount)> GetFilteredAsync(int pageNumber, int pageSize, int? categoryId, decimal? minPrice, decimal? maxPrice, string? searchTerm);
        Task<Product?> GetByIdAsync(int id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
    }
}
