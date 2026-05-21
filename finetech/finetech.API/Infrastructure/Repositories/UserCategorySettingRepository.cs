using fintech.API.Application.Interfaces.Repositories;
using fintech.API.Domain.Entities;
using fintech.API.Infrastructure.EFcore.ContextDb;
using Microsoft.EntityFrameworkCore;

namespace fintech.API.Infrastructure.Repositories
{
    public class UserCategorySettingRepository(AppDbContext context) : GenericRepository<UserCategorySetting>(context), IUserCategorySettingRepository
    {
        public async Task<UserCategorySetting?> GetByUserAsync(Guid userId, Guid categoryId)
        {
            return await context.UserCategoriesSettings.FirstOrDefaultAsync(u => u.UserId == userId && u.CategoryId == categoryId);
        }
    } 
}
