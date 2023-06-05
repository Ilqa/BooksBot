using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksBot.FunctionApp.Models
{
    internal class CrawlResponseModel
    {
        public List<string> SuccessfulUrls { get; set; }
        public List<string> FailedUrls { get; set; }
        public int CrawledBooksCount { get; set; }
    }
}
