using System.ComponentModel.DataAnnotations;
using static BooksBot.API.Constants.Enums;

namespace BooksBot.API.Data.Entities
{
    public class ShouldCrawlEan
    {
        [Key]
        public int Id { get; set; }
        public string Ean { get; set; }
        public bool CrawlSourceFound { get; set;}
        public bool CrawlSourceSearched { get; set;}
        public SourceWebSiteShortNameEnum SourceWebSiteShortNameEnum { get; set; }
    }
}
