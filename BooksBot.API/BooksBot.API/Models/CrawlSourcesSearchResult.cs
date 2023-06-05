using System.Collections.Generic;
using static BooksBot.API.Constants.Enums;

namespace BooksBot.API.Models
{
    public class CrawlSourceFound
    {
        public string CrawlSource { get; set; }
        public SourceWebSiteShortNameEnum SourceWebSiteShortNameEnum { get; set; }
    }
}
