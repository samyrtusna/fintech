using fintech.API.Domain.Entities;
using fintech.API.Domain.Enums;
using fintech.API.Infrastructure.EFcore.ContextDb;
using Microsoft.EntityFrameworkCore;

namespace fintech.API.Infrastructure.DataSeeders
{
    public static class CategorySeeder
    {

            public static async Task SeedAsync(AppDbContext context)
            {
                await context.Database.MigrateAsync();

                if (await context.Categories.AnyAsync(c => c.IsSystem))
                    return;

                // ===== ROOT CATEGORIES =====
                var incomesId = Guid.Parse("11111111-1111-1111-1111-111111111111");
                var expensesId = Guid.Parse("22222222-2222-2222-2222-222222222222");
                var investmentsId = Guid.Parse("33333333-3333-3333-3333-333333333333");
                var debtsId = Guid.Parse("44444444-4444-4444-4444-444444444444");

                var categories = new List<Category>
            {
                new() { Id = incomesId, Name = "Incomes", Type = FinancialType.Income, IsSystem = true },
                new() { Id = expensesId, Name = "Expenses", Type = FinancialType.Expense, IsSystem = true },
                new() { Id = investmentsId, Name = "Investments", Type = FinancialType.Investment, IsSystem = true },
                new() { Id = debtsId, Name = "Debts", Type = FinancialType.Debt, IsSystem = true },
            };

                // ===== INCOME CATEGORIES =====
                categories.AddRange(
                [
                Create("Salary", FinancialType.Income, incomesId),
                Create("Bonus", FinancialType.Income, incomesId),
                Create("Freelance", FinancialType.Income, incomesId),
                Create("Business Income", FinancialType.Income, incomesId),
                Create("Rental Income", FinancialType.Income, incomesId),
                Create("Dividends", FinancialType.Income, incomesId),
                Create("Interest Income", FinancialType.Income, incomesId),
                Create("Capital Gains", FinancialType.Income, incomesId),
                Create("Gifts Received", FinancialType.Income, incomesId),
                Create("Refunds", FinancialType.Income, incomesId),
                Create("Government Benefits", FinancialType.Income, incomesId),
                Create("Other Income", FinancialType.Income, incomesId),
            ]);

                // ===== EXPENSE CATEGORIES =====
                var housingId = Guid.NewGuid();
                var livingId = Guid.NewGuid();
                var transportId = Guid.NewGuid();
                var healthId = Guid.NewGuid();
                var educationId = Guid.NewGuid();
                var lifestyleId = Guid.NewGuid();

                categories.AddRange(
                [
                new Category { Id = housingId, Name = "Housing", Type = FinancialType.Expense, ParentCategoryId = expensesId, IsSystem = true },
                new Category { Id = livingId, Name = "Living", Type = FinancialType.Expense, ParentCategoryId = expensesId, IsSystem = true },
                new Category { Id = transportId, Name = "Transport", Type = FinancialType.Expense, ParentCategoryId = expensesId, IsSystem = true },
                new Category { Id = healthId, Name = "Health", Type = FinancialType.Expense, ParentCategoryId = expensesId, IsSystem = true },
                new Category { Id = educationId, Name = "Education", Type = FinancialType.Expense, ParentCategoryId = expensesId, IsSystem = true },
                new Category { Id = lifestyleId, Name = "Lifestyle", Type = FinancialType.Expense, ParentCategoryId = expensesId, IsSystem = true },
            ]);

                // Housing
                categories.AddRange(
                [
                Create("Rent", FinancialType.Expense, housingId),
                Create("Mortgage", FinancialType.Expense, housingId),
                Create("Property Tax", FinancialType.Expense, housingId),
                Create("Home Maintenance", FinancialType.Expense, housingId),
                Create("Utilities", FinancialType.Expense, housingId),
            ]);

                // Living
                categories.AddRange(
                [
                Create("Groceries", FinancialType.Expense, livingId),
                Create("Dining Out", FinancialType.Expense, livingId),
                Create("Clothing", FinancialType.Expense, livingId),
                Create("Personal Care", FinancialType.Expense, livingId),
            ]);

                // Transport
                categories.AddRange(
                [
                Create("Fuel", FinancialType.Expense, transportId),
                Create("Public Transport", FinancialType.Expense, transportId),
                Create("Car Maintenance", FinancialType.Expense, transportId),
                Create("Insurance", FinancialType.Expense, transportId),
            ]);

                // Health
                categories.AddRange(
                [
                Create("Medical Expenses", FinancialType.Expense, healthId),
                Create("Pharmacy", FinancialType.Expense, healthId),
                Create("Health Insurance", FinancialType.Expense, healthId),
            ]);

                // Education
                categories.AddRange(
                [
                Create("Tuition", FinancialType.Expense, educationId),
                Create("Books & Courses", FinancialType.Expense, educationId),
            ]);

                // Lifestyle
                categories.AddRange(
                [
                Create("Entertainment", FinancialType.Expense, lifestyleId),
                Create("Subscriptions", FinancialType.Expense, lifestyleId),
                Create("Travel", FinancialType.Expense, lifestyleId),
                Create("Hobbies", FinancialType.Expense, lifestyleId),
                Create("Donations", FinancialType.Expense, lifestyleId),
                Create("Gifts Given", FinancialType.Expense, lifestyleId),
            ]);

                // ===== INVESTMENTS =====
                categories.AddRange(
                [
                Create("Stocks", FinancialType.Investment, investmentsId),
                Create("Bonds", FinancialType.Investment, investmentsId),
                Create("Mutual Funds", FinancialType.Investment, investmentsId),
                Create("ETFs", FinancialType.Investment, investmentsId),
                Create("Real Estate Investment", FinancialType.Investment, investmentsId),
                Create("Cryptocurrency", FinancialType.Investment, investmentsId),
                Create("Retirement Contributions", FinancialType.Investment, investmentsId),
                Create("Savings", FinancialType.Savings, investmentsId),
                Create("Business Investment", FinancialType.Investment, investmentsId),
            ]);

                // ===== DEBTS =====
                categories.AddRange(
                [
                Create("Personal Loan", FinancialType.ContractedLoan, debtsId),
                Create("Credit Card Debt", FinancialType.ContractedLoan, debtsId),
                Create("Mortgage Loan", FinancialType.ContractedLoan, debtsId),
                Create("Student Loan", FinancialType.ContractedLoan, debtsId),
                Create("Car Loan", FinancialType.ContractedLoan, debtsId), 
                Create("Loan Repayment", FinancialType.PrincipalRepayment, debtsId), 
                Create("Credit Card Payment", FinancialType.PrincipalRepayment, debtsId),
                Create("Interest Paid", FinancialType.InterestPayment, debtsId),
            ]);

                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }

            private static Category Create(string name, FinancialType type, Guid parentId)
            {
                return new Category
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    Type = type,
                    ParentCategoryId = parentId,
                    IsSystem = true
                };
            }
        }
    }