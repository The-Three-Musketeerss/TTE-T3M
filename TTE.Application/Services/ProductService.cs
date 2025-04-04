using AutoMapper;
using TTE.Application.DTOs;
using TTE.Application.Interfaces;
using TTE.Commons.Constants;
using TTE.Infrastructure.Models;
using TTE.Infrastructure.Repositories;

namespace TTE.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IGenericRepository<Product> _genericProductRepository;
        private readonly IGenericRepository<Category> _genericCategoryRepository;
        private readonly IRatingRepository _ratingRepository;
        private readonly IGenericRepository<Inventory> _genericInventoryRepository;
        private readonly IGenericRepository<Job> _genericJobRepository;
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Rating> _genericRatingRepository;

        public ProductService(IProductRepository productRepository, 
            IGenericRepository<Product> genericProductRepository, 
            IGenericRepository<Category> genericCategoryRepository, 
            IRatingRepository ratingRepository, 
            IGenericRepository<Inventory> genericInventoryRepository,
            IGenericRepository<Job> genericJobRepository,
            IGenericRepository<Rating> genericRatingRepository,
            IMapper mapper)
        {
            _productRepository = productRepository;
            _ratingRepository = ratingRepository;
            _genericProductRepository = genericProductRepository;
            _genericCategoryRepository = genericCategoryRepository;
            _genericJobRepository = genericJobRepository;
            _genericInventoryRepository = genericInventoryRepository;
            _genericRatingRepository = genericRatingRepository;
            _mapper = mapper;
        }

        public async Task<GenericResponseDto<string>> UpdateProduct(int productId, ProductUpdateRequestDto request)
        {
            var includes = new string[1] { "Inventory" };
            var products = await _genericProductRepository.GetEntityWithIncludes(includes);
            var product = products.FirstOrDefault(p => p.Id == productId);
            if (product == null)
            {
                return new GenericResponseDto<string>(false, ValidationMessages.MESSAGE_PRODUCT_NOT_FOUND);
            }

            if (request.Inventory != null && request.Inventory.Available > request.Inventory.Total)
            {
                return new GenericResponseDto<string>(
                    false, "Available inventory cannot be greater than total inventory.");
            }

            if (request.Category != null)
            {
                var category = await _genericCategoryRepository.GetByCondition(x => x.Name == request.Category);
                if (category == null)
                {
                    return new GenericResponseDto<string>(false, ValidationMessages.CATEGORY_NOT_FOUND);
                }
                product.CategoryId = category.Id;
            }

            if (request.Price == null)
            {
                request.Price = product.Price;
            }

            _mapper.Map(request, product);

            if (product.Inventory == null)
            {
                product.Inventory = _mapper.Map<Inventory>(request.Inventory);
                product.Inventory.ProductId = product.Id;
            }
            else
            {
                _mapper.Map(request.Inventory, product.Inventory);
            }

            await _genericProductRepository.Update(product);
            return new GenericResponseDto<string>(true, ValidationMessages.MESSAGE_PRODUCT_UPDATED_SUCCESSFULLY);
        }

        public async Task<ProductPaginatedResponseDto> GetProducts(
            string? category, string? orderBy, bool descending, int page, int pageSize)
        {
            var (products, totalCount) = await _productRepository.GetProducts(category, orderBy, descending, page, pageSize);

            var productIds = products.Select(p => p.Id).ToList();

            var ratings = await _ratingRepository.GetRatingsByProductIds(productIds);


            var productDtos = products.Select(product =>
            {
                var productRatings = ratings.Where(r => r.ProductId == product.Id).ToList();

                var ratingDto = new RatingDto
                {
                    Rate = productRatings.Any() ? Math.Round(productRatings.Average(r => r.Rate), 1) : 0,
                    Count = productRatings.Count
                };

                var dto = _mapper.Map<ProductResponseDto>(product);
                dto.Rating = ratingDto;

                return dto;
            }).ToList();

            return new ProductPaginatedResponseDto(
                success: true,
                message: AuthenticationMessages.MESSAGE_PRODUCTS_RETRIEVED,
                data: productDtos,
                page: page,
                pageSize: pageSize,
                totalCount: totalCount
            );
        }


        public async Task<GenericResponseDto<ProductCreatedResponseDto>> CreateProducts(ProductRequestDto request, string userRole)
        {
            var category = await _genericCategoryRepository.GetByCondition(c => c.Name == request.Category);
            if (category == null)
            {
                return new GenericResponseDto<ProductCreatedResponseDto>(false, ValidationMessages.CATEGORY_NOT_FOUND);
            }

            var product = new Product
            {
                Title = request.Title,
                Price = request.Price,
                Description = request.Description,
                CategoryId = category.Id,
                Image = request.Image,
                Approved = userRole == AppConstants.ADMIN ? true : false
            };

            await _genericProductRepository.Add(product);

            var inventory = new Inventory
            {
                ProductId = product.Id,
                Total = request.Inventory.Total,
                Available = request.Inventory.Available
            };

            await _genericInventoryRepository.Add(inventory);

            if (userRole == AppConstants.EMPLOYEE)
            {
                var job = new Job
                {
                    Item_id = product.Id,
                    CreatedAt = DateTime.Now,
                    Type = Job.JobEnum.Product,
                    Operation = Job.OperationEnum.Create,
                    Status = Job.StatusEnum.Pending
                };
                await _genericJobRepository.Add(job);
            }

            var response = new ProductCreatedResponseDto
            {
                Id = product.Id,
                Title = product.Title
            };

            string state = product.Approved == false
                ? ValidationMessages.WAITING_FOR_APPROVAL
                : ValidationMessages.MESSAGE_PRODUCT_CREATED_SUCCESSFULLY;

            return new GenericResponseDto<ProductCreatedResponseDto>(true, state, response);
        }

        public async Task<GenericResponseDto<string>> DeleteProduct(int productId, string userRole)
        {
            var product = await _genericProductRepository.GetByCondition(p => p.Id == productId);
            if (product == null)
            {
                return new GenericResponseDto<string>(false, ValidationMessages.MESSAGE_PRODUCT_NOT_FOUND);
            }

            if (userRole == AppConstants.ADMIN)
            {
                await _genericProductRepository.Delete(productId);
                return new GenericResponseDto<string>(true, ValidationMessages.MESSAGE_PRODUCT_DELETED_SUCCESSFULLY);
            }

            var job = new Job
            {
                Item_id = productId,
                CreatedAt = DateTime.Now,
                Type = Job.JobEnum.Product,
                Operation = Job.OperationEnum.Delete,
                Status = Job.StatusEnum.Pending
            };
            await _genericJobRepository.Add(job);
            return new GenericResponseDto<string>(true, ValidationMessages.MESSAGE_PRODUCT_DELETED_EMPLOYEE_SUCCESSFULLY);

        }
        public async Task<GenericResponseDto<ProductByIdResponse>> GetProductById(int productId)
        {
            var includes = new string[] { "Category" };

            var product = await _genericProductRepository.GetByCondition(p => p.Id == productId,includes);
            if (product == null || product.Approved==false)
            {
                return new GenericResponseDto<ProductByIdResponse>(false, ValidationMessages.MESSAGE_PRODUCT_NOT_FOUND);
            }

            var category = await _genericCategoryRepository.GetByCondition(c => c.Id == product.CategoryId);

            var ratings = await _ratingRepository.GetRatingsByProductId(product.Id);
            var productRatings = ratings.ToList();

            var ratingDto = new RatingDto
            {
                Rate = productRatings.Any() ? Math.Round(productRatings.Average(r => r.Rate), 1) : 0,
                Count = productRatings.Count
            };

            var inventory = await _genericInventoryRepository.GetByCondition(i => i.ProductId == product.Id);
            var inventoryDto = inventory != null ? _mapper.Map<InventoryDto>(inventory) : new InventoryDto { Total = 0, Available = 0 };

            var productDto = _mapper.Map<ProductByIdResponse>(product);

            productDto.Rating = ratingDto;
            productDto.Inventory = inventoryDto;

            return new GenericResponseDto<ProductByIdResponse>(true,"", productDto);
        }
    }
}
