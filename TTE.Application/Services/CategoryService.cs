using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTE.Application.DTOs;
using TTE.Application.Interfaces;
using TTE.Commons.Constants;
using TTE.Infrastructure.Models;
using TTE.Infrastructure.Repositories;

namespace TTE.Application.Services
{
    public class CategoryService: ICategoryService
    {
        private readonly IGenericRepository<Category> _categoryRepository;
        private readonly IGenericRepository<Job> _jobRepository;


        public CategoryService(IGenericRepository<Category> categoryRepository, IGenericRepository<Job> jobRepository)
        {
            _categoryRepository = categoryRepository;
            _jobRepository = jobRepository;
        }

        public async Task<GenericResponseDto<string>> DeleteCategory(int id, string userRole)
        {
            var categoryToDelete = await _categoryRepository.GetByCondition(c => c.Id == id);

            if (categoryToDelete == null)
            {
                return new GenericResponseDto<string>(false, ValidationMessages.CATEGORY_NOT_FOUND);
            }

            var job = new Job
            {
                Item_id = categoryToDelete.Id,
                CreatedAt = DateTime.Now,
                Type = Job.JobEnum.Category,
                Operation = Job.OperationEnum.Delete,
                Status = userRole == AppConstants.ADMIN ? Job.StatusEnum.Approved : Job.StatusEnum.Declined
            };

            await _jobRepository.Add(job);

            if (userRole == AppConstants.ADMIN)
            {
                await _categoryRepository.Delete(categoryToDelete.Id);
                return new GenericResponseDto<string>(true, ValidationMessages.CATEGORY_DELETED_SUCCESSFULLY);
            }

            return new GenericResponseDto<string>(true, ValidationMessages.CATEGORY_DELETED_EMPLOYEE_SUCCESSFULLY);
        }
    }
}
