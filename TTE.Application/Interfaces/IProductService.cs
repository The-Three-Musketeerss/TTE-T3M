using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTE.Application.DTOs;

namespace TTE.Application.Interfaces
{
    public interface IProductService
    {
        Task<ProductPaginatedResponseDto> GetProducts(string? category, string? orderBy, bool descending, int page, int pageSize);
        Task<GenericResponseDto<ProductCreatedResponseDto>> CreateProducts(ProductRequestDto request, string userRole);
        Task<GenericResponseDto<ProductByIdResponse>> GetProductById(int productId);
        Task<GenericResponseDto<string>> UpdateProduct(int productId, ProductUpdateRequestDto request);
        Task<GenericResponseDto<string>> DeleteProduct(int productId, string userRole);

    }
}
