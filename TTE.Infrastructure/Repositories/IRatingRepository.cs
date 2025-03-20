using TTE.Infrastructure.Models;

namespace TTE.Infrastructure.Repositories
{
    public interface IRatingRepository
    {
        Task<IEnumerable<Rating>> GetRatingsByProductId(int productId);
        Task<List<Rating>> GetRatingsByProductIds(List<int> productIds);
    }
}
