using Moq;
using TTE.Application.DTOs;
using TTE.Application.Services;
using TTE.Commons.Constants;
using TTE.Infrastructure.Models;
using TTE.Infrastructure.Repositories;

namespace TTE.Tests.Services
{
    public class WishlistServiceTests
    {
        private readonly Mock<IGenericRepository<Wishlist>> _mockWishlistRepo;
        private readonly Mock<IGenericRepository<Product>> _mockProductRepo;
        private readonly WishlistService _wishlistService;

        public WishlistServiceTests()
        {
            _mockWishlistRepo = new Mock<IGenericRepository<Wishlist>>();
            _mockProductRepo = new Mock<IGenericRepository<Product>>();
            _wishlistService = new WishlistService(_mockWishlistRepo.Object, _mockProductRepo.Object);
        }

        [Fact]
        public async Task GetWishlist_ShouldReturnProductIds_WhenItemsExist()
        {
            var userId = 1;
            var items = new List<Wishlist>
            {
                new Wishlist { Id = 1, UserId = userId, ProductId = 10 },
                new Wishlist { Id = 2, UserId = userId, ProductId = 20 }
            };

            _mockWishlistRepo.Setup(r => r.GetAllByCondition(w => w.UserId == userId))
                             .ReturnsAsync(items);

            // Act
            var result = await _wishlistService.GetWishlist(userId);

            // Assert
            Assert.True(result.Success);
            var dto = Assert.IsType<WishlistResponseDto>(result.Data);
            Assert.Equal(new List<int> { 10, 20 }, dto.Wishlist);
        }

        [Fact]
        public async Task AddToWishlist_ShouldReturnFail_WhenProductDoesNotExist()
        {
            var userId = 1;
            var productId = 100;

            _mockProductRepo.Setup(r => r.GetByCondition(p => p.Id == productId))
                            .ReturnsAsync((Product)null);

            // Act
            var result = await _wishlistService.AddToWishlist(userId, productId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(ValidationMessages.MESSAGE_PRODUCT_NOT_FOUND, result.Message);
        }

        [Fact]
        public async Task AddToWishlist_ShouldReturnFail_WhenProductAlreadyInWishlist()
        {
            var userId = 1;
            var productId = 10;
            var product = new Product { Id = productId };

            _mockProductRepo.Setup(r => r.GetByCondition(p => p.Id == productId))
                            .ReturnsAsync(product);

            _mockWishlistRepo.Setup(r => r.GetByCondition(w => w.UserId == userId && w.ProductId == productId))
                             .ReturnsAsync(new Wishlist { Id = 1 });

            // Act
            var result = await _wishlistService.AddToWishlist(userId, productId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(ValidationMessages.MESSAGE_PRODUCT_ALREADY_IN_WISHLIST, result.Message);
        }

        [Fact]
        public async Task AddToWishlist_ShouldReturnSuccess_WhenNewProductIsAdded()
        {
            var userId = 1;
            var productId = 10;
            var product = new Product { Id = productId };

            _mockProductRepo.Setup(r => r.GetByCondition(p => p.Id == productId))
                            .ReturnsAsync(product);

            _mockWishlistRepo.Setup(r => r.GetByCondition(w => w.UserId == userId && w.ProductId == productId))
                             .ReturnsAsync((Wishlist)null);

            _mockWishlistRepo.Setup(r => r.Add(It.IsAny<Wishlist>()))
                             .ReturnsAsync(1);

            // Act
            var result = await _wishlistService.AddToWishlist(userId, productId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(ValidationMessages.MESSAGE_WISHLIST_PRODUCT_ADDED, result.Message);
        }

        [Fact]
        public async Task RemoveFromWishlist_ShouldReturnFail_WhenProductNotFoundInWishlist()
        {
            var userId = 1;
            var productId = 10;

            _mockWishlistRepo.Setup(r => r.GetAllByCondition(w => w.UserId == userId && w.ProductId == productId))
                             .ReturnsAsync(new List<Wishlist>());

            // Act
            var result = await _wishlistService.RemoveFromWishlist(userId, productId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(ValidationMessages.MESSAGE_WISHLIST_PRODUCT_NOT_FOUND, result.Message);
        }

        [Fact]
        public async Task RemoveFromWishlist_ShouldReturnSuccess_WhenProductIsRemoved()
        {
            var userId = 1;
            var productId = 10;
            var wishlistItem = new Wishlist { Id = 5, UserId = userId, ProductId = productId };

            _mockWishlistRepo.Setup(r => r.GetAllByCondition(w => w.UserId == userId && w.ProductId == productId))
                             .ReturnsAsync(new List<Wishlist> { wishlistItem });

            _mockWishlistRepo.Setup(r => r.Delete(wishlistItem.Id))
                             .Returns(Task.CompletedTask);

            // Act
            var result = await _wishlistService.RemoveFromWishlist(userId, productId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(ValidationMessages.MESSAGE_WISHLIST_PRODUCT_REMOVED, result.Message);
        }
    }
}
