using AutoMapper;
using BooksBot.API.Data.Entities;
using BooksBot.API.Models;

namespace BooksBot.API.Mappings
{
    internal class CrawlSourceProfile : Profile
    {
        public CrawlSourceProfile()
        {
            CreateMap<CrawlSourceModel, CrawlSource>().ReverseMap();
            CreateMap<CrawlSource, UrlStatus>().ReverseMap();
        }
    }
}
