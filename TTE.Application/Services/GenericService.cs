using AutoMapper;
using TTE.Application.Interfaces;
using TTE.Commons.Constants;
using TTE.Infrastructure.DTOs;
using TTE.Infrastructure.Repositories;

namespace TTE.Application.Services
{
    public class GenericService<T, T2, T3> : IGenericService<T, T2, T3>
    {
        private readonly IGenericRepository<T> _repository;
        private readonly IMapper _mapper;

        public GenericService(IGenericRepository<T> genericRepository, IMapper mapper)
        {
            _repository = genericRepository;
            _mapper = mapper;
        }

        public async Task<GenericResponseDto<T2>> GetAll(params string[] includes)
        {
            var entities = await _repository.GetEntityWithIncludes(includes);
            var responseDtos = entities.Select(x => _mapper.Map<T2>(x)).ToList();

            return new GenericResponseDto<T2>(true, "Data retrieved successfully", responseDtos);
        }

        public async Task<GenericResponseDto<string>> Update(T3 request)
        {
            var entity = _mapper.Map<T>(request);
            if (entity == null)
            {
                return new GenericResponseDto<string>(false, "Failed to update");
            }


            await _repository.Update(entity);

            return new GenericResponseDto<string>(true, "Updated successfully");
        }
    }
}
