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

            CreateMap<CouponRequestDto, Coupon>();
            CreateMap<Coupon, CouponResponseDto>();

            CreateMap<Cart_Item, CartItemResponseDto>();
            CreateMap<Coupon, CouponAppliedDto>()
                .ForMember(dest => dest.CouponCode, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.DiscountPercentage, opt => opt.MapFrom(src => src.Discount));

            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.TotalBeforeDiscount, opt => opt.MapFrom(src => src.Total_before_discount))
                .ForMember(dest => dest.TotalAfterDiscount, opt => opt.MapFrom(src => src.Total_after_discount))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.CustomerName))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.PaymentStatus));

            CreateMap<Order_Item, OrderItemDto>();


        }
    }
}
