using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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


        private readonly Mock<IProductRepository> _mockProductRepository = new();
        private readonly Mock<IGenericRepository<Product>> _mockGenericProductRepo = new();
        private readonly Mock<IGenericRepository<Category>> _mockCategoryRepo = new();
        private readonly Mock<IRatingRepository> _mockRatingRepo = new();
        private readonly Mock<IGenericRepository<Inventory>> _mockInventoryRepo = new();
        private readonly Mock<IGenericRepository<Job>> _mockJobRepo = new();
        private readonly Mock<IGenericRepository<Rating>> _mockRatingGenericRepo = new();
        private readonly Mock<IMapper> _mockMapper = new();
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _service = new ProductService(
                _mockProductRepository.Object,
                _mockGenericProductRepo.Object,
                _mockCategoryRepo.Object,
                _mockRatingRepo.Object,
                _mockInventoryRepo.Object,
                _mockJobRepo.Object,
                _mockRatingGenericRepo.Object,
                _mockMapper.Object
            );
        }

        [Fact]
        public async Task CreateProducts_ShouldReturnSuccess_WhenValid()
        {
            // Arrange
            var request = new ProductRequestDto
            {
                Title = "Test Product",
                Price = 100,
                Description = "Test Description",
                Category = "electronics",
                Image = "test.jpg",
                Inventory = new InventoryDto { Total = 10, Available = 5 }
            };

            var category = new Category { Id = 1, Name = "electronics" };

            _mockCategoryRepo.Setup(repo => repo.GetByCondition(It.IsAny<System.Linq.Expressions.Expression<System.Func<Category, bool>>>()))
                             .ReturnsAsync(category);

            _mockGenericProductRepo.Setup(repo => repo.Add(It.IsAny<Product>()))
                                   .ReturnsAsync(1);

            _mockInventoryRepo.Setup(repo => repo.Add(It.IsAny<Inventory>()))
                              .ReturnsAsync(1);

            var userRole = AppConstants.ADMIN;

            // Act
            var result = await _service.CreateProducts(request, userRole) ;

            // Assert
            var data = result.Data as ProductCreatedResponseDto;
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(ValidationMessages.MESSAGE_PRODUCT_CREATED_SUCCESSFULLY, data.Message);
        }

        [Fact]
        public async Task CreateProducts_ShouldFail_WhenCategoryNotFound()
        {
            // Arrange
            var request = new ProductRequestDto
            {
                Title = "Test Product",
                Category = "nonexistent",
                Price = 100,
                Description = "Test",
                Image = "img.jpg",
                Inventory = new InventoryDto { Total = 10, Available = 5 }
            };

            _mockCategoryRepo.Setup(repo => repo.GetByCondition(It.IsAny<System.Linq.Expressions.Expression<System.Func<Category, bool>>>()))
                             .ReturnsAsync((Category)null);

            // Act
            var result = await _service.CreateProducts(request, AppConstants.ADMIN);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(ValidationMessages.CATEGORY_NOT_FOUND, result.Message);
        }

        [Fact]
        public async Task GetProductById_ShouldReturnProduct_WhenExists()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                Title = "Product A",
                Description = "Description A",
                CategoryId = 2,
                Price = 99,
                Image = "img.jpg",
                Approved = true
            };

            var category = new Category { Id = 2, Name = "electronics" };
            var ratingList = new List<Rating> { new Rating { ProductId = 1, Rate = 4 }, new Rating { ProductId = 1, Rate = 5 } };
            var inventory = new Inventory { ProductId = 1, Total = 50, Available = 25 };

            _mockGenericProductRepo.Setup(r => r.GetByCondition(p => p.Id == 1))
                                   .ReturnsAsync(product);
            _mockCategoryRepo.Setup(r => r.GetByCondition(c => c.Id == 2))
                             .ReturnsAsync(category);
            _mockRatingRepo.Setup(r => r.GetRatingsByProductId(1))
                           .ReturnsAsync(ratingList);
            _mockInventoryRepo.Setup(r => r.GetByCondition(i => i.ProductId == 1))
                              .ReturnsAsync(inventory);
            _mockMapper.Setup(m => m.Map<ProductByIdResponse>(product))
                       .Returns(new ProductByIdResponse
                       {
                           Id = product.Id,
                           Title = product.Title,
                           Description = product.Description,
                           Image = product.Image,
                           Category = category.Name,
                           Price = product.Price
                       });

            _mockMapper.Setup(m => m.Map<InventoryDto>(inventory))
                       .Returns(new InventoryDto { Total = 50, Available = 25 });

            // Act
            var result = await _service.GetProductById(1);

            // Assert
            var data = result.Data as ProductByIdResponse;
            Assert.NotNull(data);
            Assert.Equal(1, data.Id);
            Assert.Equal("electronics", data.Category);
            Assert.Equal(2, data.Rating.Count);
            Assert.Equal(4.5, data.Rating.Rate);
        }

        [Fact]
        public async Task GetProductById_ShouldFail_WhenProductNotFound()
        {
            // Arrange
            _mockGenericProductRepo.Setup(r => r.GetByCondition(p => p.Id == 99))
                                   .ReturnsAsync((Product)null);

            // Act
            var result = await _service.GetProductById(99);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(ValidationMessages.MESSAGE_PRODUCT_NOT_FOUND, result.Message);
        }
    }
}
