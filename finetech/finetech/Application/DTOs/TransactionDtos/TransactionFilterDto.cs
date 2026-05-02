namespace fintech.Application.DTOs.TransactionDtos
{
    public class TransactionFilterDto
    {
        public int? Year { get; set; }
        public int? Month { get; set; }
        public Guid? CategoryId { get; set; }
        public bool? IsEssential { get; set; } 
    }
}
