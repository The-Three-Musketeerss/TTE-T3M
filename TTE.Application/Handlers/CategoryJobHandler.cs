using System;
using System.Threading.Tasks;
using TTE.Application.DTOs;
using TTE.Commons.Constants;
using TTE.Infrastructure.Models;
using TTE.Infrastructure.Repositories;

namespace TTE.Application.Handlers
{
    public class CategoryJobHandler : ICategoryJobHandler
    {
        private readonly IGenericRepository<Category> _categoryRepo;
        private readonly IGenericRepository<Job> _jobRepo;

        public CategoryJobHandler(IGenericRepository<Category> categoryRepo, IGenericRepository<Job> jobRepo)
        {
            _categoryRepo = categoryRepo;
            _jobRepo = jobRepo;
        }

        public async Task<GenericResponseDto<string>> Handle(Job job, Category category, string action)
        {
            if (category == null)
                return new GenericResponseDto<string>(false, ValidationMessages.CATEGORY_NOT_FOUND);

            switch (action.ToLower())
            {
                case AppConstants.APPROVE:
                    job.Status = Job.StatusEnum.Approved;

                    if (job.Operation == Job.OperationEnum.Create)
                    {
                        category.Approved = true;
                        await _categoryRepo.Update(category);
                    }

                    if (job.Operation == Job.OperationEnum.Delete)
                    {
                        await _categoryRepo.Delete(category);
                    }
                    break;

                case AppConstants.DECLINE:
                    job.Status = Job.StatusEnum.Declined;
                    break;

                default:
                    return new GenericResponseDto<string>(false, ValidationMessages.MESSAGE_JOB_INVALID_ACTION);
            }

            await _jobRepo.Update(job);

            return new GenericResponseDto<string>(true, string.Format(ValidationMessages.MESSAGE_JOB_REVIEW_SUCCESS, action));
        }
    }
}
