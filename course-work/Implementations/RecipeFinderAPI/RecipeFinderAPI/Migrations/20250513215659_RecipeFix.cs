using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecipeFinderAPI.Migrations
{
    /// <inheritdoc />
    public partial class RecipeFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_Users_UserId",
                table: "Recipes");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Recipes",
                newName: "CreatedBy");

            migrationBuilder.RenameIndex(
                name: "IX_Recipes_UserId",
                table: "Recipes",
                newName: "IX_Recipes_CreatedBy");

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

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Recipes",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Recipes_CreatedBy",
                table: "Recipes",
                newName: "IX_Recipes_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_Users_UserId",
                table: "Recipes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
