using AutoMapper;
using BooksBot.API.Data.Entities;
using BooksBot.API.Models;

namespace BooksBot.API.Mappings
{
    internal class BookDataProfile : Profile
    {
        public BookDataProfile()
        {
            CreateMap<BookData, WebsitePrice>()
                 .ForMember(dist => dist.Website, opt => opt.MapFrom(src => src.CrawlSource.SourceWebsite.Name))
                 .ForMember(dist => dist.Currency, opt => opt.MapFrom(src => src.CrawlSource.Currency))
                 .ReverseMap();
            CreateMap<BookData, WobBookModel>().ReverseMap();
            CreateMap<BookData, BookDataModel>().ForMember(dist => dist.Website, opt => opt.MapFrom(src => src.CrawlSource.SourceWebsite.Name))
                 .ForMember(dist => dist.Currency, opt => opt.MapFrom(src => src.CrawlSource.Currency)).ReverseMap();
        }
    }
}
