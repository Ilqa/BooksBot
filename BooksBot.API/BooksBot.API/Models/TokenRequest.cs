using System.ComponentModel.DataAnnotations;

namespace BooksBot.API.Models
{
    public class TokenRequest
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }


    }
}