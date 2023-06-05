﻿namespace BooksBot.API.Models
{
    public class WebsitePrice
    {
        public string ProductUrl { get; set; }
        public double Price { get; set; }

        public double Difference { get; set; }

        public string Website { get; set; }

        public string Currency { get; set; }

        public string LastCrawled { get; set; }

        public string Availability { get; set; }


    }
}