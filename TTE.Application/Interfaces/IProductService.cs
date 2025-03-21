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
        Task<GenericResponseDto<string>> UpdateProduct(ProductRequestDto request);
        Task<ProductPaginatedResponseDto> GetProducts(string? category, string? orderBy, bool descending, int page, int pageSize);
    }
}
