using System.Globalization;
using AutoMapper;
using TTE.Infrastructure.DTOs;
using TTE.Infrastructure.Models;

namespace TTE.Infrastructure.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {
            CreateMap<User, UserResponseDto>().ForMember(dest => dest.Role,
                opt => opt.MapFrom(src => src.Role.Name));
        }
    }
}
