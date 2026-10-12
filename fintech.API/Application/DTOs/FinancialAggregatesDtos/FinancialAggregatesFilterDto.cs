using fintech.API.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace fintech.API.Application.DTOs.FinancialAggregatesDtos
{
    public class FinancialAggregatesFilterDto
    {
        [FromQuery(Name = "period")]
        public TransactionPeriod AggregatesPeriod { get; set; }

        [FromQuery(Name = "date")]
        public DateTime Date { get; set; }
    }
}
