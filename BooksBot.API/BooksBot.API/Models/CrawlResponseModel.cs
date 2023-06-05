using System.Collections.Generic;

namespace BooksBot.API.Models
{
    public class CrawlResponseModel
    {
        public List<string> SuccessfulUrls { get; set; }
        public List<string> FailedUrls { get; set; }

        public List<string> InvalidUrls { get; set; }
        public List<WobBookModel> CrawledBooks { get; set; }
        public int CrawledBooksCount { get; set; }
    }
}
