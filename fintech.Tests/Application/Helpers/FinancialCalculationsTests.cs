using fintech.API.Application.Helpers;

namespace fintech.Tests.Application.Helpers
{
    public class FinancialCalculationsTests
    {
        private readonly FinancialCalculations _calculations = new();


        [Fact]
        public void NetBalance_Should_Return_Income_Minus_Expenses()
        {
            var result = _calculations.GetNetBalance(10000m, 4000m);

            Assert.Equal(6000m, result);
        }

        [Fact]
        public void NetBalance_Should_Return_Negative_Value_When_Expenses_Exceed_Income()
        {
            var result = _calculations.GetNetBalance(4000m, 6000m);

            Assert.Equal(-2000m, result);
        }


        [Fact]
        public void NetCashFlow_Should_Return_Calculated_Value()
        {
            var result = _calculations.GetNetCashFlow(
                10000m, // income
                3000m,  // expense
                1000m,  // investment
                500m,   // savings
                2000m,  // contracted loans
                300m,   // interest
                700m);  // principal

            Assert.Equal(6500m, result);
        }


        [Fact]
        public void TotalInflow_Should_Return_Income_Plus_Contracted_Loans()
        {
            var result = _calculations.GetTotalInflow(
                10000m,
                2000m);

            Assert.Equal(12000m, result);
        }


        [Fact]
        public void TotalOutflow_Should_Return_All_Outflows()
        {
            var result = _calculations.GetTotalOutflow(
                3000m,  // expenses
                1000m,  // investments
                500m,   // savings
                200m,   // interest
                800m);  // principal

            Assert.Equal(5500m, result);
        }


        [Fact]
        public void SavingsRate_Should_Return_Calculated_Percentage()
        {
            var result = _calculations.GetSavingsRate(
                10000m, // income
                2000m); // savings

            Assert.Equal(20m, result);
        }

        [Fact]
        public void SavingsRate_Should_Return_Zero_When_Income_Is_Zero()
        {
            var result = _calculations.GetSavingsRate(
                0m,
                2000m);

            Assert.Equal(0m, result);
        }


        [Fact]
        public void EssentialExpensesRatio_Should_Return_Calculated_Percentage()
        {
            var result = _calculations.GetEssentialExpensesRatio(
                10000m, // income
                3000m); // essential expenses

            Assert.Equal(30m, result);
        }

        [Fact]
        public void EssentialExpensesRatio_Should_Return_Zero_When_Income_Is_Zero()
        {
            var result = _calculations.GetEssentialExpensesRatio(
                0m,
                3000m);

            Assert.Equal(0m, result);
        }


        [Fact]
        public void ExpensesToIncomeRatio_Should_Return_Calculated_Percentage()
        {
            var result = _calculations.GetExpensesRatio(
                10000m, // income
                4000m); // expenses

            Assert.Equal(40m, result);
        }

        [Fact]
        public void ExpensesToIncomeRatio_Should_Return_Zero_When_Income_Is_Zero()
        {
            var result = _calculations.GetExpensesRatio(
                0m,
                4000m);

            Assert.Equal(0m, result);
        }


        [Fact]
        public void InvestmentRatio_Should_Return_Calculated_Percentage()
        {
            var result = _calculations.GetInvestmentRatio(
                10000m, // income
                1500m); // investments

            Assert.Equal(15m, result);
        }

        [Fact]
        public void InvestmentRatio_Should_Return_Zero_When_Income_Is_Zero()
        {
            var result = _calculations.GetInvestmentRatio(
                0m,
                1500m);

            Assert.Equal(0m, result);
        }


        [Fact]
        public void DebtToIncomeRatio_Should_Return_Calculated_Percentage()
        {
            var result = _calculations.GetDebtRatio(
                10000m, // income
                5000m); // contracted debts

            Assert.Equal(50m, result);
        }

        [Fact]
        public void DebtToIncomeRatio_Should_Return_Zero_When_Income_Is_Zero()
        {
            var result = _calculations.GetDebtRatio(
                0m,
                5000m);

            Assert.Equal(0m, result);
        }

        [Fact]
        public void DebtRepaymentRatio_Should_Return_Calculated_Percentage()
        {
            var result = _calculations.GetDebtPaymentRatio(
                10000m, // income
                3000m,  // principal payment
                1000m); // interest payment

            Assert.Equal(40m, result);
        }

        [Fact]
        public void DebtRepaymentRatio_Should_Return_Zero_When_Income_Is_Zero()
        {
            var result = _calculations.GetDebtPaymentRatio(
                0m,
                3000m,
                1000m);

            Assert.Equal(0m, result);
        }


        [Fact]
        public void InterestBurdenRatio_Should_Return_Calculated_Percentage()
        {
            var result = _calculations.GetInterestBurdenRatio(
                10000m, // income
                500m);  // paid interests

            Assert.Equal(5m, result);
        }

        [Fact]
        public void InterestBurdenRatio_Should_Return_Zero_When_Income_Is_Zero()
        {
            var result = _calculations.GetInterestBurdenRatio(
                0m,
                500m);

            Assert.Equal(0m, result);
        }


        [Fact]
        public void IndicatorGrowth_Should_Return_Calculated_Percentage()
        {
            var result = _calculations.GetIndicatorGrowth(
                100m,
                120m);

            Assert.Equal(20m, result);
        }

        [Fact]
        public void IndicatorGrowth_Should_Return_Negative_Percentage_When_Value_Decreases()
        {
            var result = _calculations.GetIndicatorGrowth(
                200m,
                150m);

            Assert.Equal(-25m, result);
        }

        [Fact]
        public void IndicatorGrowth_Should_Return_Zero_When_Previous_Value_Is_Zero()
        {
            var result = _calculations.GetIndicatorGrowth(
                0m,
                100m);

            Assert.Equal(0m, result);
        }
    }
}