using System.Globalization;
using AutoMapper;
using TTE.Application.DTOs;
using TTE.Infrastructure.Models;

namespace TTE.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {
            CreateMap<User, UserResponseDto>().ForMember(dest => dest.Role,
                opt => opt.MapFrom(src => src.Role.Name));
            CreateMap<Category, CategoryResponseDto>();
            CreateMap<CategoryRequestDto, Category>();


            CreateMap<ProductRequestDto,Product>()
                .ForMember(dest => dest.)
        }
    }
}
