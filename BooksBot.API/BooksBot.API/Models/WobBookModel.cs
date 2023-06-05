using System;

namespace BooksBot.API.Models
{
    public class WobBookModel
    {
        public string EAN { get; set; }
        public string Title { get; set; }
        public string ProductUrl { get; set; }
        public double Price { get; set; }
        public Guid CrawlSourceId { get; set; }

        public int? Quantity { get; set; }
    }
}
