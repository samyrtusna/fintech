using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace finetech.Migrations
{
    /// <inheritdoc />
    public partial class refactorFinancialAggregatestable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "FinancialAggregates",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "PeriodType",
                table: "FinancialAggregates",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "FinancialAggregates",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Variation",
                table: "FinancialAggregates",
                type: "numeric",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PeriodType",
                table: "FinancialAggregates");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "FinancialAggregates");

            migrationBuilder.DropColumn(
                name: "Variation",
                table: "FinancialAggregates");

            migrationBuilder.AlterColumn<int>(
                name: "Name",
                table: "FinancialAggregates",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
