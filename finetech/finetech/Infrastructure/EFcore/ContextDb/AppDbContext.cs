using fintech.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace fintech.Infrastructure.EFcore.ContextDb
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet <UserCategorySetting> UserCategoriesSettings { get; set; }
        public DbSet <FinancialTransaction> FinancialTransactions { get; set; }  
    }
}
