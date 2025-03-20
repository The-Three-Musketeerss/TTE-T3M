using AutoMapper;
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
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IMapper _mapper;


        public CategoryService(IGenericRepository<Category> categoryRepository, IGenericRepository<Job> jobRepository, IGenericRepository<Product> productRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _jobRepository = jobRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<GenericResponseDto<string>> DeleteCategory(int id, string userRole)
        {
            var categoryToDelete = await _categoryRepository.GetByCondition(c => c.Id == id);

            var productsWithCategory = await _productRepository.GetAllByCondition(p => p.CategoryId == id);
            if (productsWithCategory.Any())
            {
                return new GenericResponseDto<string>(false, ValidationMessages.CATEGORY_HAS_PRODUCTS);
            }

            if (categoryToDelete == null)
            {
                return new GenericResponseDto<string>(false, ValidationMessages.CATEGORY_NOT_FOUND);
            }

            if (userRole == AppConstants.ADMIN)
            {
                await _categoryRepository.Delete(categoryToDelete.Id);
                return new GenericResponseDto<string>(true, ValidationMessages.CATEGORY_DELETED_SUCCESSFULLY);
            }

            var job = new Job
            {
                Item_id = categoryToDelete.Id,
                CreatedAt = DateTime.Now,
                Type = Job.JobEnum.Category,
                Operation = Job.OperationEnum.Delete,
                Status = Job.StatusEnum.Pending
            };

            await _jobRepository.Add(job);

            return new GenericResponseDto<string>(true, ValidationMessages.CATEGORY_DELETED_EMPLOYEE_SUCCESSFULLY);
        }

        public async Task<GenericResponseDto<CategoryResponseDto>> GetCategories()
        {
            var categories = await _categoryRepository.GetAllByCondition(C => C.Approved == true);
            var categoryDtos = categories.Select(c => _mapper.Map<CategoryResponseDto>(c)).ToList();
            return new GenericResponseDto<CategoryResponseDto>(true, ValidationMessages.CATEGORIES_RETRIEVED_SUCCESSFULLY, categoryDtos);
        
        }

        public async Task<GenericResponseDto<string>> UpdateCategory(int id, CategoryRequestDto request)
        {
            var categoryToUpdate = await _categoryRepository.GetByCondition(c => c.Id == id);
            if (categoryToUpdate == null)
            {
                return new GenericResponseDto<string>(false, ValidationMessages.CATEGORY_NOT_FOUND);
            }

            _mapper.Map(request, categoryToUpdate);

            await _categoryRepository.Update(categoryToUpdate);

            return new GenericResponseDto<string>(true, ValidationMessages.CATEGORY_UPDATED_SUCCESSFULLY);

        }
    }
}
