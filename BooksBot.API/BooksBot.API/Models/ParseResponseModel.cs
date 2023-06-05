using BooksBot.API.Data.Entities;
using System.Collections.Generic;

namespace BooksBot.API.Models
{
    public class ParseResponseModel
    {
        public bool IsUsed { get; set; }
        public string UsedURL { get; set; }
        public List<WobBookModel> bookData { get; set; } = new();
    }
}
