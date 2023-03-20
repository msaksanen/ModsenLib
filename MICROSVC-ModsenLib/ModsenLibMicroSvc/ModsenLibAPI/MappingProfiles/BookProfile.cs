using AutoMapper;
using ModsenLibAbstractions.DataTransferObjects;
using ModsenLibAPI.Models;
using ModsenLibDb.Enitities;

namespace ModsenLibAPI.MappingProfiles
{
    /// <summary>
    /// BookProfile Mapping
    /// </summary>
    public class BookProfile : Profile
    {
        /// <summary>
        /// BookProfile
        /// </summary>
        public BookProfile()
        {
            CreateMap<Book, BookDto>()
                .ForMember(d => d.IsTaken,
                 opt => opt.MapFrom(b =>(b.BookPassport!=null) ? b.BookPassport.IsTaken : false));

            CreateMap<BookDto, Book>().ReverseMap();

            CreateMap<BookPassport, BookPassportDto>();

            CreateMap<BookPassportDto, BookPassport>().ReverseMap();

            CreateMap<EditBookRequestModel, BookDto>();
        }
    }
}
