using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTE.Application.DTOs;

namespace TTE.Application.Interfaces
{
    public interface IReviewService
    {
        Task<GenericResponseDto<List<ReviewResponseDto>>> GetReviews(int productId);
        Task<GenericResponseDto<string>> AddReview(int productId, ReviewRequestDto request);
    }
}
