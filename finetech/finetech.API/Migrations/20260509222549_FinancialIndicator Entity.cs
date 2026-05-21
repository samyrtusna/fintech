using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace finetech.Migrations
{
    /// <inheritdoc />
    public partial class FinancialIndicatorEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EssentialRation",
                table: "FinancialIntelligenceSnapshots");

            migrationBuilder.DropColumn(
                name: "SavingsRate",
                table: "FinancialIntelligenceSnapshots");

            migrationBuilder.RenameColumn(
                name: "TransactionCount",
                table: "FinancialIntelligenceSnapshots",
                newName: "TransactionsCount");

            migrationBuilder.RenameColumn(
                name: "TotalIncome",
                table: "FinancialIntelligenceSnapshots",
                newName: "TotalRepayedDebts");

            migrationBuilder.RenameColumn(
                name: "TopCategoryAmount",
                table: "FinancialIntelligenceSnapshots",
                newName: "TotalPaidInterests");

            migrationBuilder.RenameColumn(
                name: "TopCategory",
                table: "FinancialIntelligenceSnapshots",
                newName: "TopSpendingCategory");

            migrationBuilder.AddColumn<decimal>(
                name: "TopSpendingCategoryAmount",
                table: "FinancialIntelligenceSnapshots",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalContractedDebts",
                table: "FinancialIntelligenceSnapshots",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalEssentialExpenses",
                table: "FinancialIntelligenceSnapshots",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalIncomes",
                table: "FinancialIntelligenceSnapshots",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalInvestments",
                table: "FinancialIntelligenceSnapshots",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "FinancialIndicators",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    SnapshotId = table.Column<Guid>(type: "uuid", nullable: false),
                    PeriodStart = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PeriodEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PeriodType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Value = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
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
                name: "IX_FinancialIndicators_SnapshotId",
                table: "FinancialIndicators",
                column: "SnapshotId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FinancialIndicators");

            migrationBuilder.DropColumn(
                name: "TopSpendingCategoryAmount",
                table: "FinancialIntelligenceSnapshots");

            migrationBuilder.DropColumn(
                name: "TotalContractedDebts",
                table: "FinancialIntelligenceSnapshots");

            migrationBuilder.DropColumn(
                name: "TotalEssentialExpenses",
                table: "FinancialIntelligenceSnapshots");

            migrationBuilder.DropColumn(
                name: "TotalIncomes",
                table: "FinancialIntelligenceSnapshots");

            migrationBuilder.DropColumn(
                name: "TotalInvestments",
                table: "FinancialIntelligenceSnapshots");

            migrationBuilder.RenameColumn(
                name: "TransactionsCount",
                table: "FinancialIntelligenceSnapshots",
                newName: "TransactionCount");

            migrationBuilder.RenameColumn(
                name: "TotalRepayedDebts",
                table: "FinancialIntelligenceSnapshots",
                newName: "TotalIncome");

            migrationBuilder.RenameColumn(
                name: "TotalPaidInterests",
                table: "FinancialIntelligenceSnapshots",
                newName: "TopCategoryAmount");

            migrationBuilder.RenameColumn(
                name: "TopSpendingCategory",
                table: "FinancialIntelligenceSnapshots",
                newName: "TopCategory");

            migrationBuilder.AddColumn<decimal>(
                name: "EssentialRation",
                table: "FinancialIntelligenceSnapshots",
                type: "numeric(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SavingsRate",
                table: "FinancialIntelligenceSnapshots",
                type: "numeric(5,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
