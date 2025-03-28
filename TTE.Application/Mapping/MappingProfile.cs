using System.Globalization;
using AutoMapper;
using TTE.Application.DTOs;
using TTE.Infrastructure.Models;

namespace TTE.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserResponseDto>().ForMember(dest => dest.Role,
                opt => opt.MapFrom(src => src.Role.Name));

            CreateMap<Category, CategoryResponseDto>();
            CreateMap<CategoryRequestDto, Category>();

            CreateMap<ProductUpdateRequestDto, Product>()
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.CategoryId, opt => opt.Ignore())
                .ForMember(dest => dest.Inventory, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Product, ProductResponseDto>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.Rating, opt => opt.Ignore());

            CreateMap<InventoryRequestDto, Inventory>();
            CreateMap<Inventory, InventoryDto>();

            CreateMap<Product, ProductByIdResponse>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.Rating, opt => opt.Ignore()) 
                .ForMember(dest => dest.Inventory, opt => opt.Ignore());

        }
    }
}
