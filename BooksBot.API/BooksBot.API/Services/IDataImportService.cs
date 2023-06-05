using BooksBot.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;


namespace BooksBot.API.Services
{
    public interface IDataImportService
    {
        Task<ImportCrawlSourceResponseModel> ImportCrawlSourceDataFromFile(IFormFile file);
    
    }
}
