using TTE.Application.DTOs;

namespace TTE.Application.Interfaces
{
    public interface IWishlistService
    {
        Task<GenericResponseDto<WishlistResponseDto>> GetWishlist(int userId);
        Task<GenericResponseDto<string>> AddToWishlist(int userId, int productId);
        Task<GenericResponseDto<string>> RemoveFromWishlist(int userId, int productId);
    }
}
