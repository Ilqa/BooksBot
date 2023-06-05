using System;

namespace BooksBot.API.Models
{
    public class TokenResponse
    {
        public string Token { get; set; }
        public string Message { get; set; }
        public bool IsLoginSuccessful { get; set; }
    }
}