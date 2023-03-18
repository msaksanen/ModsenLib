﻿using AutoMapper;
using ModsenLibGateWay.Models;
using ModsenLibGateWayAbstractions.DataTransferObjects;

namespace ModsenLibGateWay.MappingProfiles
{
    /// <summary>
    /// CustomerRegRequestModel Mapping to UserDto
    /// </summary>
    public class CustomerRegRequestModelProfile : Profile
    {
        /// <summary>
        /// CustomerRegRequestModel Profile Mapping to UserDto
        /// </summary>
        public CustomerRegRequestModelProfile()
        {
            CreateMap<CustomerRegRequestModel, UserDto>()
                .ForMember(dto => dto.IsFullBlocked,
                     opt => opt.MapFrom(s => false))
                .ForMember(dto => dto.RegistrationDate,
                     opt => opt.MapFrom(model => DateTime.Now))
                .ForMember(dto => dto.Id,
                     opt => opt.MapFrom(model => Guid.NewGuid()))
                .ForMember(dto => dto.PasswordHash,
                     opt => opt.MapFrom(model => model.Password));

        }

    }
}
