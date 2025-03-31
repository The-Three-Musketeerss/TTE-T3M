using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTE.Application.DTOs;
using TTE.Infrastructure.Models;

namespace TTE.Application.Interfaces
{
    public interface IJobService
    {
        Task<GenericResponseDto<List<JobResponseDto>>> GetPendingJobs();
        Task<GenericResponseDto<string>> ReviewJob(int jobId, JobReviewRequestDto request);

    }
}
