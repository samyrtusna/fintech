using fintech.API.Application.DTOs.QueryDtos;

namespace fintech.API.Application.DTOs.FinancialTransactionDtos
{
    public class FinancialTransactionFilterDto : PaginationDto
    {
        public int? Year { get; set; }
        public int? Month { get; set; } 
        public Guid? CategoryId { get; set; }
        public bool? IsEssential { get; set; } 
    }
} 
  