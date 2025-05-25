using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecipeFinderAPI.Migrations
{
    /// <inheritdoc />
    public partial class RecipesSetNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_Users_CreatedBy",
                table: "Recipes");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_Users_CreatedBy",
                table: "Recipes",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_Users_CreatedBy",
                table: "Recipes");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_Users_CreatedBy",
                table: "Recipes",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
