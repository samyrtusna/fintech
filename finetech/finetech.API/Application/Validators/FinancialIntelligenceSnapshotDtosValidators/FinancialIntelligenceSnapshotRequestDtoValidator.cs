using fintech.API.Application.DTOs.FinancialIntelligenceSnapshotDtos;
using FluentValidation;

namespace fintech.API.Application.Validators.FinancialIntelligenceSnapshotDtosValidators
{
    public class FinancialIntelligenceSnapshotRequestDtoValidator : AbstractValidator<FinancialIntelligenceSnapshotRequestDto>
    {
        public FinancialIntelligenceSnapshotRequestDtoValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required.");
            RuleFor(x => x.PeriodStart).LessThanOrEqualTo(x => x.PeriodEnd).WithMessage("PeriodStart must be less than or equal to PeriodEnd.");
            RuleFor(x => x.PeriodEnd).GreaterThanOrEqualTo(x => x.PeriodStart).WithMessage("PeriodEnd must be greater than or equal to PeriodStart.");
            RuleFor(x => x.PeriodType).IsInEnum().WithMessage("Invalid PeriodType type.");
            RuleFor(x => x.TotalIncomes).NotEmpty().WithMessage("TotalIncomes is required.")
                .GreaterThanOrEqualTo(0).WithMessage("TotalIncomes must be non-negative.")
                .PrecisionScale(18, 2, true).WithMessage("TotalIncomes must have max 2 decimal places.");
            RuleFor(x => x.TotalExpenses).NotEmpty().WithMessage("TotalExpenses is required.")
                .GreaterThanOrEqualTo(0).WithMessage("TotalExpenses must be non-negative.")
                .PrecisionScale(18, 2, true).WithMessage("TotalExpenses must have max 2 decimal places.");
            RuleFor(x => x.NetBalance).NotEmpty().WithMessage("NetBalance is required.")
                .PrecisionScale(18, 2, true).WithMessage("NetBalance must have max 2 decimal places.");
            RuleFor(x => x.CashFlow).NotEmpty().WithMessage("CashFlow is required.")
                .PrecisionScale(18, 2, true).WithMessage("CashFlow must have max 2 decimal places.");
            RuleFor(x => x.TotalNetCashFlow).NotEmpty().WithMessage("TotalNetCashFlow is required.")
                .PrecisionScale(18, 2, true).WithMessage("CashFlow must have max 2 decimal places.");
            RuleFor(x => x.NetSavings).NotEmpty().WithMessage("NetSavings is required.")
                .GreaterThanOrEqualTo(0).WithMessage("NetSavings must be non-negative.")
                .PrecisionScale(18, 2, true).WithMessage("NetSavings must have max 2 decimal places.");
            RuleFor(x => x.TotalInvestments).NotEmpty().WithMessage("TotalInvestments is required.")
                .GreaterThanOrEqualTo(0).WithMessage("TotalInvestments must be non-negative.")
                .PrecisionScale(18, 2, true).WithMessage("TotalInvestments must have max 2 decimal places.");
            RuleFor(x => x.TotalEssentialExpenses).NotEmpty().WithMessage("TotalEssentialExpenses is required.")
                .GreaterThanOrEqualTo(0).WithMessage("TotalEssentialExpenses must be non-negative.")
                .PrecisionScale(18, 2, true).WithMessage("TotalEssentialExpenses must have max 2 decimal places.");
            RuleFor(x => x.TotalContractedDebts).NotEmpty().WithMessage("TotalContractedDebts is required.")
                .GreaterThanOrEqualTo(0).WithMessage("TotalContractedDebts must be non-negative.")
                .PrecisionScale(18, 2, true).WithMessage("TotalContractedDebts must have max 2 decimal places.");
            RuleFor(x => x.TotalRepayedDebts).NotEmpty().WithMessage("TotalRepayedDebts is required.")
                .GreaterThanOrEqualTo(0).WithMessage("TotalRepayedDebts must be non-negative.")
                .PrecisionScale(18, 2, true).WithMessage("TotalRepayedDebts must have max 2 decimal places.");
            RuleFor(x => x.TotalPaidInterests).NotEmpty().WithMessage("TotalPaidInterests is required.")
                .GreaterThanOrEqualTo(0).WithMessage("TotalPaidInterests must be non-negative.")
                .PrecisionScale(18, 2, true).WithMessage("TotalPaidInterests must have max 2 decimal places.");
            RuleFor(x => x.TopSpendingCategory).NotEmpty().WithMessage("TopSpendingCategory is required.")
                .MaximumLength(100).WithMessage("TopSpendingCategory must be at most 100 characters long.");
            RuleFor(x => x.TopSpendingCategoryAmount).NotEmpty().WithMessage("TopSpendingCategoryAmount is required.")
                .GreaterThanOrEqualTo(0).WithMessage("TopSpendingCategoryAmount must be non-negative.")
                .PrecisionScale(18, 2, true).WithMessage("TopSpendingCategoryAmount must have max 2 decimal places.");
            RuleFor(x => x.TransactionsCount).NotEmpty().WithMessage("TransactionsCount is required.")
                .GreaterThanOrEqualTo(0).WithMessage("TransactionsCount must be non-negative.");
            RuleFor(x => x.AvgTransactionAmount).NotEmpty().WithMessage("AvgTransactionAmount is required.")
                .GreaterThanOrEqualTo(0).WithMessage("AvgTransactionAmount must be non-negative.")
                .PrecisionScale(18, 2, true).WithMessage("AvgTransactionAmount must have max 2 decimal places.");
        }
    }
}
