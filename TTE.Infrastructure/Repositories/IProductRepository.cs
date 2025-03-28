﻿using TTE.Infrastructure.Models;

namespace TTE.Infrastructure.Repositories
{
    public interface IProductRepository
    {
        Task<(IEnumerable<Product> Products, int TotalCount)> GetProducts(
            string? category, string? orderBy, bool descending, int page, int pageSize);
    }
}
