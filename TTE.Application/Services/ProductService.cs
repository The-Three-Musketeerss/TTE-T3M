using TTE.Application.DTOs;
using TTE.Application.Interfaces;
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

        public async Task<IEnumerable<ProductDto>> GetProducts()
        {
            var products = await _productRepository.GetProducts();
            return products.Select(p => new ProductDto
            {
                Id = p.Id,
                Title = p.Title,
                Price = p.Price,
                CategoryId = p.CategoryId,
                Description = p.Description,
                Image = p.Image,
                Approved = p.Approved
            });
        }
    }
}
