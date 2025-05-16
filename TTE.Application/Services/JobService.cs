using TTE.Application.DTOs;
using TTE.Application.Handlers;
using TTE.Application.Interfaces;
using TTE.Commons.Constants;
using TTE.Infrastructure.Models;
using TTE.Infrastructure.Repositories;

namespace TTE.Application.Services
{
    public class JobService : IJobService
    {
        private readonly IGenericRepository<Job> _jobRepository;
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IGenericRepository<Category> _categoryRepository;
        private readonly ICategoryJobHandler _categoryJobHandler;
        private readonly IProductJobHandler _productJobHandler;

        public JobService(IGenericRepository<Job> jobRepository, 
            IGenericRepository<Product> productRepository, 
            IGenericRepository<Category> categoryRepository,
            ICategoryJobHandler categoryJobHandler,
            IProductJobHandler productJobHandler)

        {
            _jobRepository = jobRepository;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _categoryJobHandler = categoryJobHandler;
            _productJobHandler = productJobHandler;
        }
        public async Task<GenericResponseDto<List<JobResponseDto>>> GetPendingJobs()
        {
            var jobs = await _jobRepository.GetAllByCondition(j => j.Status == Job.StatusEnum.Pending);

            var result = jobs.Select(j => new JobResponseDto
            {
                Id = j.Id,
                Type = j.Type.ToString().ToLower(),       
                Id_item = j.Item_id,
                ItemName = j.ItemName,
                Operation = j.Operation.ToString().ToLower(),
                CreatedBy = j.CreatedBy,
                CreatedAt = j.CreatedAt.ToString("dd-MM-yyyy HH:mm:ss"),

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

            var action = request.Action.ToLower();

            switch (job.Type)
            {
                case Job.JobEnum.Product:
                    var product = await _productRepository.GetByCondition(p => p.Id == job.Item_id);
                    return await _productJobHandler.Handle(job, product, action);

                case Job.JobEnum.Category:
                    var category = await _categoryRepository.GetByCondition(c => c.Id == job.Item_id);
                    return await _categoryJobHandler.Handle(job, category, action);

                default:
                    return new GenericResponseDto<string>(false, ValidationMessages.UNSUPPORTED_JOB);
            }
        }
    }
}
