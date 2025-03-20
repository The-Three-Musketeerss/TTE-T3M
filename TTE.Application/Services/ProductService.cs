using TTE.Application.DTOs;
using TTE.Application.Interfaces;
using TTE.Commons.Constants;
using TTE.Infrastructure.Repositories;

namespace TTE.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ProductPaginatedResponseDto> GetProducts(
            string? category, string? orderBy, bool descending, int page, int pageSize)
        {
            var (products, totalCount) = await _productRepository.GetProductsAsync(category, orderBy, descending, page, pageSize);

            var productDtos = products.Select(p => new ProductResponseDto
            {
                Id = p.Id,
                Title = p.Title,
                Price = p.Price,
                Description = p.Description,
                Image = p.Image,
                Approved = p.Approved,
                CategoryId = p.CategoryId
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

        Task<GenericResponseDto<ProductCreatedResponseDto>> CreateProducts(ProductRequestDto request, string userRole)
        {

            throw new NotImplementedException();
        }
    }
}
