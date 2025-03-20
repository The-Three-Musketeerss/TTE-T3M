using System.Linq;
using AutoMapper;
using TTE.Application.DTOs;
using TTE.Application.Interfaces;
using TTE.Commons.Constants;
using TTE.Infrastructure.Models;
using TTE.Infrastructure.Repositories;

namespace TTE.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IRatingRepository _ratingRepository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, IRatingRepository ratingRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _ratingRepository = ratingRepository;
            _mapper = mapper;
        }

        public async Task<ProductPaginatedResponseDto> GetProducts(
            string? category, string? orderBy, bool descending, int page, int pageSize)
        {
            var (products, totalCount) = await _productRepository.GetProducts(category, orderBy, descending, page, pageSize);

            var productIds = products.Select(p => p.Id).ToList();

            var ratings = await _ratingRepository.GetRatingsByProductIds(productIds);


            var productDtos = products.Select(product =>
            {
                var productRatings = ratings.Where(r => r.ProductId == product.Id).ToList();

                var ratingDto = new RatingDto
                {
                    Rate = productRatings.Any() ? Math.Round(productRatings.Average(r => r.Rate), 1) : 0,
                    Count = productRatings.Count
                };

                var dto = _mapper.Map<ProductResponseDto>(product);
                dto.Rating = ratingDto;

                return dto;
            }).ToList();

            return new ProductPaginatedResponseDto(
                success: true,
                message: AuthenticationMessages.MESSAGE_PRODUCTS_RETRIEVED,
                data: productDtos,
                page: page,
                pageSize: pageSize,
                totalCount: totalCount
            );
        }
    }
}
