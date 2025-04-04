using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using TTE.Application.DTOs;
using TTE.Application.Interfaces;
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
        private readonly Mock<IGenericRepository<Inventory>> _mockGenericInventoryRepository;
        private readonly Mock<IGenericRepository<Job>> _mockGenericJobRepository;
        private readonly Mock<IGenericRepository<Rating>> _mockGenericRatingRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _mockGenericProductRepository = new Mock<IGenericRepository<Product>>();
            _mockGenericCategoryRepository = new Mock<IGenericRepository<Category>>();
            _mockRatingRepository = new Mock<IRatingRepository>();
            _mockGenericInventoryRepository = new Mock<IGenericRepository<Inventory>>();
            _mockGenericJobRepository = new Mock<IGenericRepository<Job>>();
            _mockGenericRatingRepository = new Mock<IGenericRepository<Rating>>();
            _mockMapper = new Mock<IMapper>();

            _productService = new ProductService(
                _mockProductRepository.Object,
                _mockGenericProductRepository.Object,
                _mockGenericCategoryRepository.Object,
                _mockRatingRepository.Object,
                _mockGenericInventoryRepository.Object,
                _mockGenericJobRepository.Object,
                _mockGenericRatingRepository.Object,
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

            _mockGenericCategoryRepository.Setup(repo => repo.GetByCondition(It.IsAny<System.Linq.Expressions.Expression<System.Func<Category, bool>>>()))
                             .ReturnsAsync(category);

            _mockGenericProductRepository.Setup(repo => repo.Add(It.IsAny<Product>()))
                                   .ReturnsAsync(1);

            _mockGenericInventoryRepository.Setup(repo => repo.Add(It.IsAny<Inventory>()))
                              .ReturnsAsync(1);

            var userRole = AppConstants.ADMIN;

            // Act
            var result = await _productService.CreateProducts(request, userRole) ;

            // Assert
            var data = result.Data as ProductCreatedResponseDto;
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(request.Title, data.Title);
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

            _mockGenericCategoryRepository.Setup(repo => repo.GetByCondition(It.IsAny<System.Linq.Expressions.Expression<System.Func<Category, bool>>>()))
                             .ReturnsAsync((Category)null);

            // Act
            var result = await _productService.CreateProducts(request, AppConstants.ADMIN);

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

            _mockGenericProductRepository.Setup(r => r.GetByCondition(p => p.Id == 1))
                                   .ReturnsAsync(product);
            _mockGenericCategoryRepository.Setup(r => r.GetByCondition(c => c.Id == 2))
                             .ReturnsAsync(category);
            _mockRatingRepository.Setup(r => r.GetRatingsByProductId(1))
                           .ReturnsAsync(ratingList);
            _mockGenericInventoryRepository.Setup(r => r.GetByCondition(i => i.ProductId == 1))
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
            var result = await _productService.GetProductById(1);

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
            _mockGenericProductRepository.Setup(r => r.GetByCondition(p => p.Id == 99))
                                   .ReturnsAsync((Product)null);

            // Act
            var result = await _productService.GetProductById(99);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(ValidationMessages.MESSAGE_PRODUCT_NOT_FOUND, result.Message);
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

        [Fact]
        public async Task UpdateProduct_ShouldReturnSuccess_WhenProductExistsAndValidUpdate()
        {
            // Arrange
            int productId = 1;
            var request = new ProductUpdateRequestDto
            {
                Price = 150,
                Category = "NewCategory",
                Inventory = new InventoryRequestDto { Total = 100, Available = 50 }
            };

            var existingProduct = new Product
            {
                Id = productId,
                Price = 100,
                Inventory = new Inventory { ProductId = productId, Total = 100, Available = 50 }
            };

            var newCategory = new Category { Id = 2, Name = "NewCategory" };

            _mockGenericProductRepository
                .Setup(repo => repo.GetEntityWithIncludes(It.IsAny<string[]>()))
                .ReturnsAsync(new List<Product> { existingProduct });

            _mockGenericCategoryRepository
                .Setup(repo => repo.GetByCondition(It.IsAny<System.Linq.Expressions.Expression<Func<Category, bool>>>()))
                .ReturnsAsync(newCategory);

            // Setup mapper to map the update request onto the product.
            _mockMapper.Setup(m => m.Map(request, existingProduct)).Verifiable();

            _mockGenericProductRepository
                .Setup(repo => repo.Update(existingProduct))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _productService.UpdateProduct(productId, request);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(ValidationMessages.MESSAGE_PRODUCT_UPDATED_SUCCESSFULLY, result.Message);
        }

        [Fact]
        public async Task UpdateProduct_ShouldReturnProductNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            int productId = 99;
            var request = new ProductUpdateRequestDto { Price = 150 };

            _mockGenericProductRepository
                .Setup(repo => repo.GetEntityWithIncludes(It.IsAny<string[]>()))
                .ReturnsAsync(new List<Product>()); // No products found

            // Act
            var result = await _productService.UpdateProduct(productId, request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(ValidationMessages.MESSAGE_PRODUCT_NOT_FOUND, result.Message);
        }

        [Fact]
        public async Task UpdateProduct_ShouldReturnFailure_WhenAvailableInventoryGreaterThanTotal()
        {
            // Arrange
            int productId = 1;
            var request = new ProductUpdateRequestDto
            {
                Price = 150,
                Inventory = new InventoryRequestDto { Total = 50, Available = 60 } // Invalid: Available > Total
            };

            var existingProduct = new Product
            {
                Id = productId,
                Price = 100,
                Inventory = new Inventory { ProductId = productId, Total = 50, Available = 40 }
            };

            _mockGenericProductRepository
                .Setup(repo => repo.GetEntityWithIncludes(It.IsAny<string[]>()))
                .ReturnsAsync(new List<Product> { existingProduct });

            // Act
            var result = await _productService.UpdateProduct(productId, request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Available inventory cannot be greater than total inventory.", result.Message);
        }

        [Fact]
        public async Task UpdateProduct_ShouldReturnFailure_WhenCategoryNotFound()
        {
            // Arrange
            int productId = 1;
            var request = new ProductUpdateRequestDto
            {
                Price = 150,
                Category = "NonexistentCategory",
                Inventory = new InventoryRequestDto { Total = 100, Available = 50 }
            };

            var existingProduct = new Product
            {
                Id = productId,
                Price = 100,
                Inventory = new Inventory { ProductId = productId, Total = 100, Available = 50 }
            };

            _mockGenericProductRepository
                .Setup(repo => repo.GetEntityWithIncludes(It.IsAny<string[]>()))
                .ReturnsAsync(new List<Product> { existingProduct });

            _mockGenericCategoryRepository
                .Setup(repo => repo.GetByCondition(It.IsAny<System.Linq.Expressions.Expression<Func<Category, bool>>>()))
                .ReturnsAsync((Category)null); // Category not found

            // Act
            var result = await _productService.UpdateProduct(productId, request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(ValidationMessages.CATEGORY_NOT_FOUND, result.Message);
        }

        [Fact]
        public async Task DeleteProduct_ShouldReturnProductNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            int productId = 99;
            _mockGenericProductRepository
                .Setup(repo => repo.GetByCondition(It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>()))
                .ReturnsAsync((Product)null);

            // Act
            var result = await _productService.DeleteProduct(productId, AppConstants.ADMIN);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(ValidationMessages.MESSAGE_PRODUCT_NOT_FOUND, result.Message);
        }

        [Fact]
        public async Task DeleteProduct_ShouldReturnSuccess_ForAdmin()
        {
            // Arrange
            int productId = 1;
            var product = new Product { Id = productId };
            _mockGenericProductRepository
                .Setup(repo => repo.GetByCondition(It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>()))
                .ReturnsAsync(product);

            _mockGenericProductRepository
                .Setup(repo => repo.Delete(productId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _productService.DeleteProduct(productId, AppConstants.ADMIN);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(ValidationMessages.MESSAGE_PRODUCT_DELETED_SUCCESSFULLY, result.Message);
        }

        [Fact]
        public async Task DeleteProduct_ShouldReturnSuccess_ForEmployee()
        {
            // Arrange
            int productId = 1;
            var product = new Product { Id = productId };
            _mockGenericProductRepository
                .Setup(repo => repo.GetByCondition(It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>()))
                .ReturnsAsync(product);

            _mockGenericJobRepository
                .Setup(repo => repo.Add(It.IsAny<Job>()))
                .ReturnsAsync(1);

            // Act
            var result = await _productService.DeleteProduct(productId, AppConstants.EMPLOYEE);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(ValidationMessages.MESSAGE_PRODUCT_DELETED_EMPLOYEE_SUCCESSFULLY, result.Message);
        }

    }
}

