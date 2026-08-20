using fintech.API.Application.Interfaces.Repositories;
using fintech.API.Domain.Entities;
using fintech.API.Infrastructure.EFcore.ContextDb;
using Microsoft.EntityFrameworkCore;

namespace fintech.API.Infrastructure.Repositories
{
    public class OptionsRepository(AppDbContext context) : GenericRepository<Options>(context), IOptionsRepository
    {
        public async Task<Options?> GetByKey (string key)
        {
            return await context.Options
                .Where(o => o.Key == key)
                .FirstOrDefaultAsync();
        }
    }
}
