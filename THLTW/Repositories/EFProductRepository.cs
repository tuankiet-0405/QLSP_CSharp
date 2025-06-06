using Microsoft.EntityFrameworkCore;
using THLTW.Data;
using THLTW.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace THLTW.Repositories
{
    public class EFProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public EFProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products
                                 .Include(p => p.Category)
                                 .Include(p => p.AdditionalImages)
                                 .ToListAsync();
        }

        public async Task<(IEnumerable<Product> Products, int TotalCount)> GetAllAsync(int pageNumber, int pageSize)
        {
            var totalCount = await _context.Products.CountAsync();
            var products = await _context.Products
                                         .Include(p => p.Category)
                                         .Include(p => p.AdditionalImages)
                                         .Skip((pageNumber - 1) * pageSize)
                                         .Take(pageSize)
                                         .ToListAsync();
            return (products, totalCount);
        }

        public async Task<Product?> GetByIdAsync(int id) // Changed to Product?
        {
            return await _context.Products
                                 .Include(p => p.Category)
                                 .Include(p => p.AdditionalImages)
                                 .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product) // product is the existingProduct from the controller, already modified
        {
            // The 'product' entity instance received here has been fetched and modified 
            // in the controller (including its AdditionalImages collection).
            // We tell EF Core to update this entity and its graph.

            // Ensure the entity is tracked and marked as modified.
            // _context.Update() is a good way to handle potentially detached entities or complex graphs.
            _context.Products.Update(product);

            // EF Core's change tracker will compare the state of 'product' (and its AdditionalImages)
            // with its original state (or the database) and generate appropriate INSERT/UPDATE/DELETE SQL.
            // For AdditionalImages:
            // - Images removed from product.AdditionalImages in the controller will be deleted from the DB.
            // - Images added to product.AdditionalImages in the controller (with ProductId set) will be inserted.
            
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                // Manually delete related ProductAdditionalImage entities
                var additionalImages = await _context.ProductAdditionalImages
                                                     .Where(pai => pai.ProductId == id)
                                                     .ToListAsync();
                if (additionalImages.Any())
                {
                    _context.ProductAdditionalImages.RemoveRange(additionalImages);
                }
                
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<(IEnumerable<Product> Products, int TotalCount)> GetFilteredAsync(int pageNumber, int pageSize, int? categoryId, decimal? minPrice, decimal? maxPrice, string? searchTerm)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.AdditionalImages)
                .AsQueryable();

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

            var totalCount = await query.CountAsync();
            var products = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (products, totalCount);
        }
    }
}
