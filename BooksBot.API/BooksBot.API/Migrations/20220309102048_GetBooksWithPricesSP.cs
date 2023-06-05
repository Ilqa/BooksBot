using Microsoft.EntityFrameworkCore.Migrations;

namespace BooksBot.API.Migrations
{
    public partial class GetBooksWithPricesSP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var createProcSql = @"
                                DROP PROCEDURE IF EXISTS dbo.sp_GetBooksWithPrices
                                Go
                                CREATE PROCEDURE[dbo].sp_GetBooksWithPrices   
                                @PageNumber INT,  
                                @PageSize INT,  
                                @SearchText varchar(max)    
                                AS  
								
								select  distinct(EAN) as EAN  
                                        into #TempEANs  
                                        from BookData bd
										 where bd.Title like '%' +  @SearchText  + '%'
										Or bd.EAN like '%' +  @SearchText  + '%'
                                        ORDER BY ean   
                                        OFFSET (@PageNumber-1)*@PageSize ROWS  
                                        FETCH NEXT @PageSize ROWS ONLY  

                                        select Max(bd.CreatedOn) as date, bd.EAN  
                                        into #TempBookData   
                                        from BookData bd   
										inner join #TempEANs te on bd.EAN = te.EAN
                                        inner join crawlsources cs on bd.CrawlSourceId = cs.id  
                                        group by bd.EAN, cs.SourceWebsiteId  
								
                                          
                                        select bd.* from bookdata bd    -- CAN BE MAD A VIEW  
                                        inner join #TempBookData tbd on bd.ean = tbd.ean and bd.CreatedOn = tbd.date  
                                        where tbd.EAN in (select ean from #TempEANs)  
  
                                        drop table #TempBookData  
                                        drop table #TempEANs  
										Go";


            migrationBuilder.Sql(createProcSql);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var dropProcSql = "DROP PROCEDURE IF EXISTS dbo.sp_GetBooksWithPrices";
            migrationBuilder.Sql(dropProcSql);
        }
    }
}
