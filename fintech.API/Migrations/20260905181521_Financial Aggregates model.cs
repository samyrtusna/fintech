using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace finetech.Migrations
{
    /// <inheritdoc />
    public partial class FinancialAggregatesmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FinancialAggregates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialAggregates", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FinancialAggregates_Year",
                table: "FinancialAggregates",
                column: "Year");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FinancialAggregates");
        }
    }
}
