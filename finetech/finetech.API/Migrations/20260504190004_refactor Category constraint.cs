using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace finetech.Migrations
{
    /// <inheritdoc />
    public partial class refactorCategoryconstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Categories_IsSystem_Name",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_UserId_Name",
                table: "Categories");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name_IsSystem",
                table: "Categories",
                columns: new[] { "Name", "IsSystem" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Categories_Name_IsSystem",
                table: "Categories");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_IsSystem_Name",
                table: "Categories",
                columns: new[] { "IsSystem", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_UserId_Name",
                table: "Categories",
                columns: new[] { "UserId", "Name" },
                unique: true);
        }
    }
}
