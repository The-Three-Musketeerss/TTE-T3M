using AutoMapper;
using Moq;
using System.Linq.Expressions;
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
        private readonly Mock<IGenericRepository<Inventory>> _mockInventoryRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly OrderService _service;

        public OrderServiceTests()
        {
            _mockOrderRepo = new Mock<IGenericRepository<Order>>();
            _mockOrderItemRepo = new Mock<IGenericRepository<Order_Item>>();
            _mockCartRepo = new Mock<IGenericRepository<Cart>>();
            _mockCartItemRepo = new Mock<IGenericRepository<Cart_Item>>();
            _mockProductRepo = new Mock<IGenericRepository<Product>>();
            _mockInventoryRepo = new Mock<IGenericRepository<Inventory>>();
            _mockMapper = new Mock<IMapper>();

            _service = new OrderService(
                _mockOrderRepo.Object,
                _mockOrderItemRepo.Object,
                _mockCartRepo.Object,
                _mockCartItemRepo.Object,
                _mockProductRepo.Object,
                _mockInventoryRepo.Object,
                _mockMapper.Object
            );
        }

        [Fact]
        public async Task CreateOrderFromCart_ShouldReturnFail_WhenCartIsNull()
        {
            _mockCartRepo
                .Setup(r => r.GetByCondition(It.IsAny<Expression<Func<Cart, bool>>>(), It.IsAny<string[]>()))
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

            _mockCartRepo
                .Setup(r => r.GetByCondition(It.IsAny<Expression<Func<Cart, bool>>>(), It.IsAny<string[]>()))
                .ReturnsAsync(cart);

            _mockCartItemRepo
                .Setup(r => r.GetAllByCondition(It.IsAny<Expression<Func<Cart_Item, bool>>>()))
                .ReturnsAsync(new List<Cart_Item>());

            // Act
            var result = await _service.CreateOrderFromCart(1);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(ValidationMessages.MESSAGE_CART_EMPTY, result.Message);
        }

        [Fact]
        public async Task CreateOrderFromCart_ShouldFail_WhenInventoryNotEnough()
        {
            var cart = new Cart { Id = 1, UserId = 1 };
            var cartItems = new List<Cart_Item> {
                new Cart_Item { CartId = 1, ProductId = 1, Quantity = 5 }
            };
            var products = new List<Product> {
                new Product { Id = 1, Title = "Test Product", Price = 10 }
            };
            var inventories = new List<Inventory> {
                new Inventory { ProductId = 1, Available = 2 }
            };

            _mockCartRepo
                .Setup(r => r.GetByCondition(It.IsAny<Expression<Func<Cart, bool>>>(), It.IsAny<string[]>()))
                .ReturnsAsync(cart);

            _mockCartItemRepo
                .Setup(r => r.GetAllByCondition(It.IsAny<Expression<Func<Cart_Item, bool>>>()))
                .ReturnsAsync(cartItems);

            _mockProductRepo.Setup(r => r.Get()).ReturnsAsync(products);
            _mockInventoryRepo.Setup(r => r.Get()).ReturnsAsync(inventories);

            // Act
            var result = await _service.CreateOrderFromCart(1);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(
                string.Format(ValidationMessages.MESSAGE_INVENTORY_NOT_ENOUGH, "Test Product", 2, 5),
                result.Message
            );
        }

        [Fact]
        public async Task CreateOrderFromCart_ShouldSucceed_WhenInventoryIsSufficient()
        {
            var cart = new Cart
            {
                Id = 1,
                UserId = 1,
                Total_before_discount = 100,
                Total_after_discount = 90,
                ShippingCost = 10
            };
            var cartItems = new List<Cart_Item> {
                new Cart_Item { CartId = 1, ProductId = 1, Quantity = 2 }
            };
            var products = new List<Product> {
                new Product { Id = 1, Title = "Test Product", Price = 45 }
            };
            var inventories = new List<Inventory> {
                new Inventory { ProductId = 1, Available = 5 }
            };

            _mockCartRepo
                .Setup(r => r.GetByCondition(It.IsAny<Expression<Func<Cart, bool>>>(), It.IsAny<string[]>()))
                .ReturnsAsync(cart);

            _mockCartItemRepo
                .Setup(r => r.GetAllByCondition(It.IsAny<Expression<Func<Cart_Item, bool>>>()))
                .ReturnsAsync(cartItems);

            _mockProductRepo.Setup(r => r.Get()).ReturnsAsync(products);
            _mockInventoryRepo.Setup(r => r.Get()).ReturnsAsync(inventories);

            _mockOrderRepo.Setup(r => r.Add(It.IsAny<Order>())).ReturnsAsync(123);
            _mockOrderItemRepo.Setup(r => r.Add(It.IsAny<Order_Item>())).Returns(Task.FromResult(0));
            _mockInventoryRepo.Setup(r => r.Update(It.IsAny<Inventory>())).Returns(Task.CompletedTask);
            _mockCartItemRepo.Setup(r => r.Delete(It.IsAny<Cart_Item>())).Returns(Task.CompletedTask);
            _mockCartRepo.Setup(r => r.Update(It.IsAny<Cart>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateOrderFromCart(1);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(ValidationMessages.MESSAGE_ORDER_CREATED_SUCCESSFULLY, result.Message);
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
