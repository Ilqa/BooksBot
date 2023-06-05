using System.Collections.Generic;

namespace BooksBot.API.Models
{
    public class BookWithPriceList
    {
        public string EAN { get; set; }
        public string Title { get; set; }

        public List<WebsitePrice> SitePrices { get; set; } = new List<WebsitePrice>();  
    }
}
