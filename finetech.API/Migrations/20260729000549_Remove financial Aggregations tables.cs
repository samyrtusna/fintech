using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace finetech.Migrations
{
    /// <inheritdoc />
    public partial class RemovefinancialAggregationstables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExchangeRates");

            migrationBuilder.DropTable(
                name: "FinancialIndicators");

            migrationBuilder.DropTable(
                name: "FinancialIntelligenceSnapshots");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExchangeRates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FromCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Rate = table.Column<decimal>(type: "numeric(18,6)", nullable: false),
                    ToCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeRates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FinancialIntelligenceSnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AvgTransactionAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CashFlow = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    NetBalance = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    NetSavings = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PeriodEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PeriodStart = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PeriodType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    TopSpendingCategory = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TopSpendingCategoryAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalContractedDebts = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalEssentialExpenses = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalExpenses = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalIncomes = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalInvestments = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalNetCashFlow = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalPaidInterests = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalRepayedDebts = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TransactionsCount = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialIntelligenceSnapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FinancialIndicators",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SnapshotId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PeriodEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PeriodStart = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PeriodType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialIndicators", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinancialIndicators_FinancialIntelligenceSnapshots_Snapshot~",
                        column: x => x.SnapshotId,
                        principalTable: "FinancialIntelligenceSnapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRates_FromCurrency_ToCurrency_Date",
                table: "ExchangeRates",
                columns: new[] { "FromCurrency", "ToCurrency", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FinancialIndicators_SnapshotId",
                table: "FinancialIndicators",
                column: "SnapshotId");
        }
    }
}
