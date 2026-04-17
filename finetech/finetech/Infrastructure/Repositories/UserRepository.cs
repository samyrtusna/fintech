using fintech.Application.Interfaces.Repositories;
using fintech.Domain.Entities;
using fintech.Infrastructure.EFcore.ContextDb;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace fintech.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository 
    { 
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
