using System.Linq.Expressions;
using Moq;
using TTE.Application.DTOs;
using TTE.Application.Handlers;
using TTE.Application.Services;
using TTE.Commons.Constants;
using TTE.Infrastructure.Models;
using TTE.Infrastructure.Repositories;

namespace TTE.Tests.Services
{
    public class JobServiceTests
    {
        private readonly Mock<IGenericRepository<Job>> _mockJobRepo = new();
        private readonly Mock<IGenericRepository<Product>> _mockProductRepo = new();
        private readonly Mock<IGenericRepository<Category>> _mockCategoryRepo = new();
        private readonly Mock<IProductJobHandler> _mockProductHandler = new();
        private readonly Mock<ICategoryJobHandler> _mockCategoryHandler = new();

        private readonly JobService _service;

        public JobServiceTests()
        {
            _service = new JobService(
                _mockJobRepo.Object,
                _mockProductRepo.Object,
                _mockCategoryRepo.Object,
                _mockCategoryHandler.Object,
                _mockProductHandler.Object
            );
        }

        [Fact]
        public async Task GetPendingJobs_ShouldReturnPendingJobs()
        {
            // Arrange
            var jobs = new List<Job>
            {
                new Job { Id = 1, Item_id = 101, Status = Job.StatusEnum.Pending, Type = Job.JobEnum.Product, Operation = Job.OperationEnum.Create },
                new Job { Id = 2, Item_id = 102, Status = Job.StatusEnum.Pending, Type = Job.JobEnum.Category, Operation = Job.OperationEnum.Delete }
            };

            _mockJobRepo.Setup(r => r.GetAllByCondition(It.IsAny<Expression<Func<Job, bool>>>()))
                        .ReturnsAsync(jobs);

            // Act
            var result = await _service.GetPendingJobs();
            var data = result.Data as List<JobResponseDto>;

            // Assert
            Assert.True(result.Success);
            Assert.Equal(2, data.Count);
            Assert.Equal(ValidationMessages.MESSAGE_JOBS_PENDING, result.Message);
        }

        [Fact]
        public async Task ReviewJob_ShouldReturnError_IfJobNotFound()
        {
            // Arrange
            _mockJobRepo.Setup(r => r.GetByCondition(It.IsAny<Expression<Func<Job, bool>>>()))
                        .ReturnsAsync((Job)null);

            var request = new JobReviewRequestDto { Action = AppConstants.APPROVE };

            // Act
            var result = await _service.ReviewJob(1, request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(ValidationMessages.MESSAGE_JOB_NOT_FOUND, result.Message);
        }

        [Fact]
        public async Task ReviewJob_ShouldReturnError_IfAlreadyReviewed()
        {
            var job = new Job { Id = 1, Status = Job.StatusEnum.Approved };

            _mockJobRepo.Setup(r => r.GetByCondition(It.IsAny<Expression<Func<Job, bool>>>()))
                        .ReturnsAsync(job);

            var request = new JobReviewRequestDto { Action = AppConstants.APPROVE };

            var result = await _service.ReviewJob(1, request);

            Assert.False(result.Success);
            Assert.Equal(ValidationMessages.MESSAGE_JOB_ALREADY_REVIEWED, result.Message);
        }

        [Fact]
        public async Task ReviewJob_ShouldDelegateToProductHandler_WhenTypeIsProduct()
        {
            var job = new Job { Id = 1, Item_id = 101, Status = Job.StatusEnum.Pending, Type = Job.JobEnum.Product };
            var product = new Product { Id = 101 };

            var request = new JobReviewRequestDto { Action = AppConstants.APPROVE };

            _mockJobRepo.Setup(r => r.GetByCondition(It.IsAny<Expression<Func<Job, bool>>>()))
                        .ReturnsAsync(job);

            _mockProductRepo.Setup(r => r.GetByCondition(It.IsAny<Expression<Func<Product, bool>>>()))
                            .ReturnsAsync(product);

            _mockProductHandler.Setup(h => h.Handle(job, product, AppConstants.APPROVE))
                               .ReturnsAsync(new GenericResponseDto<string>(true, "Approved"));

            var result = await _service.ReviewJob(job.Id, request);

            Assert.True(result.Success);
            Assert.Equal("Approved", result.Message);
        }

        [Fact]
        public async Task ReviewJob_ShouldDelegateToCategoryHandler_WhenTypeIsCategory()
        {
            var job = new Job { Id = 2, Item_id = 202, Status = Job.StatusEnum.Pending, Type = Job.JobEnum.Category };
            var category = new Category { Id = 202 };

            var request = new JobReviewRequestDto { Action = AppConstants.DECLINE };

            _mockJobRepo.Setup(r => r.GetByCondition(It.IsAny<Expression<Func<Job, bool>>>()))
                        .ReturnsAsync(job);

            _mockCategoryRepo.Setup(r => r.GetByCondition(It.IsAny<Expression<Func<Category, bool>>>()))
                             .ReturnsAsync(category);

            _mockCategoryHandler.Setup(h => h.Handle(job, category, AppConstants.DECLINE))
                                .ReturnsAsync(new GenericResponseDto<string>(true, "Declined"));

            var result = await _service.ReviewJob(job.Id, request);

            Assert.True(result.Success);
            Assert.Equal("Declined", result.Message);
        }

        [Fact]
        public async Task ReviewJob_ShouldReturnError_WhenJobTypeUnsupported()
        {
            var job = new Job { Id = 3, Item_id = 999, Status = Job.StatusEnum.Pending, Type = (Job.JobEnum)99 };
            var request = new JobReviewRequestDto { Action = AppConstants.APPROVE };

            _mockJobRepo.Setup(r => r.GetByCondition(It.IsAny<Expression<Func<Job, bool>>>()))
                        .ReturnsAsync(job);

            var result = await _service.ReviewJob(job.Id, request);

            Assert.False(result.Success);
            Assert.Equal(ValidationMessages.UNSUPPORTED_JOB, result.Message);
        }
    }
}
