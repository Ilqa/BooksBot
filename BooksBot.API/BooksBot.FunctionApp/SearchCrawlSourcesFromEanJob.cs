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
    public class SearchCrawlSourcesFromEanJob
    {
        [FunctionName("SearchCrawlSourcesFromEanJob")]
        public static async Task Run([TimerTrigger("*/30 * * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"SearchCrawlSourcesFromEanJob Timer trigger function executed at: {DateTime.Now}");

            HttpClient client = new HttpClient();
            var response = await client.GetAsync(Environment.GetEnvironmentVariable("SearchCrawlSourcesFromEanUrl"));
            log.LogInformation($"Success Code: {response.StatusCode}");
        }
    }
}
