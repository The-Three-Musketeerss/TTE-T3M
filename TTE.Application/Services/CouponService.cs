using AutoMapper;
using TTE.Application.DTOs;
using TTE.Application.Interfaces;
using TTE.Commons.Constants;
using TTE.Infrastructure.Models;
using TTE.Infrastructure.Repositories;

namespace TTE.Application.Services
{
    public class CouponService : ICouponService
    {
        private readonly IGenericRepository<Coupon> _couponRepository;
        private readonly IMapper _mapper;

        public CouponService(IGenericRepository<Coupon> couponRepository, IMapper mapper)
        {
            _couponRepository = couponRepository;
            _mapper = mapper;
        }

        public async Task<GenericResponseDto<string>> CreateCoupon(CouponRequestDto request)
        {
            var exists = await _couponRepository.GetByCondition(c => c.Code == request.Code);
            if (exists != null)
            {
                return new GenericResponseDto<string>(false, ValidationMessages.MESSAGE_COUPON_CODE_ALREADY_EXISTS);
            }

            var coupon = _mapper.Map<Coupon>(request);
            await _couponRepository.Add(coupon);
            return new GenericResponseDto<string>(true, ValidationMessages.MESSAGE_COUPON_CREATED_SUCCESSFULLY);
        }

        public async Task<GenericResponseDto<string>> UpdateCoupon(int id, CouponRequestDto request)
        {
            var coupon = await _couponRepository.GetByCondition(c => c.Id == id);
            if (coupon == null)
            {
                return new GenericResponseDto<string>(false, ValidationMessages.MESSAGE_COUPON_NOT_FOUND);
            }

            coupon.Code = request.Code;
            coupon.Discount = request.Discount;
            await _couponRepository.Update(coupon);

            return new GenericResponseDto<string>(true, ValidationMessages.MESSAGE_COUPON_UPDATED_SUCCESSFULLY);
        }

        public async Task<GenericResponseDto<string>> DeleteCoupon(int id)
        {
            var coupon = await _couponRepository.GetByCondition(c => c.Id == id);
            if (coupon == null)
            {
                return new GenericResponseDto<string>(false, ValidationMessages.MESSAGE_COUPON_NOT_FOUND);
            }

            await _couponRepository.Delete(id);
            return new GenericResponseDto<string>(true, ValidationMessages.MESSAGE_COUPON_DELETED_SUCCESSFULLY);
        }

        public async Task<GenericResponseDto<List<CouponResponseDto>>> GetAllCoupons()
        {
            var coupons = await _couponRepository.Get();
            var mapped = _mapper.Map<List<CouponResponseDto>>(coupons);
            return new GenericResponseDto<List<CouponResponseDto>>(true, ValidationMessages.MESSAGE_COUPONS_RETRIEVED_SUCCESSFULLY, mapped);
        }


    }
}
