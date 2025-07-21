using System;
using AutoMapper;
using FirstAPI.Models;
using FirstAPI.Models.DTOs;

namespace FirstAPI.Mappers;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<DoctorAddRequestDTO, User>()
            .ForMember(dest => dest.Username, act => act.MapFrom(src => src.Email))
            .ForMember(dest => dest.Password, opt => opt.Ignore());

        CreateMap<User, DoctorAddRequestDTO>()
            .ForMember(dest => dest.Email, act => act.MapFrom(src => src.Username));

        CreateMap<PatientAddRequestDTO, User>()
            .ForMember(dest => dest.Username, act => act.MapFrom(src => src.Email))
            .ForMember(dest => dest.Password, opt => opt.Ignore());

        CreateMap<User, PatientAddRequestDTO>()
            .ForMember(dest => dest.Email, act => act.MapFrom(src => src.Username));
    }
}
