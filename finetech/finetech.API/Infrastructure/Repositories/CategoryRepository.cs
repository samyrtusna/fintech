using fintech.API.Application.Interfaces.Repositories;
using fintech.API.Domain.Entities;
using fintech.API.Infrastructure.EFcore.ContextDb;
using Microsoft.EntityFrameworkCore;

namespace fintech.API.Infrastructure.Repositories
{
    public class CategoryRepository(AppDbContext context) : GenericRepository<Category>(context), ICategoryRepository
    {
        public async Task<Category?> GetAsync(Guid id)
        {
            return await context.Categories
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

        public async Task<IEnumerable<Category>> GetByUserAsync(Guid userId) 
        {
            return await context.Categories
                .Where(c => (c.IsSystem || c.UserId == userId) && !c.IsDeleted)
                .Select(c => new Category
                {
                    Id = c.Id,
                    Name = c.Name,
                    Type = c.Type,
                    ParentCategoryId = c.ParentCategoryId,
                })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> CategoryExistAsync (string name, Guid userId)
        {
            return await context.Categories
                .AsNoTracking()
                .AnyAsync(c => c.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase) && (c.IsSystem || c.UserId == userId) && !c.IsDeleted);
        }

        public async Task<IEnumerable<Category>> GetSubCategoriesAsync(Guid categoryId, Guid userId)
        {
            return await context.Categories
                .Where(c => c.ParentCategoryId == categoryId && (c.IsSystem || c.UserId == userId) && !c.IsDeleted)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
