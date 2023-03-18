using AutoMapper;
using ModsenLibGateWayAbstractions.DataTransferObjects;
using ModsenLibGateWayDb.Entities;

namespace ModsenLibGateWay.MappingProfiles
{
    /// <summary>
    /// Roleprofile Mapping
    /// </summary>
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<Role, RoleDto>();

            CreateMap<RoleDto, Role>().ReverseMap();
        }
    }
}
