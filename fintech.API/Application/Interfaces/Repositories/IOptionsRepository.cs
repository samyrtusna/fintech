using fintech.API.Domain.Entities;

namespace fintech.API.Application.Interfaces.Repositories
{
    public interface IOptionsRepository : IGenericRepository<Option>
    {
        Task<Option?> GetByKeyAsync(string key); 
    }
}
