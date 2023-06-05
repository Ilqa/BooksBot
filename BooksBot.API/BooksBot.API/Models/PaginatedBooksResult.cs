using System;
using System.Collections.Generic;

namespace BooksBot.API.Models
{
    public class PaginatedBooksResult
    {  
        public int CurrentPage { get; set; }

        public int TotalPages => TotalCount > PageSize? TotalCount / PageSize : 0 ;

        public int TotalCount { get; set; }
        public int PageSize { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;

        public bool HasNextPage => CurrentPage < TotalPages;

        public List<BookWithPriceList> BooksWithPrices { get; set; } = new();
    }

}
