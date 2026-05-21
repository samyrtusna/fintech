using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace finetech.Migrations
{
    /// <inheritdoc />
    public partial class FinancialIntelligenceSnapshotEntityrefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FinancialIntelligenceSnapshot",
                table: "FinancialIntelligenceSnapshot");

            migrationBuilder.RenameTable(
                name: "FinancialIntelligenceSnapshot",
                newName: "FinancialIntelligenceSnapshots");

            migrationBuilder.AddColumn<string>(
                name: "PeriodType",
                table: "FinancialIntelligenceSnapshots",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FinancialIntelligenceSnapshots",
                table: "FinancialIntelligenceSnapshots",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FinancialIntelligenceSnapshots",
                table: "FinancialIntelligenceSnapshots");

            migrationBuilder.DropColumn(
                name: "PeriodType",
                table: "FinancialIntelligenceSnapshots");

            migrationBuilder.RenameTable(
                name: "FinancialIntelligenceSnapshots",
                newName: "FinancialIntelligenceSnapshot");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FinancialIntelligenceSnapshot",
                table: "FinancialIntelligenceSnapshot",
                column: "Id");
        }
    }
}
