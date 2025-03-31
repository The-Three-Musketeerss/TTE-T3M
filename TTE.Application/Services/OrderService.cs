using AutoMapper;
using TTE.Application.DTOs;
using TTE.Application.Interfaces;
using TTE.Commons.Constants;
using TTE.Infrastructure.Models;
using TTE.Infrastructure.Repositories;

namespace TTE.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IGenericRepository<Order> _orderRepo;
        private readonly IGenericRepository<Order_Item> _orderItemRepo;
        private readonly IGenericRepository<Cart> _cartRepo;
        private readonly IGenericRepository<Cart_Item> _cartItemRepo;
        private readonly IGenericRepository<Product> _productRepo;
        private readonly IGenericRepository<Inventory> _inventoryRepo;
        private readonly IMapper _mapper;

        public OrderService(
            IGenericRepository<Order> orderRepo,
            IGenericRepository<Order_Item> orderItemRepo,
            IGenericRepository<Cart> cartRepo,
            IGenericRepository<Cart_Item> cartItemRepo,
            IGenericRepository<Product> productRepo,
            IGenericRepository<Inventory> inventoryRepo,
            IMapper mapper)
        {
            _orderRepo = orderRepo;
            _orderItemRepo = orderItemRepo;
            _cartRepo = cartRepo;
            _cartItemRepo = cartItemRepo;
            _productRepo = productRepo;
            _inventoryRepo = inventoryRepo;
            _mapper = mapper;
        }

        public async Task<GenericResponseDto<int>> CreateOrderFromCart(int userId)
        {
            var cart = await _cartRepo.GetByCondition(c => c.UserId == userId, "Coupon");
            if (cart == null)
                return new GenericResponseDto<int>(false, ValidationMessages.MESSAGE_CART_NOT_FOUND);

            var cartItems = await _cartItemRepo.GetAllByCondition(i => i.CartId == cart.Id);
            if (!cartItems.Any())
                return new GenericResponseDto<int>(false, ValidationMessages.MESSAGE_CART_EMPTY);

            var products = await _productRepo.Get();
            var inventories = await _inventoryRepo.Get();

            foreach (var item in cartItems)
            {
                var product = products.FirstOrDefault(p => p.Id == item.ProductId);
                if (product == null)
                    return new GenericResponseDto<int>(false, ValidationMessages.MESSAGE_PRODUCT_NOT_FOUND);

                var inventory = inventories.FirstOrDefault(i => i.ProductId == item.ProductId);
                if (inventory == null)
                    return new GenericResponseDto<int>(false, ValidationMessages.MESSAGE_INVENTORY_NOT_FOUND);

                if (inventory.Available < item.Quantity)
                    return new GenericResponseDto<int>(false, string.Format(ValidationMessages.MESSAGE_INVENTORY_NOT_ENOUGH, product.Title, inventory.Available, item.Quantity));

                inventory.Available -= item.Quantity;
                await _inventoryRepo.Update(inventory);
            }

            var order = new Order
            {
                UserId = userId,
                Total_before_discount = cart.Total_before_discount,
                Total_after_discount = cart.Total_after_discount,
                FinalTotal = cart.Total_after_discount + cart.ShippingCost,
                ShippingCost = cart.ShippingCost,
                CouponId = cart.CouponId,
                CreatedAt = DateTime.UtcNow,
                Status = "Approved"
            };

            await _orderRepo.Add(order);

            foreach (var item in cartItems)
            {
                var product = products.First(p => p.Id == item.ProductId);
                var orderItem = new Order_Item
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = product.Price
                };
                await _orderItemRepo.Add(orderItem);
            }

            foreach (var item in cartItems)
                await _cartItemRepo.Delete(item);

            cart.Total_before_discount = 0;
            cart.Total_after_discount = 0;
            cart.CouponId = null;
            cart.Coupon = null;
            cart.ShippingCost = 0;

            await _cartRepo.Update(cart);

            return new GenericResponseDto<int>(true, ValidationMessages.MESSAGE_ORDER_CREATED_SUCCESSFULLY, order.Id);
        }

        public async Task<GenericResponseDto<List<OrderDto>>> GetOrdersByUser(int userId)
        {
            var orders = await _orderRepo.GetAllByCondition(o => o.UserId == userId);
            var orderDtos = new List<OrderDto>();

            foreach (var order in orders)
            {
                var items = await _orderItemRepo.GetAllByCondition(i => i.OrderId == order.Id);
                var orderDto = _mapper.Map<OrderDto>(order);
                orderDto.OrderItems = _mapper.Map<List<OrderItemDto>>(items);

                orderDtos.Add(orderDto);
            }

            return new GenericResponseDto<List<OrderDto>>(true, ValidationMessages.MESSAGE_ORDERS_RETRIEVED_SUCCESSFULLY, orderDtos);
        }
    }
}
