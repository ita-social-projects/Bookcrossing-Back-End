using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Dto;
using Application.Dto.QueryParams;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.RDBMS;
using Domain.RDBMS.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementation
{
    public class LocationService : Interfaces.ILocationService
    {
        private readonly IRepository<Location> _locationRepository;
        private readonly IMapper _mapper;
        private readonly IPaginationService _paginationService;
        public LocationService(IRepository<Location> locationRepository, IMapper mapper, IPaginationService paginationService)
        {
            _locationRepository = locationRepository;
            _paginationService = paginationService;
            _mapper = mapper;
        }

        public async Task<LocationDto> GetById(int locationId)
        {
            return _mapper.Map<LocationDto>(await _locationRepository.GetAll().Include(p => p.UserRoom).FirstOrDefaultAsync(p => p.Id == locationId));
        }

        public async Task<List<LocationDto>> GetAll()
        {
            return _mapper.Map<List<LocationDto>>(await _locationRepository.GetAll()
                                                                   .Include(p => p.UserRoom)
                                                                   .OrderBy(x => x.City)
                                                                   .ThenBy(x => x.OfficeName)
                                                                   .ToListAsync());
        }

        public async Task<PaginationDto<LocationDto>> GetAll(FullPaginationQueryParams fullPaginationQuery)
        {
            var query = _locationRepository.GetAll().IgnoreQueryFilters();
            return await _paginationService.GetPageAsync<LocationDto, Location>(query, fullPaginationQuery);
        }

        public async Task Update(LocationDto location)
        {
            var newLocation = _mapper.Map<Location>(location);
            _locationRepository.Update(newLocation);
            await _locationRepository.SaveChangesAsync();
        }

        public async Task<LocationDto> Remove(int locationId)
        {
            var location = await _locationRepository.FindByIdAsync(locationId);
            if (location == null)
                return null;
            _locationRepository.Remove(location);
            await _locationRepository.SaveChangesAsync();
            return _mapper.Map<LocationDto>(location);
        }

        public async Task<int> Add(LocationDto location)
        {
            var newLocation = _mapper.Map<Location>(location);
            _locationRepository.Add(newLocation);
            await _locationRepository.SaveChangesAsync();
            return newLocation.Id;
        }
    }
}
