using fintech.API.Application.DTOs.QueryDtos;
using fintech.API.Domain.Enums;

namespace fintech.API.Application.DTOs.FinancialTransactionDtos
{
    public class FinancialTransactionFilterDto : PaginationDto
    {
        public DateTime? Date { get; set; }
        public Guid? CategoryId { get; set; }
        public bool? IsEssential { get; set; }  
    }
} 
  