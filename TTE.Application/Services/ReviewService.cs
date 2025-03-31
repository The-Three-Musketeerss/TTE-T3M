using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTE.Application.DTOs;
using TTE.Application.Interfaces;
using TTE.Commons.Constants;
using TTE.Infrastructure.Models;
using TTE.Infrastructure.Repositories;

namespace TTE.Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IGenericRepository<Review> _reviewRepository;
        private readonly IGenericRepository<Rating> _genericRatingRepository;
        private readonly IRatingRepository _ratingRepository;
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IGenericRepository<User> _userRepository;
        private readonly IGenericRepository<Job> _jobRepository;

        public ReviewService(
        IGenericRepository<Review> reviewRepository,
        IGenericRepository<Rating> genericRatingRepository,
        IGenericRepository<Product> productRepository,
        IGenericRepository<User> userRepository,
        IRatingRepository ratingRepository,
        IGenericRepository<Job> jobRepository)
        {
            _reviewRepository = reviewRepository;
            _genericRatingRepository = genericRatingRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
            _ratingRepository = ratingRepository;
            _jobRepository = jobRepository;
        }


        public async Task<GenericResponseDto<string>> AddReview(int productId, ReviewRequestDto request)
        {
            var product = await _productRepository.GetByCondition(p => p.Id == productId);
            if (product == null) {
                return new GenericResponseDto<string> (false, ValidationMessages.MESSAGE_PRODUCT_NOT_FOUND);
            }

            var user = await _userRepository.GetByCondition(u => u.UserName == request.User);
            if (user == null)
            {
                return new GenericResponseDto<string>(false, ValidationMessages.MESSAGE_USER_NOT_FOUND);
            }

            var existingReview = await _reviewRepository.GetByCondition(
                r => r.ProductId == productId && r.UserId == user.Id);

            if (existingReview != null)
            {
                await _reviewRepository.Delete(existingReview.Id);
            }


            var existingRating = await _genericRatingRepository.GetByCondition(
                r => r.ProductId == productId && r.UserId == user.Id);

            if (existingRating != null)
            {
                await _genericRatingRepository.Delete(existingRating.Id);
            }

            var newReview = new Review
            {
                ProductId = productId,
                UserId = user.Id,
                Comment = request.Review,
                CreatedAt = DateTime.Now
            };

            await _reviewRepository.Add(newReview);

            var rating = new Rating
            {
                ProductId = productId,
                UserId = user.Id,
                Rate = request.Rating
            };
            await _genericRatingRepository.Add(rating);

            return new GenericResponseDto<string>(true, ValidationMessages.MESSAGE_REVIEW_ADDED_SUCCESSFULLY);
        }

        public async Task<GenericResponseDto<List<ReviewResponseDto>>> GetReviews(int productId)
        {
            var product = await _productRepository.GetByCondition(p => p.Id == productId);
            if (product == null)
                return new GenericResponseDto<List<ReviewResponseDto>>(false, ValidationMessages.MESSAGE_PRODUCT_NOT_FOUND);

            var reviews = await _reviewRepository.GetAllByCondition(r => r.ProductId == productId,AppConstants.USER);

            var ratings = await _ratingRepository.GetRatingsByProductId(productId);

            var response = reviews.Select(r =>
            {
                var matchingRating = ratings.FirstOrDefault(rt => rt.UserId == r.UserId && rt.ProductId == r.ProductId);
                return new ReviewResponseDto
                {
                    User = r.User.Name,
                    Review = r.Comment,
                    Rating = matchingRating?.Rate ?? 0
                };
            }).ToList();

            return new GenericResponseDto<List<ReviewResponseDto>>(true, "", response);
        }

    }
}
