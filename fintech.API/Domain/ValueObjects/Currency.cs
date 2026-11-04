namespace fintech.API.Domain.ValueObjects
{
    public sealed class Currency
    {
        public string Code { get; private set; } = null!;
        public string Symbol { get; private set; } = null!;

        private Currency() { } // For EF Core

        public Currency(string code, string symbol)
        {
            if(string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Currency code is required.", nameof(code));
            if(string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException("Currency symbol is required.", nameof(symbol));

            code = code.Trim().ToUpperInvariant();

            if(code.Length != 3 || !code.All(Char.IsAsciiLetter))
            {
                throw new ArgumentException("Currency code must be a 3-letter ISO 4217 code.", nameof(code));
            }

            Code = code;
            Symbol = symbol.Trim();
        }
    }
}
