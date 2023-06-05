using AutoMapper;
using BooksBot.API.Constants;
using BooksBot.API.Data.Entities;
using BooksBot.API.Data.Repositories;
using BooksBot.API.Models;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BooksBot.API.Services
{
    public class DataImportService : IDataImportService
    {
        private readonly IMapper _mapper;

        private readonly ICrawlSourceRepository _crawlSourceRepository;

        private readonly IUnitOfWork _unitOfWork;

        private readonly ILogger<ScraperService> _logger;

        public DataImportService(IMapper mapper,
                                 ICrawlSourceRepository crawlSourceRepository,
                                IUnitOfWork unitOfWork,
                                ILogger<ScraperService> logger)
        {
            _mapper = mapper;
            _crawlSourceRepository = crawlSourceRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }



        public async Task<ImportCrawlSourceResponseModel> ImportCrawlSourceDataFromFile(IFormFile file)
        {
            var response = new ImportCrawlSourceResponseModel();
            try
            {
                var crawlSources = new List<CrawlSourceModel>();

                //for now we are only supporting csv files as required
                if (!file.FileName.EndsWith(".csv"))
                {
                    response.Message = MessageStrings.FileNotSupported;
                    return response;
                }

                if (file.FileName.EndsWith(".csv"))
                #region CSV File Read
                {
                    using var streamCsv = file.OpenReadStream();
                    using StreamReader sr = new StreamReader(streamCsv);
                    _ = sr.ReadLine().Split(',');  // Ignore headers

                    while (!sr.EndOfStream)
                    {
                        string[] row = sr.ReadLine().Split(',');
                        var url = row[3].Trim();
                        //skip if the url already exist in the list
                        if (!crawlSources.Any(t => t.Url.Equals(url, StringComparison.InvariantCultureIgnoreCase)))
                         crawlSources.Add(new CrawlSourceModel { Url = url, Currency = row[4], Priority = Convert.ToInt32(row[5]) }); // add pagetype here too
                    }
                }

                #endregion

                else if (file.FileName.EndsWith(".xlsx"))
                #region Excel File Read
                {
                    using var stream = new MemoryStream();
                    file.CopyTo(stream);
                    stream.Position = 0;
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        while (reader.Read()) //Each row of the file
                        {
                            var url = reader.GetValue(0).ToString().Trim();
                            //skip if the url already exist in the list
                            if (!reader.GetValue(3).ToString().Equals("URL") && !crawlSources.Any(t => t.Url.Equals(url, StringComparison.InvariantCultureIgnoreCase)))
                                crawlSources.Add(new CrawlSourceModel { Url = url, Priority = Convert.ToInt32(reader.GetValue(1).ToString()) });
                        }
                    }

                }
                #endregion


                var totalUploadedRecords = await ExportCrawlSourceDataToDB(crawlSources);
                response.Success = true;
                response.Count = totalUploadedRecords;
                response.Message = MessageStrings.UploadSuccess;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return response;
        }

        private async Task<int> ExportCrawlSourceDataToDB(List<CrawlSourceModel> crawlSources)
        {
            var newCrawlSourceUrls = crawlSources.Select(t => t.Url).ToList();
            var wobSiteId = Guid.Parse(StringConstants.WobGBGuid);

            var alreadyPresentCrawlSources = _crawlSourceRepository.CrawlSources
                                               .Where(t => t.SourceWebsiteId == wobSiteId && newCrawlSourceUrls.Any(newUrl => newUrl.Equals(t.Url)))
                                               .Select(t => t.Url.Trim())
                                               .ToList();
            //filtering the urls that doesn't already exist in database
            var crawlSourceList = crawlSources.Select(c => _mapper.Map<CrawlSource>(c))
                                              .Where(t => !alreadyPresentCrawlSources.Any(existingUrl => existingUrl.Equals(t.Url, StringComparison.InvariantCultureIgnoreCase)))
                                              .ToList();

            if (crawlSourceList.Any())
            {
                crawlSourceList.ForEach(crawlSource => { crawlSource.Id = Guid.NewGuid(); crawlSource.SourceWebsiteId = wobSiteId; }); // set website id conditionally
                await _crawlSourceRepository.AddRangeAsync(crawlSourceList);
                await _unitOfWork.Commit();
                return crawlSourceList.Count();
            }
            return 0;
        }
    }
}
