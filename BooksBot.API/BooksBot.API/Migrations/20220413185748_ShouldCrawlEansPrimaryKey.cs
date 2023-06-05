using Microsoft.EntityFrameworkCore.Migrations;

namespace BooksBot.API.Migrations
{
    public partial class ShouldCrawlEansPrimaryKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Ean",
                table: "ShouldCrawlEans",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShouldCrawlEans",
                table: "ShouldCrawlEans",
                column: "Ean");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ShouldCrawlEans",
                table: "ShouldCrawlEans");

            migrationBuilder.AlterColumn<string>(
                name: "Ean",
                table: "ShouldCrawlEans",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
