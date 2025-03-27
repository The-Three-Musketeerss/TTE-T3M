using AutoMapper;
using Moq;
using TTE.Application.DTOs;
using TTE.Application.Services;
using TTE.Commons.Constants;
using TTE.Infrastructure.Models;
using TTE.Infrastructure.Repositories;

namespace TTE.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<IGenericRepository<Product>> _mockGenericProductRepository;
        private readonly Mock<IGenericRepository<Category>> _mockGenericCategoryRepository;
        private readonly Mock<IRatingRepository> _mockRatingRepository;
        private readonly Mock<IGenericRepository<Inventory>> _mockInventoryRepository;
        private readonly Mock<IGenericRepository<Job>> _mockJobRepository;
        private readonly Mock<IGenericRepository<Rating>> _mockGenericRatingRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _mockGenericProductRepository = new Mock<IGenericRepository<Product>>();
            _mockGenericCategoryRepository = new Mock<IGenericRepository<Category>>();
            _mockRatingRepository = new Mock<IRatingRepository>();
            _mockInventoryRepository = new Mock<IGenericRepository<Inventory>>();
            _mockJobRepository = new Mock<IGenericRepository<Job>>();
            _mockGenericRatingRepository = new Mock<IGenericRepository<Rating>>();
            _mockMapper = new Mock<IMapper>();

            _productService = new ProductService(
                _mockProductRepository.Object,
                _mockGenericProductRepository.Object,
                _mockGenericCategoryRepository.Object,
                _mockRatingRepository.Object,
                _mockInventoryRepository.Object,
                _mockJobRepository.Object,
                _mockGenericRatingRepository.Object,
                _mockMapper.Object
            );
        }

        [Fact]
        public async Task GetProducts_ShouldReturnPaginatedResponse_WithCorrectRatings()
        {
            var products = new List<Product>
            {
                new Product { Id = 1, Title = "Product 1", Price = 10 },
                new Product { Id = 2, Title = "Product 2", Price = 20 }
            };

            var ratings = new List<Rating>
            {
                new Rating { ProductId = 1, Rate = 4 },
                new Rating { ProductId = 1, Rate = 5 },
                new Rating { ProductId = 2, Rate = 3 }
            };

            var mappedDtos = new List<ProductResponseDto>
            {
                new ProductResponseDto { Id = 1, Title = "Product 1", Price = 10 },
                new ProductResponseDto { Id = 2, Title = "Product 2", Price = 20 }
            };

            _mockProductRepository
                .Setup(r => r.GetProducts(null, null, false, 1, 10))
                .ReturnsAsync((products, products.Count));

            _mockRatingRepository
                .Setup(r => r.GetRatingsByProductIds(It.IsAny<List<int>>()))
                .ReturnsAsync(ratings);

            _mockMapper
                .Setup(m => m.Map<ProductResponseDto>(products[0]))
                .Returns(mappedDtos[0]);

            _mockMapper
                .Setup(m => m.Map<ProductResponseDto>(products[1]))
                .Returns(mappedDtos[1]);

            // Act
            var result = await _productService.GetProducts(null, null, false, 1, 10);

            // Assert
            Assert.True(result.Success);

            var data = Assert.IsAssignableFrom<IEnumerable<ProductResponseDto>>(result.Data).ToList();

            Assert.Equal(2, data.Count);

            var dto1 = data.First(p => p.Id == 1);
            var dto2 = data.First(p => p.Id == 2);

            Assert.Equal(4.5, dto1.Rating.Rate);
            Assert.Equal(2, dto1.Rating.Count);

            Assert.Equal(3, dto2.Rating.Rate);
            Assert.Equal(1, dto2.Rating.Count);

            Assert.Equal(1, result.Page);
            Assert.Equal(10, result.PageSize);
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(1, result.TotalPages);

        }
    }
}
