using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using static BooksBot.API.Constants.Enums;

namespace BooksBot.API.Data.Entities
{
    public class CrawlSource : IAuditableEntity
    {
        public Guid Id { get; set; }
        
        [Required]
        public string Url { get; set; }

        [NotMapped]
        public string UsedUrl { get; set; }
        public int Priority { get; set; }
        public string Currency { get; set; }
        public int CrawlCounter { get; set; }
        public DateTime? LastCrawled { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }

        [Required]
        public Guid SourceWebsiteId { get; set; }

        [JsonIgnore]
        public virtual SourceWebsite SourceWebsite { get; set; }
        public virtual ICollection<BookData> BookData { get; set; }

        public string Status { get; set; }
        public PageTypeEnum PageType { get; set; }

        public DateTime? CrawlStartTime { get; set; }

        public bool ShouldCrawl { get; set; }
    }
}
