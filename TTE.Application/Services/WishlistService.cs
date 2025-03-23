using TTE.Application.DTOs;
using TTE.Application.Interfaces;
using TTE.Commons.Constants;
using TTE.Infrastructure.Models;
using TTE.Infrastructure.Repositories;

namespace TTE.Application.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IGenericRepository<Wishlist> _wishlistRepo;
        private readonly IGenericRepository<Product> _productRepository;

        public WishlistService(IGenericRepository<Wishlist> wishlistRepo, IGenericRepository<Product> productRepository)
        {
            _wishlistRepo = wishlistRepo;
            _productRepository = productRepository;
        }

        public async Task<GenericResponseDto<WishlistResponseDto>> GetWishlist(int userId)
        {
            var items = await _wishlistRepo.GetAllByCondition(w => w.UserId == userId);
            var productIds = items.Select(w => w.ProductId).ToList();

            var dto = new WishlistResponseDto { Wishlist = productIds };
            return new GenericResponseDto<WishlistResponseDto>(true, ValidationMessages.MESSAGE_WISHLIST_RETRIEVED_SUCCESSFULLY, dto);
        }

        public async Task<GenericResponseDto<string>> AddToWishlist(int userId, int productId)
        {
            var product = await _productRepository.GetByCondition(p => p.Id == productId);
            if (product == null)
            {
                return new GenericResponseDto<string>(false, ValidationMessages.MESSAGE_PRODUCT_NOT_FOUND);
            }
            var existing = await _wishlistRepo.GetByCondition(w => w.UserId == userId && w.ProductId == productId);
            if (existing != null)
            {
                return new GenericResponseDto<string>(false, ValidationMessages.MESSAGE_PRODUCT_ALREADY_IN_WISHLIST);
            }

            var newItem = new Wishlist { UserId = userId, ProductId = productId };
            await _wishlistRepo.Add(newItem);

            return new GenericResponseDto<string>(true, ValidationMessages.MESSAGE_WISHLIST_PRODUCT_ADDED);
        }

        public async Task<GenericResponseDto<string>> RemoveFromWishlist(int userId, int productId)
        {
            var items = await _wishlistRepo.GetAllByCondition(w => w.UserId == userId && w.ProductId == productId);
            var item = items.FirstOrDefault();

            if (item == null)
            {
                return new GenericResponseDto<string>(false, ValidationMessages.MESSAGE_WISHLIST_PRODUCT_NOT_FOUND);
            }

            await _wishlistRepo.Delete(item.Id);
            return new GenericResponseDto<string>(true, ValidationMessages.MESSAGE_WISHLIST_PRODUCT_REMOVED);
        }
    }

}
