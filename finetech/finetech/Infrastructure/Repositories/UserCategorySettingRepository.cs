using fintech.Application.Interfaces.Repositories;
using fintech.Domain.Entities;
using fintech.Infrastructure.EFcore.ContextDb;
using Microsoft.EntityFrameworkCore;

namespace fintech.Infrastructure.Repositories
{
    public class UserCategorySettingRepository(AppDbContext context) : GenericRepository<UserCategorySetting>(context), IUserCategorySettingRepository
    {
        public async Task<UserCategorySetting?> GetByUserIdAsync(Guid userId, Guid categoryId)
        {
            return await context.UserCategoriesSettings.FirstOrDefaultAsync(u => u.UserId == userId && u.CategoryId == categoryId);
        }
    }
}
