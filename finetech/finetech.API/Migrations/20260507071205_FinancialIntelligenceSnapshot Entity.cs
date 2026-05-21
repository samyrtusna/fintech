using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace finetech.Migrations
{
    /// <inheritdoc />
    public partial class FinancialIntelligenceSnapshotEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "FinancialTransactions",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "FinancialIntelligenceSnapshot",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PeriodStart = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PeriodEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TotalIncome = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalExpenses = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    NetBalance = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CashFlow = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    NetSavings = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    SavingsRate = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    EssentialRation = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    TopCategory = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TopCategoryAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TransactionCount = table.Column<int>(type: "integer", nullable: false),
                    AvgTransactionAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialIntelligenceSnapshot", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FinancialIntelligenceSnapshot");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "FinancialTransactions");
        }
    }
}
