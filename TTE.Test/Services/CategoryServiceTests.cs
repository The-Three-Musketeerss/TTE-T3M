using AutoMapper;
using Moq;
using TTE.Application.DTOs;
using TTE.Application.Services;
using TTE.Commons.Constants;
using TTE.Infrastructure.Models;
using TTE.Infrastructure.Repositories;

namespace TTE.Tests.Services
{
    public class CategoryServiceTests
    {
        private readonly Mock<IGenericRepository<Category>> _mockCategoryRepo;
        private readonly Mock<IGenericRepository<Job>> _mockJobRepo;
        private readonly Mock<IGenericRepository<Product>> _mockProductRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CategoryService _service;

        public CategoryServiceTests()
        {
            _mockCategoryRepo = new Mock<IGenericRepository<Category>>();
            _mockJobRepo = new Mock<IGenericRepository<Job>>();
            _mockProductRepo = new Mock<IGenericRepository<Product>>();
            _mockMapper = new Mock<IMapper>();

            _service = new CategoryService(
                _mockCategoryRepo.Object,
                _mockJobRepo.Object,
                _mockProductRepo.Object,
                _mockMapper.Object
            );
        }

        [Fact]
        public async Task CreateCategoryReturnSucess()
        {
            var userRole = AppConstants.ADMIN;
            var request = new CategoryRequestDto { Name = "Test" };
            _mockCategoryRepo.Setup(r => r.GetByCondition(c => c.Name == request.Name))
                             .ReturnsAsync((Category)null);
            var category = new Category { Name = request.Name };
            _mockMapper.Setup(m => m.Map<Category>(request)).Returns(category);
            _mockCategoryRepo.Setup(r => r.Add(category)).ReturnsAsync(1);
            var result = await _service.CreateCategory(request, userRole);
            Assert.True(result.Success);
            Assert.Equal(ValidationMessages.CATEGORY_CREATED_SUCCESSFULLY, result.Message);
        }

        [Fact]
        public async Task CreateCategoryReturnFail()
        {
            var userRole = AppConstants.ADMIN;
            var request = new CategoryRequestDto { Name = "Test" };
            _mockCategoryRepo.Setup(r => r.GetByCondition(c => c.Name == request.Name))
                             .ReturnsAsync(new Category());
            var result = await _service.CreateCategory(request, userRole);
            Assert.False(result.Success);
            Assert.Equal(ValidationMessages.CATEGORY_ALREADY_EXISTS, result.Message);
        }

        [Fact]
        public async Task GetCategoriesReturnSucess()
        {
            var categories = new List<Category> { new Category { Id = 1, Name = "Test" } };
            _mockCategoryRepo.Setup(r => r.Get()).ReturnsAsync(categories);
            var result = await _service.GetCategories();
            Assert.True(result.Success);
            Assert.Equal(ValidationMessages.CATEGORIES_RETRIEVED_SUCCESSFULLY, result.Message);
        }

        [Fact]
        public async Task UpdateCategory_ShouldReturnSuccess_WhenCategoryExists()
        {
            var categoryId = 1;
            var request = new CategoryRequestDto { Id = categoryId, Name = "Updated" };
            var existingCategory = new Category { Id = categoryId, Name = "Old" };

            _mockCategoryRepo.Setup(r => r.GetByCondition(c => c.Id == categoryId))
                             .ReturnsAsync(existingCategory);

            _mockMapper.Setup(m => m.Map(request, existingCategory));

            _mockCategoryRepo.Setup(r => r.Update(existingCategory))
                             .Returns(Task.CompletedTask);

            // Act
            var result = await _service.UpdateCategory(categoryId, request);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(ValidationMessages.CATEGORY_UPDATED_SUCCESSFULLY, result.Message);
        }

        [Fact]
        public async Task UpdateCategory_ShouldReturnFail_WhenCategoryNotFound()
        {
            var categoryId = 999;
            var request = new CategoryRequestDto { Id = categoryId, Name = "Test" };

            _mockCategoryRepo.Setup(r => r.GetByCondition(c => c.Id == categoryId))
                             .ReturnsAsync((Category)null);

            // Act
            var result = await _service.UpdateCategory(categoryId, request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(ValidationMessages.CATEGORY_NOT_FOUND, result.Message);
        }

        [Fact]
        public async Task DeleteCategory_ShouldReturnSuccess_WhenAdminAndNoProducts()
        {
            var categoryId = 1;
            var userRole = AppConstants.ADMIN;
            var category = new Category { Id = categoryId, Name = "Test" };

            _mockCategoryRepo.Setup(r => r.GetByCondition(c => c.Id == categoryId))
                             .ReturnsAsync(category);

            _mockProductRepo.Setup(r => r.GetAllByCondition(p => p.CategoryId == categoryId))
                            .ReturnsAsync(new List<Product>());

            _mockCategoryRepo.Setup(r => r.Delete(categoryId))
                             .Returns(Task.CompletedTask);

            // Act
            var result = await _service.DeleteCategory(categoryId, userRole);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(ValidationMessages.CATEGORY_DELETED_SUCCESSFULLY, result.Message);
        }

        [Fact]
        public async Task DeleteCategory_ShouldReturnFail_WhenCategoryHasProducts()
        {
            var categoryId = 1;
            var userRole = AppConstants.ADMIN;
            var category = new Category { Id = categoryId };
            var products = new List<Product> { new Product { Id = 1, CategoryId = categoryId } };

            _mockCategoryRepo.Setup(r => r.GetByCondition(c => c.Id == categoryId))
                             .ReturnsAsync(category);

            _mockProductRepo.Setup(r => r.GetAllByCondition(p => p.CategoryId == categoryId))
                            .ReturnsAsync(products);

            // Act
            var result = await _service.DeleteCategory(categoryId, userRole);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(ValidationMessages.CATEGORY_HAS_PRODUCTS, result.Message);
        }

        [Fact]
        public async Task DeleteCategory_ShouldCreateJob_WhenEmployeeAndNoProducts()
        {
            var categoryId = 1;
            var userRole = AppConstants.EMPLOYEE;
            var category = new Category { Id = categoryId };

            _mockCategoryRepo.Setup(r => r.GetByCondition(c => c.Id == categoryId))
                             .ReturnsAsync(category);

            _mockProductRepo.Setup(r => r.GetAllByCondition(p => p.CategoryId == categoryId))
                            .ReturnsAsync(new List<Product>());

            _mockJobRepo.Setup(r => r.Add(It.IsAny<Job>()))
                        .ReturnsAsync(1);

            // Act
            var result = await _service.DeleteCategory(categoryId, userRole);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(ValidationMessages.CATEGORY_DELETED_EMPLOYEE_SUCCESSFULLY, result.Message);
        }
    }
}
