using Microsoft.EntityFrameworkCore.Migrations;

namespace BooksBot.API.Migrations
{
    public partial class CrawlSourceStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "CrawlSources",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "CrawlSources");
        }
    }
}
