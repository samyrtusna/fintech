using fintech.API.Application.Interfaces.Repositories;
using fintech.API.Domain.Entities;
using fintech.API.Infrastructure.EFcore.ContextDb;
using Microsoft.EntityFrameworkCore;

namespace fintech.API.Infrastructure.Repositories
{
    public class OptionsRepository(AppDbContext context) : GenericRepository<Option>(context), IOptionsRepository
    {
        public async Task<Option?> GetByKey (string key)
        {
            return await context.Options
                .Where(o => o.Key == key)
                .FirstOrDefaultAsync();
        }
    }
}
