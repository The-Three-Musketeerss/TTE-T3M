using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTE.Application.DTOs;
using TTE.Application.Interfaces;
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

            return new GenericResponseDto<List<JobResponseDto>>(true,"Jobs pending", result);
        }
    }
}
