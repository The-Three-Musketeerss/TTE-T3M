using AutoMapper;
using Moq;
using TTE.Application.DTOs;
using TTE.Application.Services;
using TTE.Commons.Constants;
using TTE.Infrastructure.Models;
using TTE.Infrastructure.Repositories;

namespace TTE.Tests.Services
{
    public class CartServiceTests
    {
        private readonly Mock<IGenericRepository<Cart>> _mockCartRepo;
        private readonly Mock<IGenericRepository<Cart_Item>> _mockCartItemRepo;
        private readonly Mock<IGenericRepository<Product>> _mockProductRepo;
        private readonly Mock<IGenericRepository<Coupon>> _mockCouponRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CartService _service;

        public CartServiceTests()
        {
            _mockCartRepo = new Mock<IGenericRepository<Cart>>();
            _mockCartItemRepo = new Mock<IGenericRepository<Cart_Item>>();
            _mockProductRepo = new Mock<IGenericRepository<Product>>();
            _mockCouponRepo = new Mock<IGenericRepository<Coupon>>();
            _mockMapper = new Mock<IMapper>();

            _service = new CartService(
                _mockCartRepo.Object,
                _mockCartItemRepo.Object,
                _mockProductRepo.Object,
                _mockCouponRepo.Object,
                _mockMapper.Object
            );
        }

        [Fact]
        public async Task GetCart_ShouldReturnSuccess_WhenCartExists()
        {
            int userId = 1;
            var cart = new Cart { Id = 1, UserId = userId, Total_before_discount = 100, Total_after_discount = 90, ShippingCost = 10 };
            var items = new List<Cart_Item> { new Cart_Item { CartId = 1, ProductId = 1, Quantity = 2 } };
            var itemDtos = new List<CartItemResponseDto> { new CartItemResponseDto { ProductId = 1, Quantity = 2 } };

            _mockCartRepo.Setup(r => r.GetByCondition(c => c.UserId == userId, "Coupon")).ReturnsAsync(cart);
            _mockCartItemRepo.Setup(r => r.GetAllByCondition(i => i.CartId == cart.Id)).ReturnsAsync(items);
            _mockMapper.Setup(m => m.Map<List<CartItemResponseDto>>(items)).Returns(itemDtos);

            // Act
            var result = await _service.GetCart(userId);
            var cartDto = Assert.IsType<CartResponseDto>(result.Data);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(userId, cartDto.UserId);
            Assert.Equal(90, cartDto.TotalAfterDiscount);
        }

        [Fact]
        public async Task ApplyCoupon_ShouldReturnFail_WhenCouponDoesNotExist()
        {
            int userId = 1;
            var cart = new Cart { Id = 1, UserId = userId };
            _mockCartRepo.Setup(r => r.GetByCondition(c => c.UserId == userId, "Coupon")).ReturnsAsync(cart);
            _mockCouponRepo.Setup(r => r.GetByCondition(c => c.Code == "INVALID")).ReturnsAsync((Coupon)null);

            // Act
            var result = await _service.ApplyCoupon(userId, new ApplyCouponDto { Code = "INVALID" });

            // Assert
            Assert.False(result.Success);
            Assert.Equal(ValidationMessages.MESSAGE_COUPON_NOT_FOUND, result.Message);
        }

        [Fact]
        public async Task RemoveCartItem_ShouldReturnSuccess_WhenItemExists()
        {
            int userId = 1;
            int productId = 2;
            var cart = new Cart { Id = 1, UserId = userId };
            var cartItem = new Cart_Item { CartId = cart.Id, ProductId = productId };

            _mockCartRepo.Setup(r => r.GetByCondition(c => c.UserId == userId, "Coupon")).ReturnsAsync(cart);
            _mockCartItemRepo.Setup(r => r.GetByCondition(i => i.CartId == cart.Id && i.ProductId == productId)).ReturnsAsync(cartItem);

            // Act
            var result = await _service.RemoveCartItem(userId, productId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(ValidationMessages.MESSAGE_CART_ITEM_DELETED, result.Message);
        }

        [Fact]
        public async Task AddOrUpdateCartItem_ShouldCreateCart_WhenItDoesNotExist()
        {
            int userId = 5;
            var cart = new Cart { Id = 10, UserId = userId, ShippingCost = 0 };
            var request = new CartItemRequestDto { ProductId = 1, Quantity = 2 };

            _mockCartRepo.SetupSequence(r => r.GetByCondition(c => c.UserId == userId, "Coupon"))
                         .ReturnsAsync((Cart)null)
                         .ReturnsAsync(cart);
            _mockCartRepo.Setup(r => r.Add(It.IsAny<Cart>())).ReturnsAsync(1);

            // Act
            var result = await _service.AddOrUpdateCartItem(userId, request);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(ValidationMessages.MESSAGE_CART_ITEM_ADDED, result.Message);
        }
    }
}
