using AutoMapper;
using TTE.Application.DTOs;
using TTE.Application.Interfaces;
using TTE.Commons.Constants;
using TTE.Infrastructure.Models;
using TTE.Infrastructure.Repositories;

namespace TTE.Application.Services
{
    public class CartService : ICartService
    {
        private readonly IGenericRepository<Cart> _cartRepo;
        private readonly IGenericRepository<Cart_Item> _cartItemRepo;
        private readonly IGenericRepository<Product> _productRepo;
        private readonly IGenericRepository<Coupon> _couponRepo;
        private readonly IMapper _mapper;

        public CartService(
            IGenericRepository<Cart> cartRepo,
            IGenericRepository<Cart_Item> cartItemRepo,
            IGenericRepository<Product> productRepo,
            IGenericRepository<Coupon> couponRepo,
            IMapper mapper)
        {
            _cartRepo = cartRepo;
            _cartItemRepo = cartItemRepo;
            _productRepo = productRepo;
            _couponRepo = couponRepo;
            _mapper = mapper;
        }

        public async Task<GenericResponseDto<CartResponseDto>> GetCart(int userId)
        {
            var cart = await _cartRepo.GetByCondition(c => c.UserId == userId, "Coupon");
            if (cart == null)
                return new GenericResponseDto<CartResponseDto>(false, ValidationMessages.MESSAGE_CART_NOT_FOUND);

            var cartItems = await _cartItemRepo.GetAllByCondition(i => i.CartId == cart.Id);
            var itemDtos = _mapper.Map<List<CartItemResponseDto>>(cartItems);

            var couponDto = cart.Coupon != null
                ? _mapper.Map<CouponAppliedDto>(cart.Coupon)
                : null;

            var response = new CartResponseDto
            {
                UserId = userId,
                ShoppingCart = itemDtos,
                TotalBeforeDiscount = cart.Total_before_discount,
                TotalAfterDiscount = cart.Total_after_discount,
                ShippingCost = cart.ShippingCost,
                FinalTotal = cart.Total_after_discount + cart.ShippingCost,
                CouponApplied = couponDto
            };

            return new GenericResponseDto<CartResponseDto>(true, ValidationMessages.MESSAGE_CART_RETRIEVED_SUCCESSFULLY, response);
        }

        public async Task<GenericResponseDto<string>> AddOrUpdateCartItem(int userId, CartItemRequestDto request)
        {
            var cart = await _cartRepo.GetByCondition(c => c.UserId == userId, "Coupon");
            if (cart == null)
            {
                cart = new Cart { UserId = userId, ShippingCost = 0 };
                await _cartRepo.Add(cart);
            }

            var existingItem = await _cartItemRepo.GetByCondition(i =>
                i.CartId == cart.Id && i.ProductId == request.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity = request.Quantity;
                await _cartItemRepo.Update(existingItem);
            }
            else
            {
                await _cartItemRepo.Add(new Cart_Item
                {
                    CartId = cart.Id,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity
                });
            }

            await UpdateCartTotals(cart);

            return new GenericResponseDto<string>(true, ValidationMessages.MESSAGE_CART_ITEM_ADDED);
        }

        public async Task<GenericResponseDto<string>> RemoveCartItem(int userId, int productId)
        {
            var cart = await _cartRepo.GetByCondition(c => c.UserId == userId, "Coupon");
            if (cart == null)
                return new GenericResponseDto<string>(false, ValidationMessages.MESSAGE_CART_NOT_FOUND);

            var item = await _cartItemRepo.GetByCondition(i => i.CartId == cart.Id && i.ProductId == productId);
            if (item == null)
                return new GenericResponseDto<string>(false, ValidationMessages.MESSAGE_CART_ITEM_NOT_FOUND);

            await _cartItemRepo.Delete(item);

            await UpdateCartTotals(cart);

            return new GenericResponseDto<string>(true, ValidationMessages.MESSAGE_CART_ITEM_DELETED);
        }


        public async Task<GenericResponseDto<string>> ApplyCoupon(int userId, ApplyCouponDto request)
        {
            var cart = await _cartRepo.GetByCondition(c => c.UserId == userId, "Coupon");
            if (cart == null)
                return new GenericResponseDto<string>(false, ValidationMessages.MESSAGE_CART_NOT_FOUND);

            var coupon = await _couponRepo.GetByCondition(c => c.Code == request.Code);
            if (coupon == null)
                return new GenericResponseDto<string>(false, ValidationMessages.MESSAGE_COUPON_NOT_FOUND);

            cart.CouponId = coupon.Id;
            cart.Coupon = coupon;
            await _cartRepo.Update(cart);

            await UpdateCartTotals(cart);

            return new GenericResponseDto<string>(true, ValidationMessages.MESSAGE_COUPON_APPLIED_SUCCESSFULLY);
        }

        private async Task UpdateCartTotals(Cart cart)
        {
            var cartItems = await _cartItemRepo.GetAllByCondition(i => i.CartId == cart.Id);
            var products = await _productRepo.Get();

            decimal totalBefore = cartItems.Sum(item =>
            {
                var product = products.FirstOrDefault(p => p.Id == item.ProductId);
                return product != null ? product.Price * item.Quantity : 0;
            });

            decimal discount = 0;

            if (cart.CouponId.HasValue)
            {
                cart.Coupon ??= await _couponRepo.GetByCondition(c => c.Id == cart.CouponId.Value);
                if (cart.Coupon != null)
                    discount = totalBefore * (cart.Coupon.Discount / 100m);
            }

            cart.Total_before_discount = totalBefore;
            cart.Total_after_discount = totalBefore - discount;

            await _cartRepo.Update(cart);
        }
    }
}
