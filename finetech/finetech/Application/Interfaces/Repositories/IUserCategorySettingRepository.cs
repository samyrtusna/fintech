using fintech.Domain.Entities;

namespace fintech.Application.Interfaces.Repositories
{
    public interface IUserCategorySettingRepository : IGenericRepository<UserCategorySetting>
    {
        Task<UserCategorySetting?> GetByUserIdAsync(Guid userId, Guid categoryId);
    }
}
