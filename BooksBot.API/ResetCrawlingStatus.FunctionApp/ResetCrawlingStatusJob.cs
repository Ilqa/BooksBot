using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace ResetCrawlingStatus.FunctionApp
{
    public class ResetCrawlingStatusJob
    {
        [FunctionName("ResetCrawlingStatusJob")]
        public async Task RunAsync([TimerTrigger("0 0 */2 * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"ResetCrawlingStatusJob Timer trigger function executed at: {DateTime.Now}");

            HttpClient client = new HttpClient();
            var response = await client.GetAsync(Environment.GetEnvironmentVariable("BooksBotEngineUrl"));
            //if (response.IsSuccessStatusCode)
            //{
            //    var jsonString = await response.Content.ReadAsStringAsync();
            //    var responseModel = JsonConvert.DeserializeObject<CrawlResponseModel>(jsonString);
            //    log.LogInformation($"Total Crawled Books: {responseModel.CrawledBooksCount}");
            //    log.LogInformation($"Total successfull crawl hits: {responseModel.SuccessfulUrls.Count}");
            //    log.LogInformation($"Total failed crawl hits: {responseModel.FailedUrls.Count}");
            //}
            log.LogInformation($"Success Code: {response.StatusCode}");
        }
    }
}
