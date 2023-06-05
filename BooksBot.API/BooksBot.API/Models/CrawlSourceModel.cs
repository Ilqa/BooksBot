namespace BooksBot.API.Models
{
    public class CrawlSourceModel
    {
        public string Url { get; set; }
        public int Priority { get; set; }
        public int CrawlCounter { get; set; }
        public string Currency { get; set; }

        public int PageType { get; set; }
    }
}
