using Microsoft.EntityFrameworkCore;
using TTE.Infrastructure.Data;
using TTE.Infrastructure.Models;

namespace TTE.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<Product> Products, int TotalCount)> GetProducts(
            string? category, string? orderBy, bool descending, int page, int pageSize)
        {
            IQueryable<Product> query = _context.Products.Include(p => p.Category).Where(p => p.Approved);

            if (!string.IsNullOrEmpty(category))
                query = query.Where(p => p.Category.Name == category);

            query = orderBy switch
            {
                "price" => descending ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
                "title" => descending ? query.OrderByDescending(p => p.Title) : query.OrderBy(p => p.Title),
                _ => query.OrderBy(p => p.Title)
            };

            int totalCount = await query.CountAsync();

            int validatedPage = Math.Max(page, 1);
            var products = await query.Skip((validatedPage - 1) * pageSize)
                                      .Take(pageSize)
                                      .ToListAsync();

            return (products, totalCount);
        }

        public async Task<List<string>> GetTopCategoryNamesByProductCount(int top)
        {
            return await _context.Products
                .Where(p => p.Approved)
                .GroupBy(p => p.Category.Name)
                .Select(g => new { CategoryName = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .Take(top)
                .Select(g => g.CategoryName)
                .ToListAsync();
        }

    }
}
