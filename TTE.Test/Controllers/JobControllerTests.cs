using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TTE.API.Controllers;
using TTE.Application.DTOs;
using TTE.Application.Interfaces;
using TTE.Commons.Constants;

namespace TTE.Tests.Controllers
{
    public class JobControllerTests
    {

        private readonly Mock<IJobService> _mockJobService;
        private readonly JobController _controller;

        public JobControllerTests()
        {
            _mockJobService = new Mock<IJobService>();
            _controller = new JobController(_mockJobService.Object);
        }

        [Fact]
        public async Task GetPendingJobs_ShouldReturnOk_WithPendingJobs()
        {
            // Arrange
            var jobs = new List<JobResponseDto>
            {
                new JobResponseDto { Id = 100, Type = "product", Operation = "delete" },
                new JobResponseDto { Id = 101, Type = "category", Operation = "create" }
            };

            var response = new GenericResponseDto<List<JobResponseDto>>(true, "Jobs pending", jobs);

            _mockJobService.Setup(s => s.GetPendingJobs()).ReturnsAsync(response);

            // Act
            var result = await _controller.GetPendingJobs() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(response, result.Value);
        }

        [Fact]
        public async Task ReviewJob_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            int jobId = 1;
            var request = new JobReviewRequestDto { Action = AppConstants.APPROVE };

            var response = new GenericResponseDto<string>(true, "Job approved successfully.");

            _mockJobService.Setup(s => s.ReviewJob(jobId, request)).ReturnsAsync(response);

            // Act
            var result = await _controller.ReviewJob(jobId, request) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(response, result.Value);
        }

        [Fact]
        public async Task ReviewJob_ShouldReturnBadRequest_WhenFails()
        {
            // Arrange
            int jobId = 1;
            var request = new JobReviewRequestDto { Action = "invalid" };

            var response = new GenericResponseDto<string>(false, "Invalid action");

            _mockJobService.Setup(s => s.ReviewJob(jobId, request)).ReturnsAsync(response);

            // Act
            var result = await _controller.ReviewJob(jobId, request) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal(response, result.Value);
        }
    }
}
