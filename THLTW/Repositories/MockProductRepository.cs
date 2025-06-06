using THLTW.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace THLTW.Repositories
{
    public class MockProductRepository : IProductRepository
    {
        private List<Product> _products;
        private readonly ICategoryRepository _categoryRepository; // Added

        public MockProductRepository(ICategoryRepository categoryRepository) // Updated constructor
        {
            _categoryRepository = categoryRepository; // Added
            _products = new List<Product>
            {
                new Product { Id = 1, Name = "Laptop", Price = 1200.00m, Description = "High-performance laptop", ImageUrl = "/images/laptop.png", CategoryId = 1, AdditionalImages = new List<ProductAdditionalImage> { new ProductAdditionalImage { ImageUrl = "/images/laptop_side.png" }, new ProductAdditionalImage { ImageUrl = "/images/laptop_top.png" } } },
                new Product { Id = 2, Name = "Mouse", Price = 25.00m, Description = "Ergonomic wireless mouse", ImageUrl = "/images/mouse.png", CategoryId = 1, AdditionalImages = new List<ProductAdditionalImage>() },
                new Product { Id = 3, Name = "Keyboard", Price = 75.00m, Description = "Mechanical gaming keyboard", ImageUrl = "/images/keyboard.png", CategoryId = 1, AdditionalImages = new List<ProductAdditionalImage>() },
                new Product { Id = 4, Name = "T-Shirt", Price = 20.00m, Description = "Cotton t-shirt", ImageUrl = "/images/tshirt.png", CategoryId = 2, AdditionalImages = new List<ProductAdditionalImage>() }
            };
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            // Manually load categories for each product
            foreach (var product in _products)
            {
                if (product.CategoryId != 0) // Ensure CategoryId is valid
                {
                    product.Category = await _categoryRepository.GetByIdAsync(product.CategoryId);
                }
            }
            return _products;
        }

        public async Task<(IEnumerable<Product> Products, int TotalCount)> GetAllAsync(int pageNumber, int pageSize)
        {
            // Manually load categories for each product before pagination
            foreach (var product in _products)
            {
                if (product.CategoryId != 0 && product.Category == null) // Ensure CategoryId is valid and not already loaded
                {
                    product.Category = await _categoryRepository.GetByIdAsync(product.CategoryId);
                }
            }

            var totalCount = _products.Count;
            var products = _products.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return (products, totalCount);
        }

        public async Task<Product?> GetByIdAsync(int id) // Changed to Product?
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product != null && product.CategoryId != 0 && product.Category == null)
            {
                product.Category = await _categoryRepository.GetByIdAsync(product.CategoryId);
            }
            return product;
        }

        public Task AddAsync(Product product)
        {
            product.Id = _products.Any() ? _products.Max(p => p.Id) + 1 : 1; // Handle empty list
            _products.Add(product);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Product product)
        {
            var existingProduct = _products.FirstOrDefault(p => p.Id == product.Id);
            if (existingProduct != null)
            {
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Description = product.Description;
                existingProduct.ImageUrl = product.ImageUrl; // This will be handled by file upload
                existingProduct.CategoryId = product.CategoryId;
                // existingProduct.AdditionalImageUrls = product.AdditionalImageUrls; // This will be handled by file upload
                existingProduct.AdditionalImages = product.AdditionalImages; // Updated to use AdditionalImages
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                _products.Remove(product);
            }
            return Task.CompletedTask;
        }

        public async Task<(IEnumerable<Product> Products, int TotalCount)> GetFilteredAsync(int pageNumber, int pageSize, int? categoryId, decimal? minPrice, decimal? maxPrice, string? searchTerm)
        {
            // Manually load categories for each product first
            foreach (var product in _products)
            {
                if (product.CategoryId != 0 && product.Category == null)
                {
                    product.Category = await _categoryRepository.GetByIdAsync(product.CategoryId);
                }
            }

            var query = _products.AsQueryable();

            // Apply filters
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.Name.ToLower().Contains(searchTerm.ToLower()) ||
                                        (p.Description != null && p.Description.ToLower().Contains(searchTerm.ToLower())));
            }

            var totalCount = query.Count();
            var products = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return (products, totalCount);
        }
    }
}
