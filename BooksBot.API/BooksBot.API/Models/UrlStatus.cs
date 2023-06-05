using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BooksBot.API.Models
{
    [NotMapped]
    public class UrlStatus
    {

        public string Url { get; set; }
        public string Status { get; set; }  

        public DateTime CrawlStartTime { get; set; }
    }
}
