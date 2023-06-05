using Microsoft.EntityFrameworkCore.Migrations;

namespace BooksBot.API.Migrations
{
    public partial class ArchiveBookData_Sp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var createProcSql = @"
                                DROP PROCEDURE IF EXISTS dbo.sp_ArchiveBookData
                                Go
                                CREATE PROCEDURE[dbo].sp_ArchiveBookData 
								As
									DECLARE @TableName varchar(200);
									SET @TableName='ArchivedData' +(CONVERT(VARCHAR,GETDATE(),20))
									select id into #listOfCrawlSourceIDs from crawlsources where crawlcounter >=10
									select id into #listOfbookDataIDs from BookData where CrawlSourceId in (select id from #listOfCrawlSourceIDs)
									
									select * into ArchivedData 
									from BookData where id in (select id from #listOfbookDataIDs) 

									delete from bookdata where id in (select id from ArchivedData)
									update CrawlSources set CrawlCounter = 0, LastCrawled = null, CrawlStartTime = null where id in (select id from #listOfCrawlSourceIDs)

									EXEC sp_rename 'ArchivedData', @TableName

									drop table #listOfCrawlSourceIDs;
									drop table #listOfbookDataIDs
                               Go";

            migrationBuilder.Sql(createProcSql);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var dropProcSql = "DROP PROCEDURE IF EXISTS dbo.sp_ArchiveBookData";
            migrationBuilder.Sql(dropProcSql);
        }
    }
}
