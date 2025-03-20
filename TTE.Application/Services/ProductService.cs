using System.Linq;
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

        public ProductService(IProductRepository productRepository, IRatingRepository ratingRepository)
        {
            _productRepository = productRepository;
            _ratingRepository = ratingRepository;
        }

        public async Task<ProductPaginatedResponseDto> GetProducts(
            string? category, string? orderBy, bool descending, int page, int pageSize)
        {
            var (products, totalCount) = await _productRepository.GetProducts(category, orderBy, descending, page, pageSize);

            var productDtos = new List<ProductResponseDto>();
            foreach (var product in products)
            {
                var rating = await GetProductRatings(product.Id);
                productDtos.Add(new ProductResponseDto
                {
                    Id = product.Id,
                    Title = product.Title,
                    Price = product.Price,
                    Description = product.Description,
                    Category = product.Category.Name,
                    Image = product.Image,
                    Rating = rating
                });
            }

            return new ProductPaginatedResponseDto(
                success: true,
                message: AuthenticationMessages.MESSAGE_PRODUCTS_RETRIEVED,
                data: productDtos,
                page: page,
                pageSize: pageSize,
                totalCount: totalCount
            );
        }

        private async Task<RatingDto> GetProductRatings(int productId)
        {
            var ratings = await _ratingRepository.GetRatingsByProductId(productId);

            if (!ratings.Any())
                return new RatingDto { Rate = 0, Count = 0 };

            return new RatingDto
            {
                Rate = Math.Round(ratings.Average(r => r.Rate), 1),
                Count = ratings.Count()
            };
        }
    }
}
