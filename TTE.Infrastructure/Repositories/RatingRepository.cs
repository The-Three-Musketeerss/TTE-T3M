using Microsoft.EntityFrameworkCore;
using TTE.Infrastructure.Data;
using TTE.Infrastructure.Models;

namespace TTE.Infrastructure.Repositories
{
    public class RatingRepository : IRatingRepository
    {
        private readonly AppDbContext _context;

        public RatingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Rating>> GetRatingsByProductId(int productId)
        {
            return await _context.Ratings.Where(r => r.ProductId == productId).ToListAsync();
        }

        public async Task<List<Rating>> GetRatingsByProductIds(List<int> productIds)
        {
            return await _context.Ratings
                .Where(r => productIds.Contains(r.ProductId))
                .ToListAsync();
        }
    }
}
