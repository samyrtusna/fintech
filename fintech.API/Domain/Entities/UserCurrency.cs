using fintech.API.Domain.ValueObjects;

namespace fintech.API.Domain.Entities
{
    public class UserCurrency
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public Currency Currency { get; set; } = null!;  
        public bool IsDefault { get; set; } = false;
        public bool IsDeleted { get; set; } = false; 
        public User User { get; set; } = null!;
    }
}
