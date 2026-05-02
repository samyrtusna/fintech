namespace fintech.Application.DTOs.TransactionDtos
{
    public class UpdateTransactionRequestDto
    {
        public string? Description { get; set; }
        public bool? IsEssential { get; set; }
    }
}
