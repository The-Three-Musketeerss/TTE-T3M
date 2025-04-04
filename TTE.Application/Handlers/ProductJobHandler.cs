using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.Runtime.Internal;
using TTE.Application.DTOs;
using TTE.Commons.Constants;
using TTE.Infrastructure.Models;
using TTE.Infrastructure.Repositories;

namespace TTE.Application.Handlers
{
    public class ProductJobHandler : IProductJobHandler
    {

        private readonly IGenericRepository<Product> _productRepo;
        private readonly IGenericRepository<Job> _jobRepo;


        public ProductJobHandler(IGenericRepository<Product> productRepo, IGenericRepository<Job> jobRepo)
        {
            _jobRepo = jobRepo;
            _productRepo = productRepo;
        }

        public async Task<GenericResponseDto<string>> Handle(Job job, Product product, string action)
        {
            if (product == null)
                return new GenericResponseDto<string>(false,ValidationMessages.MESSAGE_PRODUCT_NOT_FOUND);

            switch (action.ToLower())
            {
                case AppConstants.APPROVE:
                    job.Status = Job.StatusEnum.Approved;
                    if (job.Operation == Job.OperationEnum.Create)
                        product.Approved = true;

                    if (job.Operation == Job.OperationEnum.Delete)
                        await _productRepo.Delete(product);
                    break;

                case AppConstants.DECLINE:
                    job.Status = Job.StatusEnum.Declined;
                    break;

                default:
                    return new GenericResponseDto<string>(false, ValidationMessages.MESSAGE_JOB_INVALID_ACTION);
            }

            job.Status = action == AppConstants.APPROVE ? Job.StatusEnum.Approved : Job.StatusEnum.Declined;
            
            await _jobRepo.Update(job);
            await _productRepo.Update(product);
            return new GenericResponseDto<string>(true, string.Format(ValidationMessages.MESSAGE_JOB_REVIEW_SUCCESS,action));
        }
    }
}
