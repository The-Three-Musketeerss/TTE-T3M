﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTE.Application.DTOs;

namespace TTE.Application.Interfaces
{
    public interface IProductService
    {
        Task<ProductPaginatedDto> GetProductsAsync(string? category, string? orderBy, bool descending, int page, int pageSize);
    }
}
