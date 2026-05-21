using fintech.API.Domain.Entities;

namespace fintech.API.Application.Interfaces.Repositories
{
    public interface IUserCategorySettingRepository : IGenericRepository<UserCategorySetting>
    {
        Task<UserCategorySetting?> GetByUserAsync(Guid userId, Guid categoryId); 
    }
}
