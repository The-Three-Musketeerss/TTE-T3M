using AutoMapper;
using TTE.Application.Interfaces;
using TTE.Infrastructure.DTOs;
using TTE.Infrastructure.Repositories;

namespace TTE.Application.Services
{
    public class GenericService<T, T2> : IGenericService<T,T2>
    {
        private readonly IGenericRepository<T> _repository;
        private readonly IMapper _mapper;

        public GenericService(IGenericRepository<T> genericRepository, IMapper mapper) { 
            _repository = genericRepository;
            _mapper = mapper;
        }

        public async Task<GenericResponseDto<T2>> GetAll()
        {
            var entities = await _repository.Get();
            var responseDtos = entities.Select(x => _mapper.Map<T2>(x)).ToList();

            return new GenericResponseDto<T2>(true, "Data retrieved successfully", responseDtos);
        }
    }
}
