using TTE.Application.DTOs;

namespace TTE.Application.Interfaces
{
    public interface IOrderService
    {
        Task<GenericResponseDto<List<OrderDto>>> GetOrdersByUser(int userId);
        Task<GenericResponseDto<OrderDto>> GetOrderById(int orderId);
        Task<GenericResponseDto<int>> CreateOrderFromCart(int userId, OrderRequestDto request);
    }
}
