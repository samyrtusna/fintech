namespace fintech.API.Domain.Entities
{
    public class UserCurrency
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string CurrencySymbol { get; set; } = string.Empty;
        public string CurrencyCode { get; set; } = string.Empty;    
        public bool IsDefault { get; set; } = false;
        public bool IsDeleted { get; set; } = false; 
        public User User { get; set; } = null!;
    }
}
