﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTE.Application.DTOs;

namespace TTE.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<GenericResponseDto<CategoryResponseDto>> CreateCategory(CategoryRequestDto request, string userRole, string userName);
        Task<GenericResponseDto<CategoryResponseDto>> GetCategories();
        Task<GenericResponseDto<string>> DeleteCategory(int id, string userRole, string userName);
        Task<GenericResponseDto<string>> UpdateCategory(int id, CategoryRequestDto request);
        Task<GenericResponseDto<string>> GetTopCategoryNamesByProductCount(int top);
    }
}
