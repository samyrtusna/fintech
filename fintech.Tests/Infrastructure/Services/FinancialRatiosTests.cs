using fintech.API.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fintech.Tests.Infrastructure.Services
{
    public class FinancialRatiosTests
    {
        private readonly FinancialRatiosService _service;

        public FinancialRatiosTests()
        {
            _service = new FinancialRatiosService();
        }

        [Fact]
        public void GetNetBalance_ShouldReturnIncomeMinusExpense()
        {
            // Arrange
            decimal totalIncome = 5000;
            decimal totalExpense = 3000;

            // Act
            var result = _service.GetNetBalance(totalIncome, totalExpense);

            // Assert
            Assert.Equal(2000, result);
        }

        [Fact]
        public void GetNetCashFlow_ShouldReturnCorrectValue()
        {
            // Arrange
            decimal totalIncome = 5000;
            decimal totalExpense = 2000;
            decimal totalInvestment = 500;
            decimal totalSavings = 300;
            decimal contractedLoans = 1000;
            decimal interestPayment = 100;
            decimal principalRepayment = 400;

            // Act
            var result = _service.GetNetCashFlow(
                totalIncome,
                totalExpense,
                totalInvestment,
                totalSavings,
                contractedLoans,
                interestPayment,
                principalRepayment);

            // Assert
            Assert.Equal(2700, result);
        }

        [Fact]
        public void GetTotalOutflow_ShouldReturnSumOfAllOutflows()
        {
            // Arrange
            decimal totalExpense = 2000;
            decimal totalInvestment = 500;
            decimal totalSavings = 300;
            decimal interestPayment = 100;
            decimal principalRepayment = 400;

            // Act
            var result = _service.GetTotalOutflow(
                totalExpense,
                totalInvestment,
                totalSavings,
                interestPayment,
                principalRepayment);

            // Assert
            Assert.Equal(3300, result);
        }

        [Fact]
        public void GetTotalInflow_ShouldReturnIncomePlusContractedLoans()
        {
            // Arrange
            decimal totalIncome = 5000;
            decimal contractedLoans = 1000;

            // Act
            var result = _service.GetTotalInflow(
                totalIncome,
                contractedLoans);

            // Assert
            Assert.Equal(6000, result);
        }

        [Fact]
        public void GetTotalActiveDebt_ShouldReturnContractedLoansMinusPrincipalRepayment()
        {
            // Arrange
            decimal totalContractedLoans = 10000;
            decimal totalPrincipalRepayment = 2500;

            // Act
            var result = _service.GetTotalActiveDebt(
                totalContractedLoans,
                totalPrincipalRepayment);

            // Assert
            Assert.Equal(7500, result);
        }

        [Fact]
        public void GetExpenseRatio_ShouldReturnExpensePercentageOfIncome()
        {
            // Arrange
            decimal totalIncome = 5000;
            decimal totalExpense = 2000;

            // Act
            var result = _service.GetExpenseRatio(
                totalIncome,
                totalExpense);

            // Assert
            Assert.Equal(40, result);
        }

        [Fact]
        public void GetExpenseRatio_ShouldReturnZero_WhenIncomeIsZero()
        {
            // Arrange
            decimal totalIncome = 0;
            decimal totalExpense = 2000;

            // Act
            var result = _service.GetExpenseRatio(
                totalIncome,
                totalExpense);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void GetNonEssentialExpenseRatio_ShouldReturnNonEssentialPercentage()
        {
            // Arrange
            decimal totalExpense = 4000;
            decimal nonEssentialExpenses = 1000;

            // Act
            var result = _service.GetNonEssentialExpenseRatio(
                totalExpense,
                nonEssentialExpenses);

            // Assert
            Assert.Equal(25, result);
        }

        [Fact]
        public void GetNonEssentialExpenseRatio_ShouldReturnZero_WhenExpenseIsZero()
        {
            // Arrange
            decimal totalExpense = 0;
            decimal nonEssentialExpenses = 1000;

            // Act
            var result = _service.GetNonEssentialExpenseRatio(
                totalExpense,
                nonEssentialExpenses);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void GetInvestmentRate_ShouldReturnInvestmentPercentageOfIncome()
        {
            // Arrange
            decimal totalIncome = 10000;
            decimal totalInvestment = 2000;

            // Act
            var result = _service.GetInvestmentRate(
                totalIncome,
                totalInvestment);

            // Assert
            Assert.Equal(20, result);
        }

        [Fact]
        public void GetInvestmentRate_ShouldReturnZero_WhenIncomeIsZero()
        {
            // Arrange
            decimal totalIncome = 0;
            decimal totalInvestment = 2000;

            // Act
            var result = _service.GetInvestmentRate(
                totalIncome,
                totalInvestment);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void GetSavingsRate_ShouldReturnSavingsPercentageOfIncome()
        {
            // Arrange
            decimal totalIncome = 10000;
            decimal totalSavings = 1500;

            // Act
            var result = _service.GetSavingsRate(
                totalIncome,
                totalSavings);

            // Assert
            Assert.Equal(15, result);
        }

        [Fact]
        public void GetSavingsRate_ShouldReturnZero_WhenIncomeIsZero()
        {
            // Arrange
            decimal totalIncome = 0;
            decimal totalSavings = 1500;

            // Act
            var result = _service.GetSavingsRate(
                totalIncome,
                totalSavings);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void GetIndicatorGrowth_ShouldReturnPositiveGrowth()
        {
            // Arrange
            decimal previousValue = 100;
            decimal currentValue = 120;

            // Act
            var result = _service.GetIndicatorGrowth(
                previousValue,
                currentValue);

            // Assert
            Assert.Equal(20, result);
        }

        [Fact]
        public void GetIndicatorGrowth_ShouldReturnNegativeGrowth()
        {
            // Arrange
            decimal previousValue = 200;
            decimal currentValue = 150;

            // Act
            var result = _service.GetIndicatorGrowth(
                previousValue,
                currentValue);

            // Assert
            Assert.Equal(-25, result);
        }

        [Fact]
        public void GetIndicatorGrowth_ShouldReturnZero_WhenPreviousValueIsZero()
        {
            // Arrange
            decimal previousValue = 0;
            decimal currentValue = 100;

            // Act
            var result = _service.GetIndicatorGrowth(
                previousValue,
                currentValue);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void GetDebtPaymentRatio_ShouldReturnDebtPaymentPercentageOfIncome()
        {
            // Arrange
            decimal totalIncome = 10000;
            decimal totalPrincipalPayment = 1500;
            decimal totalInterestPayment = 500;

            // Act
            var result = _service.GetDebtPaymentRatio(
                totalIncome,
                totalPrincipalPayment,
                totalInterestPayment);

            // Assert
            Assert.Equal(20, result);
        }

        [Fact]
        public void GetDebtPaymentRatio_ShouldReturnZero_WhenIncomeIsZero()
        {
            // Arrange
            decimal totalIncome = 0;
            decimal totalPrincipalPayment = 1500;
            decimal totalInterestPayment = 500;

            // Act
            var result = _service.GetDebtPaymentRatio(
                totalIncome,
                totalPrincipalPayment,
                totalInterestPayment);

            // Assert
            Assert.Equal(0, result);
        }
    }
}
