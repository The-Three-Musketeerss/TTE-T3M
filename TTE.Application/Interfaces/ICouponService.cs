using TTE.Application.DTOs;

namespace TTE.Application.Interfaces
{
    public interface ICouponService
    {
        Task<GenericResponseDto<string>> CreateCoupon(CouponRequestDto request);
        Task<GenericResponseDto<string>> UpdateCoupon(int id, CouponRequestDto request);
        Task<GenericResponseDto<string>> DeleteCoupon(int id);
        Task<GenericResponseDto<List<CouponResponseDto>>> GetAllCoupons();
    }
}
