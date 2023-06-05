using BooksBot.API.Models;
using System;
using System.Collections.Generic;

namespace BooksBot.API.Utilities
{
    public interface IHtmlParser : IDisposable
    {
        string EanSearchURL { get; }
        ParseResponseModel IdentifyExactParser(HtmlResponseModel htmlResponse);
        string ParseProductUrl(string html);
    }
}
