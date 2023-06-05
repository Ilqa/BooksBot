using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BooksBot.API.Data.Entities
{
    public class BookData : IAuditableEntity
    {
        public Guid Id { get; set; }
        [StringLength(200)]
        [Required]
        public string EAN { get; set; }
        [Required]
        public string Title { get; set; }
        public bool InStock { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        [Required]
        public string ProductUrl { get; set; }
        public double Difference { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int Status { get; set; }

        [Required]
        public Guid CrawlSourceId { get; set; }

        [JsonIgnore]
        public virtual CrawlSource CrawlSource { get; set; }

        public string Availability => InStock ? $"{Quantity} available" : "Not available";

        [NotMapped]
        public string LastCrawled { get; set; }
    }

}
