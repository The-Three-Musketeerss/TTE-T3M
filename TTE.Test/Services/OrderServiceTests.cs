using AutoMapper;
using Moq;
using TTE.Application.DTOs;
using TTE.Application.Services;
using TTE.Commons.Constants;
using TTE.Infrastructure.Models;
using TTE.Infrastructure.Repositories;

namespace TTE.Tests.Services
{
    public class OrderServiceTests
    {
        private readonly Mock<IGenericRepository<Order>> _mockOrderRepo;
        private readonly Mock<IGenericRepository<Order_Item>> _mockOrderItemRepo;
        private readonly Mock<IGenericRepository<Cart>> _mockCartRepo;
        private readonly Mock<IGenericRepository<Cart_Item>> _mockCartItemRepo;
        private readonly Mock<IGenericRepository<Product>> _mockProductRepo;
        private readonly Mock<IGenericRepository<Coupon>> _mockCouponRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly OrderService _service;

        public OrderServiceTests()
        {
            _mockOrderRepo = new Mock<IGenericRepository<Order>>();
            _mockOrderItemRepo = new Mock<IGenericRepository<Order_Item>>();
            _mockCartRepo = new Mock<IGenericRepository<Cart>>();
            _mockCartItemRepo = new Mock<IGenericRepository<Cart_Item>>();
            _mockProductRepo = new Mock<IGenericRepository<Product>>();
            _mockMapper = new Mock<IMapper>();

            _service = new OrderService(
                _mockOrderRepo.Object,
                _mockOrderItemRepo.Object,
                _mockCartRepo.Object,
                _mockCartItemRepo.Object,
                _mockProductRepo.Object,
                _mockMapper.Object
            );
        }

        [Fact]
        public async Task CreateOrderFromCart_ShouldReturnFail_WhenCartIsNull()
        {
            _mockCartRepo.Setup(r => r.GetByCondition(c => c.UserId == 1, "Coupon"))
                .ReturnsAsync((Cart)null);

            // Act
            var result = await _service.CreateOrderFromCart(1);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(ValidationMessages.MESSAGE_CART_NOT_FOUND, result.Message);
        }

        [Fact]
        public async Task CreateOrderFromCart_ShouldReturnFail_WhenCartIsEmpty()
        {
            var cart = new Cart { Id = 1, UserId = 1 };
            _mockCartRepo.Setup(r => r.GetByCondition(c => c.UserId == 1, "Coupon")).ReturnsAsync(cart);
            _mockCartItemRepo.Setup(r => r.GetAllByCondition(i => i.CartId == cart.Id)).ReturnsAsync(new List<Cart_Item>());

            // Act
            var result = await _service.CreateOrderFromCart(1);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(ValidationMessages.MESSAGE_CART_EMPTY, result.Message);
        }

        [Fact]
        public async Task GetOrdersByUser_ShouldReturnList()
        {
            var orders = new List<Order> { new Order { Id = 1, UserId = 1 } };
            var items = new List<Order_Item>
            {
                new Order_Item { OrderId = 1, ProductId = 1, Quantity = 1, Price = 10 }
            };

            _mockOrderRepo.Setup(r => r.GetAllByCondition(o => o.UserId == 1)).ReturnsAsync(orders);
            _mockOrderItemRepo.Setup(r => r.GetAllByCondition(i => i.OrderId == 1)).ReturnsAsync(items);

            _mockMapper.Setup(m => m.Map<OrderDto>(orders[0])).Returns(new OrderDto { Id = 1 });
            _mockMapper.Setup(m => m.Map<List<OrderItemDto>>(items)).Returns(new List<OrderItemDto> {
                new OrderItemDto { ProductId = 1, Quantity = 1, Price = 10 }
            });

            // Act
            var result = await _service.GetOrdersByUser(1);

            // Assert
            Assert.True(result.Success);
            var list = Assert.IsType<List<OrderDto>>(result.Data);
            Assert.Single(list);
            Assert.Equal(1, list.First().Id);
        }

    }
}
