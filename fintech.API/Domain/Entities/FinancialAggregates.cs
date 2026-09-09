using fintech.API.Domain.Enums;
using Microsoft.AspNetCore.SignalR;

namespace fintech.API.Domain.Entities
{
    public class FinancialAggregates
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public int Year { get; set; }
        public AggregateName Name { get; set; } 
        public decimal Value { get; set; }
    }
}
