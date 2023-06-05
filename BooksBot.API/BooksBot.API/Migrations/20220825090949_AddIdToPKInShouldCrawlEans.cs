using Microsoft.EntityFrameworkCore.Migrations;

namespace BooksBot.API.Migrations
{
    public partial class AddIdToPKInShouldCrawlEans : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ShouldCrawlEans",
                table: "ShouldCrawlEans");

            migrationBuilder.AlterColumn<string>(
                name: "Ean",
                table: "ShouldCrawlEans",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ShouldCrawlEans",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShouldCrawlEans",
                table: "ShouldCrawlEans",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ShouldCrawlEans",
                table: "ShouldCrawlEans");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ShouldCrawlEans");

            migrationBuilder.AlterColumn<string>(
                name: "Ean",
                table: "ShouldCrawlEans",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShouldCrawlEans",
                table: "ShouldCrawlEans",
                column: "Ean");
        }
    }
}
