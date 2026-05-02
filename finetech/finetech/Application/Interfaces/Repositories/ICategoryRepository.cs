using fintech.Domain.Entities;
using fintech.Domain.Enums;

namespace fintech.Application.Interfaces.Repositories
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<Category?> GetAsync(Guid id);
        Task<IEnumerable<Category>> GetByUserIdAsync(Guid userId);
        Task<bool> CategoryExistAsync(string name, Guid userId);
        Task<IEnumerable<Category>> GetSubCategoriesAsync(Guid categoryId, Guid userId); 
    }
}
