using fintech.API.Application.Helpers;


namespace fintech.Tests.Application.Helpers
{
    public class FinancialCalculationsTests
    {
        private readonly FinancialCalculations _calculations = new();

        [Fact]
        public void SavingsRate_Should_Return_Calculated_Percentage()
        {
            var result = _calculations.SavingsRate(2000m, 10000m);

            Assert.Equal(20m, result);
        }

        [Fact]
        public void SavingsRate_Should_Return_Zero_When_Income_Is_Zero()
        {
            var result = _calculations.SavingsRate(2000m, 0m);

            Assert.Equal(0m, result);
        }


        [Fact]
        public void EssentialExpensesRatio_Should_Return_Calculated_Percentage()
        {
            var result = _calculations.EssentialExpensesRatio(3000m, 10000m);

            Assert.Equal(30m, result);
        }

        [Fact]
        public void EssentialExpensesRatio_Should_Return_Zero_When_Income_Is_Zero()
        {
            var result = _calculations.EssentialExpensesRatio(3000m, 0m);

            Assert.Equal(0m, result);
        }


        [Fact]
        public void ExpensesToIncomeRatio_Should_Return_Calculated_Percentage()
        {
            var result = _calculations.ExpensesToIncomeRatio(4000m, 10000m);

            Assert.Equal(40m, result);
        }

        [Fact]
        public void ExpensesToIncomeRatio_Should_Return_Zero_When_Income_Is_Zero()
        {
            var result = _calculations.ExpensesToIncomeRatio(4000m, 0m);

            Assert.Equal(0m, result);
        }


        [Fact]
        public void InvestmentRatio_Should_Return_Calculated_Percentage()
        {
            var result = _calculations.InvestmentRatio(1500m, 10000m);

            Assert.Equal(15m, result);
        }

        [Fact]
        public void InvestmentRatio_Should_Return_Zero_When_Income_Is_Zero()
        {
            var result = _calculations.InvestmentRatio(1500m, 0m);

            Assert.Equal(0m, result);
        }


        [Fact]
        public void DebtToIncomeRatio_Should_Return_Calculated_Percentage()
        {
            var result = _calculations.DebtToIncomeRatio(5000m, 10000m);

            Assert.Equal(50m, result);
        }

        [Fact]
        public void DebtToIncomeRatio_Should_Return_Zero_When_Income_Is_Zero()
        {
            var result = _calculations.DebtToIncomeRatio(5000m, 0m);

            Assert.Equal(0m, result);
        }


        [Fact]
        public void CashRetentionRatio_Should_Return_Calculated_Percentage()
        {
            var result = _calculations.CashRetentionRatio(2500m, 10000m);

            Assert.Equal(25m, result);
        }

        [Fact]
        public void CashRetentionRatio_Should_Return_Zero_When_Income_Is_Zero()
        {
            var result = _calculations.CashRetentionRatio(2500m, 0m);

            Assert.Equal(0m, result);
        }


        [Fact]
        public void DebtRepaymentRatio_Should_Return_Calculated_Percentage()
        {
            var result = _calculations.DebtRepaymentRatio(2000m, 5000m);

            Assert.Equal(40m, result);
        }

        [Fact]
        public void DebtRepaymentRatio_Should_Return_Zero_When_Contracted_Debts_Are_Zero()
        {
            var result = _calculations.DebtRepaymentRatio(2000m, 0m);

            Assert.Equal(0m, result);
        }


        [Fact]
        public void InterestBurdenRatio_Should_Return_Calculated_Percentage()
        {
            var result = _calculations.InterestBurdenRatio(500m, 10000m);

            Assert.Equal(5m, result);
        }

        [Fact]
        public void InterestBurdenRatio_Should_Return_Zero_When_Income_Is_Zero()
        {
            var result = _calculations.InterestBurdenRatio(500m, 0m);

            Assert.Equal(0m, result);
        }
    }
}
