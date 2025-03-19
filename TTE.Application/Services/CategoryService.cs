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

        public async Task<GenericResponseDto<CategoryResponseDto>> GetCategories()
        {
            var categories = await _categoryRepository.GetAllByCondition(C => C.Approved == true);
            var categoryDtos = categories.Select(c => new CategoryResponseDto
            {
                Id = c.Id,
                Name = c.Name
            }).ToList();
            return new GenericResponseDto<CategoryResponseDto>(true, ValidationMessages.CATEGORIES_RETRIEVED_SUCCESSFULLY, categoryDtos);
        
        }

        public async Task<GenericResponseDto<string>> UpdateCategory(int id, CategoryRequestDto request)
        {
            var categoryToUpdate = await _categoryRepository.GetByCondition(c => c.Id == id);
            if (categoryToUpdate == null)
            {
                return new GenericResponseDto<string>(false, ValidationMessages.CATEGORY_NOT_FOUND);
            }
            categoryToUpdate.Name = request.Name;

            await _categoryRepository.Update(categoryToUpdate);

            return new GenericResponseDto<string>(true, ValidationMessages.CATEGORY_UPDATED_SUCCESSFULLY);

        }
    }
}
