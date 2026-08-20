using fintech.API.Domain.Entities;

namespace fintech.API.Application.Interfaces.Repositories
{
    public interface IOptionsRepository : IGenericRepository<Options>
    {
        Task<Options?> GetByKey(string key);
    }
}
