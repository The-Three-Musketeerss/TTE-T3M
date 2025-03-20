using System.Linq;
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
        private readonly IRatingRepository _ratingRepository;
        private readonly IGenericRepository<Category> _categoryRepository;
        private readonly IGenericRepository<Product> _genericProductRepository;
        private readonly IGenericRepository<Inventory> _inventoryRepository;
        private readonly IGenericRepository<Job> _jobRepository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, IRatingRepository ratingRepository, IMapper mapper, IGenericRepository<Category> categoryRepository )
        {
            _productRepository = productRepository;
            _ratingRepository = ratingRepository;
            _categoryRepository = categoryRepository; 
            _mapper = mapper;
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
            var category = await _categoryRepository.GetByCondition(c => c.Name == request.Category);
            if(category == null)
            {
                return new GenericResponseDto<ProductCreatedResponseDto>(false, "category not found");
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

            await _inventoryRepository.Add(inventory);

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
                await _jobRepository.Add(job);
            }

            var response = new ProductCreatedResponseDto
            {
                Id = product.Id,
                Message = "Product created successfully."
            };

            return new GenericResponseDto<ProductCreatedResponseDto>(true,"Created", response);
        }

    }
}
