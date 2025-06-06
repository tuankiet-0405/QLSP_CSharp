using THLTW.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace THLTW.Repositories
{
    public class MockCategoryRepository : ICategoryRepository
    {
        private List<Category> _categories;

        public MockCategoryRepository()
        {
            _categories = new List<Category>
            {
                new Category { Id = 1, Name = "Electronics" },
                new Category { Id = 2, Name = "Clothing" },
                new Category { Id = 3, Name = "Books" }
            };
        }

        public Task<IEnumerable<Category>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<Category>>(_categories);
        }

        public Task<Category?> GetByIdAsync(int id) // Changed to Category?
        {
            return Task.FromResult(_categories.FirstOrDefault(c => c.Id == id));
        }

        public Task AddAsync(Category category)
        {
            category.Id = _categories.Any() ? _categories.Max(c => c.Id) + 1 : 1; // Handle empty list
            _categories.Add(category);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Category category)
        {
            var existingCategory = _categories.FirstOrDefault(c => c.Id == category.Id);
            if (existingCategory != null)
            {
                existingCategory.Name = category.Name;
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(int id)
        {
            var category = _categories.FirstOrDefault(c => c.Id == id);
            if (category != null)
            {
                _categories.Remove(category);
            }
            return Task.CompletedTask;
        }
    }
}
