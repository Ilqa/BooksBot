using System;
using System.ComponentModel.DataAnnotations;
using static BooksBot.API.Constants.Enums;

namespace BooksBot.API.Data.Entities
{
    public class SourceWebsite
    {
        public Guid Id { get; set; }
        [StringLength(500)]
        [Required]
        public string Name { get; set; }
        [Required]
        public string BaseUrl { get; set; }
        [StringLength(300)]
        public string ShortName { get; set; }
        public string Currency { get; set; }
        public SourceWebSiteShortNameEnum SourceWebSiteShortNameEnum { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool IsActive { get; set; }
    }
}
