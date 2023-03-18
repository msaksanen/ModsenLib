using AutoMapper;
using ModsenLibGateWayAbstractions.DataTransferObjects;
using ModsenLibGateWayDb.Entities;

namespace ModsenLibGateWay.MappingProfiles
{
    /// <summary>
    /// UserProfile Mapping
    /// </summary>
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>();

            CreateMap<UserDto, User>().ReverseMap();
        }
    }
}
