using Microsoft.EntityFrameworkCore.Migrations;

namespace BooksBot.API.Migrations
{
    public partial class sp_GetBookPricesFromEan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var createProcSql = @"
                                DROP PROCEDURE IF EXISTS dbo.sp_GetBookPricesFromEan
                                Go
                                CREATE PROCEDURE[dbo].sp_GetBookPricesFromEan     
                                @EanList varchar(max) = NULL     
                                AS 
                                SELECT value as Ean into #TempEANs FROM STRING_SPLIT(@EanList, ',')   
                                          
  
                                        select Max(bd.CreatedOn) as date, bd.EAN    
                                        into #TempBookData     
                                        from BookData bd     
										inner join #TempEANs te on bd.EAN = te.EAN  
                                        inner join crawlsources cs on bd.CrawlSourceId = cs.id    
                                        group by bd.EAN, cs.SourceWebsiteId    
          
                                            
                                        select bd.* from bookdata bd   
                                        inner join #TempBookData tbd on bd.ean = tbd.ean and bd.CreatedOn = tbd.date    
                                        where tbd.EAN in (select ean from #TempEANs)    
    
                                        drop table #TempBookData    
                                        drop table #TempEANs  
								go";

            migrationBuilder.Sql(createProcSql);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var dropProcSql = "DROP PROCEDURE IF EXISTS dbo.sp_GetBookPricesFromEan";
            migrationBuilder.Sql(dropProcSql);

        }
    }
}
