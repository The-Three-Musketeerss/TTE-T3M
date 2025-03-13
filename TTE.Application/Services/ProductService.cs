using TTE.Application.DTOs;
using TTE.Application.Interfaces;
using TTE.Infrastructure.Models;
using TTE.Infrastructure.Repositories;

namespace TTE.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IGenericRepository<Product> _productRepository;

        public ProductService(IGenericRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<ProductDto>> GetProducts()
        {
            var products = await _productRepository.Get();
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
