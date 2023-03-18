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
        /// <summary>
        /// RoleProfile
        /// </summary>
        public RoleProfile()
        {
            CreateMap<Role, RoleDto>();

            CreateMap<RoleDto, Role>().ReverseMap();
        }
    }
}
