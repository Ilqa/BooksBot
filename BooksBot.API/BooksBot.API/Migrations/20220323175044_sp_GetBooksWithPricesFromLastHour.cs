using Microsoft.EntityFrameworkCore.Migrations;

namespace BooksBot.API.Migrations
{
    public partial class sp_GetBooksWithPricesFromLastHour : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var createProcSql = @"
                                DROP PROCEDURE IF EXISTS dbo.sp_GetBooksWithPricesFromLastHour
                                Go
                                CREATE PROCEDURE[dbo].sp_GetBooksWithPricesFromLastHour
                                     AS
                                        select Max(bd.CreatedOn) as date, bd.EAN, cs.SourceWebsiteId  
                                        into #TempBooksData   
                                        from BookData bd
                                        inner join crawlsources cs on bd.CrawlSourceId = cs.id 
										where bd.createdon > DATEADD(hh,-1,GETDATE())
										group by bd.EAN, cs.SourceWebsiteId 
										--ORDER BY bd.CreatedOn
  
                                        select bd.*  from bookdata bd   
                                        inner join #TempBooksData tbd on bd.ean = tbd.ean and bd.CreatedOn = tbd.date 
										
  
                                        drop table #TempBooksData  
										Go";

            migrationBuilder.Sql(createProcSql);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var dropProcSql = "DROP PROCEDURE IF EXISTS dbo.sp_GetBooksWithPricesFromLastHour";
            migrationBuilder.Sql(dropProcSql);
        }
    }
}
