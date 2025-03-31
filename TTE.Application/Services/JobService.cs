using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTE.Application.DTOs;
using TTE.Application.Interfaces;
using TTE.Commons.Constants;
using TTE.Infrastructure.Models;
using TTE.Infrastructure.Repositories;

namespace TTE.Application.Services
{
    public class JobService : IJobService
    {
        private readonly IGenericRepository<Job> _jobRepository;

        public JobService(IGenericRepository<Job> jobRepository)
        {
            _jobRepository = jobRepository;
        }
        public async Task<GenericResponseDto<List<JobResponseDto>>> GetPendingJobs()
        {
            var jobs = await _jobRepository.GetAllByCondition(j => j.Status == Job.StatusEnum.Pending);

            var result = jobs.Select(j => new JobResponseDto
            {
                Type = j.Type.ToString().ToLower(),       
                Id = j.Item_id,                           
                Operation = j.Operation.ToString().ToLower()
            }).ToList();

            return new GenericResponseDto<List<JobResponseDto>>(true,ValidationMessages.MESSAGE_JOBS_PENDING, result);
        }

        public async Task<GenericResponseDto<string>> ReviewJob(int jobId, JobReviewRequestDto request)
        {
            var job = await _jobRepository.GetByCondition(j => j.Id == jobId);
            if (job == null)
                return new GenericResponseDto<string>(false, ValidationMessages.MESSAGE_JOB_NOT_FOUND);

            if (job.Status != Job.StatusEnum.Pending)
                return new GenericResponseDto<string>(false, ValidationMessages.MESSAGE_JOB_ALREADY_REVIEWED);

            switch (request.Action.ToLower())
            {
                case AppConstants.APPROVE:
                    job.Status = Job.StatusEnum.Approved;
                    break;
                case AppConstants.DECLINE:
                    job.Status = Job.StatusEnum.Declined;
                    break;
                default:
                    return new GenericResponseDto<string>(false,ValidationMessages.MESSAGE_JOB_INVALID_ACTION);
            }

            await _jobRepository.Update(job);

            return new GenericResponseDto<string>(true, string.Format(ValidationMessages.MESSAGE_JOB_REVIEW_SUCCESS, request.Action.ToLower()));
        }
    }
}
