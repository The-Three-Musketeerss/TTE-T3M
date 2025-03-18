using TTE.Application.DTOs;
using TTE.Application.Interfaces;
using TTE.Commons.Constants;
using TTE.Infrastructure.Interfaces;

namespace TTE.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ProductPaginatedDto> GetProducts(
            string? category, string? orderBy, bool descending, int page, int pageSize)
        {
            var (products, totalCount) = await _productRepository.GetProductsAsync(category, orderBy, descending, page, pageSize);

            var productDtos = products.Select(p => new ProductDto
            {
                Id = p.Id,
                Title = p.Title,
                Price = p.Price,
                Description = p.Description,
                Image = p.Image,
                Approved = p.Approved,
                CategoryId = p.CategoryId
            }).ToList();

            return new ProductPaginatedDto(
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
