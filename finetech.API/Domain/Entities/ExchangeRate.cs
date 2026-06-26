namespace fintech.API.Domain.Entities
{
    public class ExchangeRate
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FromCurrency { get; set; } = null!;
        public string ToCurrency { get; set; } = null!;
        public decimal Rate { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow; 
    }
}
