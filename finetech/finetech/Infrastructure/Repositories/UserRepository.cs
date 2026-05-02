using fintech.Application.Interfaces.Repositories;
using fintech.Domain.Entities;
using fintech.Infrastructure.EFcore.ContextDb;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace fintech.Infrastructure.Repositories
{
    public class UserRepository(AppDbContext context) : GenericRepository<User>(context), IUserRepository 
    { 

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
