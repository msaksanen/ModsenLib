using AutoMapper;
using ModsenLibAbstractions.DataTransferObjects;
using ModsenLibAPI.Models;
using ModsenLibDb.Enitities;

namespace ModsenLibAPI.MappingProfiles
{
    /// <summary>
    /// NewBookProfile Mapping
    /// </summary>
    public class NewBookProfile : Profile
    {
        /// <summary>
        /// BookProfile
        /// </summary>
        public NewBookProfile()
        {
            CreateMap<AddBookRequestModel, BookDto>()
                 .ForMember(dto => dto.CreationDate,
                      opt => opt.MapFrom(model => DateTime.Now))
                 .ForMember(dto => dto.IsTaken,
                      opt => opt.MapFrom(model => false))
                 .ForMember(dto => dto.Id,
                      opt => opt.MapFrom(model => Guid.NewGuid()));

        }
    }
}
