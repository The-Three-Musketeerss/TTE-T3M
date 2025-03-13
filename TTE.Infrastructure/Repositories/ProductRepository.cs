using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await _context.Products.ToListAsync<Product>();
        }
    }
}
