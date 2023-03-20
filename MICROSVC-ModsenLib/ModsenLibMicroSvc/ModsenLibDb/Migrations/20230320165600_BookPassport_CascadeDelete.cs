using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModsenLibDb.Migrations
{
    /// <inheritdoc />
    public partial class BookPassport_CascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookPassports_Books_BookId",
                table: "BookPassports");

            migrationBuilder.AddForeignKey(
                name: "FK_BookPassports_Books_BookId",
                table: "BookPassports",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookPassports_Books_BookId",
                table: "BookPassports");

            migrationBuilder.AddForeignKey(
                name: "FK_BookPassports_Books_BookId",
                table: "BookPassports",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id");
        }
    }
}
