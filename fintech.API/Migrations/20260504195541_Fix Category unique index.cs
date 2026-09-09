using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace finetech.Migrations
{
    /// <inheritdoc />
    public partial class FixCategoryuniqueindex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Categories_Name_IsSystem",
                table: "Categories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name_IsSystem",
                table: "Categories",
                columns: new[] { "Name", "IsSystem" },
                unique: true);
        }
    }
}
