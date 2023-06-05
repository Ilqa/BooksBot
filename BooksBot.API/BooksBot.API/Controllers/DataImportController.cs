using BooksBot.API.Models;
using BooksBot.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BooksBot.API.Controllers
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DataImportController : Controller
    {

        private readonly IDataImportService _dataImportService;

        public DataImportController(IDataImportService service)
        {
            _dataImportService = service;
        }

        /// <summary>
        /// This end point will import the Crawl Source data from csv file to the database. Please upload a csv file in acceptable format.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("ImportCrawlSourceFromFile")]
        public async Task<ImportCrawlSourceResponseModel> ImportCrawlSourceData(IFormFile file)
        {

            //var httpRequest = HttpContext.Request;
            //var file = HttpContext.Request.Form.Files.GetFile("CrawlSourceData.xls");
            return await _dataImportService.ImportCrawlSourceDataFromFile(file);



        }
    }
}
