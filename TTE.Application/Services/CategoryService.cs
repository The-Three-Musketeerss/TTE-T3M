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
        private readonly IGenericRepository<Product> _genericProductRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;


        public CategoryService(IGenericRepository<Category> categoryRepository, IGenericRepository<Job> jobRepository, IGenericRepository<Product> genericProductRepository, IProductRepository productRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _jobRepository = jobRepository;
            _genericProductRepository = genericProductRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<GenericResponseDto<string>> DeleteCategory(int id, string userRole, string userName)
        {
            var categoryToDelete = await _categoryRepository.GetByCondition(c => c.Id == id);

            var productsWithCategory = await _genericProductRepository.GetAllByCondition(p => p.CategoryId == id);
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
                ItemName = categoryToDelete.Name,
                Operation = Job.OperationEnum.Delete,
                CreatedBy = userName,
                Status = Job.StatusEnum.Pending
            };

            await _jobRepository.Add(job);

            return new GenericResponseDto<string>(true, ValidationMessages.CATEGORY_DELETED_EMPLOYEE_SUCCESSFULLY);
        }

        public async Task<GenericResponseDto<CategoryResponseDto>> CreateCategory(CategoryRequestDto request, string userRole, string userName)
        {
            var categoryExists = await _categoryRepository.GetByCondition(c => c.Name == request.Name);
            if (categoryExists != null)
            {
                return new GenericResponseDto<CategoryResponseDto>(false, ValidationMessages.CATEGORY_ALREADY_EXISTS);
            }

            request.Id = null;

            var category = _mapper.Map<Category>(request);

            if (userRole == AppConstants.ADMIN)
            {
                category.Approved = true;
                await _categoryRepository.Add(category);
                var response = _mapper.Map<CategoryResponseDto>(category);
                return new GenericResponseDto<CategoryResponseDto>(true, ValidationMessages.CATEGORY_CREATED_SUCCESSFULLY, response);
            }

            category.Approved = false;
            await _categoryRepository.Add(category);

            var job = new Job
            {
                Item_id = category.Id,
                CreatedAt = DateTime.Now,
                Type = Job.JobEnum.Category,
                ItemName = category.Name,
                CreatedBy = userName,
                Operation = Job.OperationEnum.Create,
                Status = Job.StatusEnum.Pending
            };
            await _jobRepository.Add(job);

            var responseDto = _mapper.Map<CategoryResponseDto>(category);
            return new GenericResponseDto<CategoryResponseDto>(true, ValidationMessages.CATEGORY_CREATED_EMPLOYEE_SUCCESSFULLY, responseDto);
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

        public async Task<GenericResponseDto<string>> GetTopCategoryNamesByProductCount(int top = 3)
        {
            var names = await _productRepository.GetTopCategoryNamesByProductCount(top);
            return new GenericResponseDto<string>(true, ValidationMessages.CATEGORIES_RETRIEVED_SUCCESSFULLY, names);
        }
    }
}
