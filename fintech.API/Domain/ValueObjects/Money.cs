namespace fintech.API.Domain.ValueObjects
{
    public sealed class Money
    {
        public decimal Amount { get; private set; }
        public Currency? Currency { get; private set; }

        private Money() { } // For EF Core

        public Money(decimal amount, Currency? currency)
        {  
            Amount = amount;
            Currency = currency ?? null;
            
        }
    }
}
