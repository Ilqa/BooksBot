namespace BooksBot.API.Models
{
    public class CrawlStatus
    {
        public int SuccessfulCrawls { get; set; }
        public int FailedCrawls { get; set; }
        public int DistinctEans { get; set; }
    }
}
