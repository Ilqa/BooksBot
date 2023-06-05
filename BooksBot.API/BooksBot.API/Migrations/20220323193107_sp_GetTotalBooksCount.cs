using Microsoft.EntityFrameworkCore.Migrations;

namespace BooksBot.API.Migrations
{
    public partial class sp_GetTotalBooksCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var createProcSql = @"
                                DROP PROCEDURE IF EXISTS dbo.sp_GetTotalBooksCount
                                Go
                                CREATE PROCEDURE[dbo].sp_GetTotalBooksCount 
                                @SearchText varchar(max),
								@Count INT OUTPUT
								as
								    If @SearchText is null Or @SearchText =''
								    set @Count = (select Count(Distinct(ean)) from bookdata)
								    else
								    set @Count = (select Count(Distinct(ean)) from bookdata 
								    where Title like '%' +  @SearchText  + '%'
								    Or EAN like '%' +  @SearchText  + '%')
								    set @Count = CONVERT(int, @Count)
								    return @Count
								go";

            migrationBuilder.Sql(createProcSql);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var dropProcSql = "DROP PROCEDURE IF EXISTS dbo.sp_GetTotalBooksCount";
            migrationBuilder.Sql(dropProcSql);
        }
    }
}
