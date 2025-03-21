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
        private readonly IGenericRepository<Product> _genericProductRepository;
        private readonly IGenericRepository<Category> _genericCategoryRepository;
        private readonly IRatingRepository _ratingRepository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, IGenericRepository<Product> genericProductRepository, IGenericRepository<Category> genericCategoryRepository, IRatingRepository ratingRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _ratingRepository = ratingRepository;
            _genericProductRepository = genericProductRepository;
            _genericCategoryRepository = genericCategoryRepository;
            _mapper = mapper;
        }

        public async Task<GenericResponseDto<string>> UpdateProduct(int productId, ProductRequestDto request)
        {
            var includes = new string[1] { "Inventory" };
            var products = await _genericProductRepository.GetEntityWithIncludes(includes);
            var product = products.FirstOrDefault(p => p.Id == productId);
            if (product == null)
            {
                return new GenericResponseDto<string>(false, ValidationMessages.MESSAGE_PRODUCT_NOT_FOUND);
            }

            if (request.Inventory != null && request.Inventory.Available > request.Inventory.Total)
            {
                return new GenericResponseDto<string>(
                    false, "Available inventory cannot be greater than total inventory.");
            }

            if (request.Category != null)
            {
                var category = await _genericCategoryRepository.GetByCondition(x => x.Name == request.Category);
                if (category == null)
                {
                    return new GenericResponseDto<string>(false, ValidationMessages.CATEGORY_NOT_FOUND);
                }
                product.CategoryId = category.Id;
            }

            if (request.Price == null)
            {
                request.Price = product.Price;
            }

            _mapper.Map(request, product);

            if (product.Inventory == null)
            {
                product.Inventory = _mapper.Map<Inventory>(request.Inventory);
                product.Inventory.ProductId = product.Id;
            }
            else
            {
                _mapper.Map(request.Inventory, product.Inventory);
            }

            await _genericProductRepository.Update(product);
            return new GenericResponseDto<string>(true, ValidationMessages.MESSAGE_PRODUCT_UPDATED_SUCCESSFULLY);
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
