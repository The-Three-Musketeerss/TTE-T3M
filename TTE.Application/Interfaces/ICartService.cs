using TTE.Application.DTOs;

namespace TTE.Application.Interfaces
{
    public interface ICartService
    {
        Task<GenericResponseDto<CartResponseDto>> GetCart(int userId);
        Task<GenericResponseDto<string>> AddOrUpdateCartItem(int userId, CartItemRequestDto request);
        Task<GenericResponseDto<string>> RemoveCartItem(int userId, int productId);
        Task<GenericResponseDto<string>> ApplyCoupon(int userId, ApplyCouponDto request);
    }
}
