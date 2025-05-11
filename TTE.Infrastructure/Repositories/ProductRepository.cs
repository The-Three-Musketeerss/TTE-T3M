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
    string? category, string? orderBy, bool descending, int page, int pageSize, string? search)
        {
            IQueryable<Product> query = _context.Products
                .Include(p => p.Category)
                .Where(p => p.Approved);

            if (!string.IsNullOrEmpty(category))
                query = query.Where(p => p.Category.Name == category);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(p => p.Title.Contains(search) || p.Description.Contains(search));

            query = orderBy switch
            {
                "price" => descending ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
                "title" => descending ? query.OrderByDescending(p => p.Title) : query.OrderBy(p => p.Title),
                _ => query.OrderBy(p => p.Title)
            };

            int totalCount = await query.CountAsync();

            var products = await query
                .Skip((Math.Max(page, 1) - 1) * pageSize)
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

        public async Task<List<Product>> GetLatestProducts(int count = 3)
        {
            return await _context.Products
                .Where(p => p.Approved)
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task<List<Product>> GetTopSellingProducts(int count = 3)
        {
            var topSoldIds = await _context.Order_Items
                .GroupBy(oi => oi.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalSold = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(g => g.TotalSold)
                .Take(count)
                .Select(g => g.ProductId)
                .ToListAsync();

            var topSoldProducts = await _context.Products
                .Where(p => topSoldIds.Contains(p.Id) && p.Approved)
                .Include(p => p.Category)
                .ToListAsync();

            if (topSoldProducts.Count < count)
            {
                var needed = count - topSoldProducts.Count;

                var fallbackProducts = await _context.Products
                    .Where(p => p.Approved && !topSoldIds.Contains(p.Id))
                    .OrderByDescending(p => p.CreatedAt)
                    .Include(p => p.Category)
                    .Take(needed)
                    .ToListAsync();

                topSoldProducts.AddRange(fallbackProducts);
            }

            return topSoldProducts;
        }



    }
}
