using Microsoft.AspNetCore.Mvc;
using Moq;
using TTE.API.Controllers;
using TTE.Application.DTOs;
using TTE.Application.Interfaces;

namespace TTE.Tests.Controllers
{
    public class ProductControllerTests
    {
        private readonly Mock<IProductService> _mockProductService;
        private readonly ProductController _controller;

        public ProductControllerTests()
        {
            _mockProductService = new Mock<IProductService>();
            _controller = new ProductController(_mockProductService.Object);
        }

        [Fact]
        public async Task GetProducts_ShouldReturnFirst3CheapestProducts_WhenOrderedByPriceAscending()
        {
            var page = 1;
            var pageSize = 3;
            var orderBy = "price";
            var descending = false;

            var allProducts = new List<ProductResponseDto>
    {
            new ProductResponseDto { Id = 5, Title = "Product 5", Price = 50 },
            new ProductResponseDto { Id = 3, Title = "Product 3", Price = 30 },
            new ProductResponseDto { Id = 1, Title = "Product 1", Price = 10 },
            new ProductResponseDto { Id = 4, Title = "Product 4", Price = 40 },
            new ProductResponseDto { Id = 2, Title = "Product 2", Price = 20 }
    };

            var ordered = allProducts.OrderBy(p => p.Price).Take(3).ToList();

            var paginatedResponse = new ProductPaginatedResponseDto(
                success: true,
                message: "Products retrieved successfully",
                data: ordered,
                page: page,
                pageSize: pageSize,
                totalCount: allProducts.Count
            );

            _mockProductService.Setup(service =>
                service.GetProducts(null, orderBy, descending, page, pageSize))
                .ReturnsAsync(paginatedResponse);

            // Act
            var result = await _controller.GetProducts(null, orderBy, descending, page, pageSize);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ProductPaginatedResponseDto>(okResult.Value);

            Assert.True(response.Success);

            var products = Assert.IsAssignableFrom<IEnumerable<ProductResponseDto>>(response.Data).ToList();

            Assert.Equal(3, products.Count);
            Assert.Equal(10, products[0].Price);
            Assert.Equal(20, products[1].Price);
            Assert.Equal(30, products[2].Price);

            Assert.Equal("Product 1", products[0].Title);
            Assert.Equal("Product 2", products[1].Title);
            Assert.Equal("Product 3", products[2].Title);

            Assert.Equal(page, response.Page);
            Assert.Equal(pageSize, response.PageSize);
            Assert.Equal(5, response.TotalCount);
            Assert.Equal(2, response.TotalPages);
        }

    }
}
