using System.Collections.Generic;

namespace BooksBot.API.Constants
{
    public static class AppContants
    {
        //contains strings that indicate something like "Page not found" etc
        private static readonly List<string> arrNotFoundMessages = new List<string>()
        {
            "This page could not be found",
            "404 (NOT FOUND)",
            "We're sorry! We couldn't find any results"
        };

        public static List<string> NotFoundMessagesArray { get; private set; } = arrNotFoundMessages;
    }
}
