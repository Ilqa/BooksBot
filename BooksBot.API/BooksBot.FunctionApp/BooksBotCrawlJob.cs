using System;
using System.Net.Http;
using System.Threading.Tasks;
using BooksBot.FunctionApp.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BooksBot.FunctionApp
{
    public class BooksBotCrawlJob
    {
        [FunctionName("BooksBotCrawlJob")]
        public static async Task Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"BooksBotCrawlJob Timer trigger function executed at: {DateTime.Now}");

            HttpClient client = new HttpClient();
            var response = await client.GetAsync(Environment.GetEnvironmentVariable("BooksBotEngineUrl"));
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var responseModel = JsonConvert.DeserializeObject<CrawlResponseModel>(jsonString);
                log.LogInformation($"Total Crawled Books: {responseModel.CrawledBooksCount}");
                log.LogInformation($"Total successfull crawl hits: {responseModel.SuccessfulUrls.Count}");
                log.LogInformation($"Total failed crawl hits: {responseModel.FailedUrls.Count}");
            }
            log.LogInformation($"Success Code: {response.StatusCode}");
        }
    }
}
