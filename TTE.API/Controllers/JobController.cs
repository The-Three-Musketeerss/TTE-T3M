using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TTE.Application.Interfaces;
using TTE.Commons.Constants;
using TTE.Application.DTOs;
using TTE.Application.Services;

namespace TTE.API.Controllers
{
    [ApiController]
    [Route("api/jobs")]
    public class JobController : ControllerBase
    {
        private readonly IJobService _jobService;

        public JobController(IJobService jobService)
        {
            _jobService = jobService;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetPendingJobs()
        {
            var result = await _jobService.GetPendingJobs();
            return Ok(result);
        }



        [HttpPost("/api/reviewJob/{jobId}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> ReviewJob(int jobId, [FromBody] JobReviewRequestDto request)
        {
            var result = await _jobService.ReviewJob(jobId, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
