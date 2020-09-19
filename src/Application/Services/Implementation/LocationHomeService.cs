using Application.Services.Interfaces;
using AutoMapper;
using Domain.RDBMS;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Dto;
using Application.Dto.QueryParams;
using Domain.RDBMS.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementation
{
    public class LocationHomeService : Interfaces.ILocationHomeService
    {
        private readonly IRepository<LocationHome> _locationHomeRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IMapper _mapper;
        private readonly IPaginationService _paginationService;
        public LocationHomeService(IRepository<LocationHome> locationHomeRepository, IMapper mapper, IPaginationService paginationService,
            IRepository<User> userRepository)
        {
            _locationHomeRepository = locationHomeRepository;
            _userRepository = userRepository;
            _paginationService = paginationService;
            _mapper = mapper;
        }

        public async Task<LocationHomeDto> GetById(int locationId)
        {
            return _mapper.Map<LocationHomeDto>(await _locationHomeRepository.GetAll().Include(p => p.User).FirstOrDefaultAsync(p => p.Id == locationId));
        }

        public async Task<List<LocationHomeDto>> GetAll()
        {
            return _mapper.Map<List<LocationHomeDto>>(await _locationHomeRepository.GetAll()
                                                                   .Include(p => p.User)
                                                                   .OrderBy(x => x.City)
                                                                   .ToListAsync());
        }

        public async Task Update(LocationHomeDto locationHomeDto)
        {
            var location = _mapper.Map<LocationHome>(locationHomeDto);
            _locationHomeRepository.Update(location);
            await _locationHomeRepository.SaveChangesAsync();
        }

        public async Task<LocationHomeDto> Remove(int locationId)
        {
            var location = await _locationHomeRepository.FindByIdAsync(locationId);
            if (location == null)
            {
                return null;
            }
            _locationHomeRepository.Remove(location);
            await _locationHomeRepository.SaveChangesAsync();
            return _mapper.Map<LocationHomeDto>(location);
        }

        public async Task<int> Add(LocationHomeDto locationHomeDto)
        {
            var location = _mapper.Map<LocationHome>(locationHomeDto);
            _locationHomeRepository.Add(location);
            await _locationHomeRepository.SaveChangesAsync();
            var user = await _userRepository.FindByIdAsync(location.UserId);
            user.LocationHomeId = location.Id;
            _userRepository.Update(user);
            await _locationHomeRepository.SaveChangesAsync();
            return location.Id;
        }
        public async Task<PaginationDto<LocationHomeDto>> GetAll(FullPaginationQueryParams parameters)
        {
            var query = _locationHomeRepository.GetAll().IgnoreQueryFilters();
            return await _paginationService.GetPageAsync<LocationHomeDto, LocationHome>(query, parameters);
        }
    }
}
