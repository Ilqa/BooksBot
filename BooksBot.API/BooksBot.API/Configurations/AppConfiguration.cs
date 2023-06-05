namespace BooksBot.API.Configurations
{
    public class AppConfiguration
    {
        public int UrlCount { get; set; }
        public string ScrappingBeeClientUrl { get; set; }
        public string ExtraQueryParams { get; set; }
        public int DelayTimeInSeconds { get; set; }

        public int ResetRunningCrawlProcessInHours { get; set; }
        public int EanURLCount { get; set; }
    }
}
