using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Moq;
using TTE.Application.DTOs;
using TTE.Application.Services;
using TTE.Commons.Constants;
using TTE.Infrastructure.Models;
using TTE.Infrastructure.Repositories;

namespace TTE.Tests.Services
{
    public class JobServiceTests
    {

        private readonly Mock<IGenericRepository<Job>> _mockJobRepo;
        private readonly JobService _jobService;

        public JobServiceTests()
        {
            _mockJobRepo = new Mock<IGenericRepository<Job>>();
            _jobService = new JobService(_mockJobRepo.Object);
        }

        [Fact]
        public async Task GetPendingJobs_ShouldReturnPendingJobs()
        {
            // Arrange
            var jobs = new List<Job>
            {
                new Job { Id = 1, Item_id = 100, Status = Job.StatusEnum.Pending, Type = Job.JobEnum.Product, Operation = Job.OperationEnum.Delete },
                new Job { Id = 2, Item_id = 101, Status = Job.StatusEnum.Pending, Type = Job.JobEnum.Category, Operation = Job.OperationEnum.Create }
            };

            _mockJobRepo.Setup(r => r.GetAllByCondition(It.IsAny<Expression<Func<Job, bool>>>()))
                        .ReturnsAsync(jobs);

            // Act
            var result = await _jobService.GetPendingJobs();
            var data = result.Data as List<JobResponseDto>;

            // Assert
            Assert.True(result.Success);
            Assert.Equal(ValidationMessages.MESSAGE_JOBS_PENDING, result.Message);
            Assert.Equal(2, data.Count);
            Assert.Contains(data, j => j.Type == "product");
            Assert.Contains(data, j => j.Type == "category");
        }

        [Fact]
        public async Task ReviewJob_ShouldReturnSuccess_WhenApproved()
        {
            // Arrange
            var job = new Job { Id = 1, Status = Job.StatusEnum.Pending, Item_id = 100 };

            _mockJobRepo.Setup(r => r.GetByCondition(It.IsAny<Expression<Func<Job, bool>>>()))
                        .ReturnsAsync(job);

            _mockJobRepo.Setup(r => r.Update(It.IsAny<Job>()))
                        .Returns(Task.CompletedTask);

            var request = new JobReviewRequestDto { Action = AppConstants.APPROVE };

            // Act
            var result = await _jobService.ReviewJob(1, request);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Job approved successfully.", result.Message);
        }

        [Fact]
        public async Task ReviewJob_ShouldReturnSuccess_WhenDeclined()
        {
            // Arrange
            var job = new Job { Id = 1, Status = Job.StatusEnum.Pending, Item_id = 200 };

            _mockJobRepo.Setup(r => r.GetByCondition(It.IsAny<Expression<Func<Job, bool>>>()))
                        .ReturnsAsync(job);

            _mockJobRepo.Setup(r => r.Update(It.IsAny<Job>()))
                        .Returns(Task.CompletedTask);

            var request = new JobReviewRequestDto { Action = AppConstants.DECLINE };

            // Act
            var result = await _jobService.ReviewJob(1, request);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Job declined successfully.", result.Message);
        }

        [Fact]
        public async Task ReviewJob_ShouldReturnError_WhenJobNotFound()
        {
            _mockJobRepo.Setup(r => r.GetByCondition(It.IsAny<Expression<Func<Job, bool>>>()))
                        .ReturnsAsync((Job)null);

            var request = new JobReviewRequestDto { Action = AppConstants.APPROVE };

            var result = await _jobService.ReviewJob(1, request);

            Assert.False(result.Success);
            Assert.Equal(ValidationMessages.MESSAGE_JOB_NOT_FOUND, result.Message);
        }

        [Fact]
        public async Task ReviewJob_ShouldReturnError_WhenAlreadyReviewed()
        {
            var job = new Job { Id = 1, Status = Job.StatusEnum.Approved };

            _mockJobRepo.Setup(r => r.GetByCondition(It.IsAny<Expression<Func<Job, bool>>>()))
                        .ReturnsAsync(job);

            var request = new JobReviewRequestDto { Action = AppConstants.APPROVE };

            var result = await _jobService.ReviewJob(1, request);

            Assert.False(result.Success);
            Assert.Equal(ValidationMessages.MESSAGE_JOB_ALREADY_REVIEWED, result.Message);
        }

        [Fact]
        public async Task ReviewJob_ShouldReturnError_WhenActionIsInvalid()
        {
            var job = new Job { Id = 1, Status = Job.StatusEnum.Pending };

            _mockJobRepo.Setup(r => r.GetByCondition(It.IsAny<Expression<Func<Job, bool>>>()))
                        .ReturnsAsync(job);

            var request = new JobReviewRequestDto { Action = "invalid_action" };

            var result = await _jobService.ReviewJob(1, request);

            Assert.False(result.Success);
            Assert.Equal(ValidationMessages.MESSAGE_JOB_INVALID_ACTION, result.Message);
        }
    }
}
