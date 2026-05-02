using fintech.Application.DTOs.CategoryDtos;
using fintech.Application.Interfaces.Repositories;
using fintech.Domain.Entities;
using fintech.Domain.Enums;
using fintech.Infrastructure.EFcore.ContextDb;
using Microsoft.EntityFrameworkCore;

namespace fintech.Infrastructure.Repositories
{
    public class CategoryRepository(AppDbContext context) : GenericRepository<Category>(context), ICategoryRepository
    {
        public async Task<Category?> GetAsync(Guid id)
        {
            return await context.Categories
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

        public async Task<IEnumerable<Category>> GetByUserIdAsync(Guid userId)
        {
            return await context.Categories
                .Where(c => (c.IsSystem || c.UserId == userId) && !c.IsDeleted)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> CategoryExistAsync (string name, Guid userId)
        {
            return await context.Categories
                .AsNoTracking()
                .AnyAsync(c => c.Name == name && (c.IsSystem || c.UserId == userId) && !c.IsDeleted);
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
