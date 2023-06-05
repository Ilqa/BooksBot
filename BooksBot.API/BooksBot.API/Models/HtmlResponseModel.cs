using BooksBot.API.Data.Entities;

namespace BooksBot.API.Models
{
    public class HtmlResponseModel
    {
        public string Html { get; set; }
        public CrawlSource CrawlSource { get; set; }

    }

    public class HtmlResponseModelEAN
    {
        public string response { get; set; }
        public ShouldCrawlEan shouldCrawlEan { get; set; }

    }
}
