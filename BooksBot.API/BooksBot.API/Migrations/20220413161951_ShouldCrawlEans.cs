using Microsoft.EntityFrameworkCore.Migrations;

namespace BooksBot.API.Migrations
{
    public partial class ShouldCrawlEans : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShouldCrawlEans",
                columns: table => new
                {
                    Ean = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CrawlSourceFound = table.Column<bool>(type: "bit", nullable: false),
                    CrawlSourceSearched = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShouldCrawlEans");
        }
    }
}
