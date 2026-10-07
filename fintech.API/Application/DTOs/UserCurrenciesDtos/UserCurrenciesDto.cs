namespace fintech.API.Application.DTOs.UserCurrenciesDtos
{
    public class UserCurrenciesDto
    {
        public string CurrencySymbol { get; set; } = null!;
        public string CurrencyCode { get; set; } = null!; 
        public bool IsDefault { get; set; } = false;
    }
}
