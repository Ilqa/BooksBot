using System.Collections.Generic;

namespace BooksBot.API.Models
{
    public class DownloadableBookData
    {
        public List<BookWithPriceList> BookWithPriceList { get; set; }

        public List<BookDataModel> BookDataModel { get; set; }
    }
}
