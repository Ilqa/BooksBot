using Microsoft.EntityFrameworkCore.Migrations;

namespace BooksBot.API.Migrations
{
    public partial class AddColumnSourceWebSiteShortNameEnumInShouldCrawlEans : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SourceWebSiteShortNameEnum",
                table: "ShouldCrawlEans",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SourceWebSiteShortNameEnum",
                table: "ShouldCrawlEans");
        }
    }
}
