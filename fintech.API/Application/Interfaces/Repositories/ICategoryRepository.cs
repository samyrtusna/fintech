using fintech.API.Domain.Entities;

namespace fintech.API.Application.Interfaces.Repositories
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<Category?> GetAsync(Guid id);
        Task<IEnumerable<Category>> GetByUserAsync(Guid userId);
        Task<bool> CategoryExistAsync(string name, Guid userId); 
        Task<IEnumerable<Category>> GetSubCategoriesAsync(Guid categoryId, Guid userId); 
    }
}
