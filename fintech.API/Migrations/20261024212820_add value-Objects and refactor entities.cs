using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace finetech.Migrations
{
    /// <inheritdoc />
    public partial class addvalueObjectsandrefactorentities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FinancialAggregates_Year",
                table: "FinancialAggregates");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "FinancialAggregates");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "FinancialAggregates");

            migrationBuilder.RenameColumn(
                name: "Currency",
                table: "FinancialTransactions",
                newName: "CurrencyCode");

            migrationBuilder.RenameColumn(
                name: "BaseCurrency",
                table: "FinancialTransactions",
                newName: "BaseCurrencyCode");

            migrationBuilder.RenameColumn(
                name: "Currency",
                table: "FinancialAggregates",
                newName: "CurrencySymbol");

            migrationBuilder.AlterColumn<string>(
                name: "CurrencySymbol",
                table: "UserCurrencies",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3);

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "FinancialAggregates",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "CurrencyCode",
                table: "FinancialAggregates",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateOnly>(
                name: "PeriodEndDate",
                table: "FinancialAggregates",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<DateOnly>(
                name: "PeriodStartDate",
                table: "FinancialAggregates",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "FinancialAggregates");

            migrationBuilder.DropColumn(
                name: "CurrencyCode",
                table: "FinancialAggregates");

            migrationBuilder.DropColumn(
                name: "PeriodEndDate",
                table: "FinancialAggregates");

            migrationBuilder.DropColumn(
                name: "PeriodStartDate",
                table: "FinancialAggregates");

            migrationBuilder.RenameColumn(
                name: "CurrencyCode",
                table: "FinancialTransactions",
                newName: "Currency");

            migrationBuilder.RenameColumn(
                name: "BaseCurrencyCode",
                table: "FinancialTransactions",
                newName: "BaseCurrency");

            migrationBuilder.RenameColumn(
                name: "CurrencySymbol",
                table: "FinancialAggregates",
                newName: "Currency");

            migrationBuilder.AlterColumn<string>(
                name: "CurrencySymbol",
                table: "UserCurrencies",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AddColumn<decimal>(
                name: "Value",
                table: "FinancialAggregates",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "FinancialAggregates",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_FinancialAggregates_Year",
                table: "FinancialAggregates",
                column: "Year");
        }
    }
}
