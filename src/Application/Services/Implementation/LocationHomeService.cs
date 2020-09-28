using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Dto;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.RDBMS;
using Domain.RDBMS.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementation
{
    public class LocationHomeService : ILocationHomeService
    {
        private readonly IRepository<LocationHome> _locationHomeRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Book> _bookRepository;
        private readonly IMapper _mapper;

        public LocationHomeService(
            IRepository<LocationHome> locationHomeRepository, 
            IMapper mapper,
            IRepository<User> userRepository,
            IRepository<Book> bookRepository)
        {
            _locationHomeRepository = locationHomeRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _bookRepository = bookRepository;
        }

        public async Task<IEnumerable<LocationHomeDto>> GetAll()
        {
            return  _mapper.Map<IEnumerable<LocationHomeDto>>(await _locationHomeRepository
                                                                    .GetAll()
                                                                    .OrderBy(x => x.City)
                                                                    .ToArrayAsync()
                                                                    );
        }

        public async Task<LocationHomeDto> GetById(int Id)
        {
            return _mapper.Map<LocationHomeDto>(await _locationHomeRepository.FindByIdAsync(Id));
        }

        public async Task Update(LocationHomeDto locationHomeDto)
        {
            var location = _mapper.Map<LocationHome>(locationHomeDto);
            _locationHomeRepository.Update(location);
            await _locationHomeRepository.SaveChangesAsync();
        }

        public async Task<LocationHomeDto> Remove(int Id)
        {
            var location = await _locationHomeRepository.FindByIdAsync(Id);
            if (location == null)
            {
                return null;
            }

            _locationHomeRepository.Remove(location);
            await _locationHomeRepository.SaveChangesAsync();

            return _mapper.Map<LocationHomeDto>(location);
        }

        public async Task<int> Add(LocationHomePostDto locationHomeDto)
        {
            var userId = locationHomeDto.UserId;

            var location = _mapper.Map<LocationHome>(locationHomeDto);
            _locationHomeRepository.Add(location);

            var user = await _userRepository.FindByIdAsync(userId);
            if(user == null)
            {
                return 0;
            }

            await _locationHomeRepository.SaveChangesAsync();
            user.LocationHomeId = location.Id;
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            return location.Id;
        }

        public IEnumerable<MapHomeLocationDto> GetBooksQuantityOnHomeLocations()
        {
            return _bookRepository.GetAll()
                .Include(b => b.User)
                .ThenInclude(r => r.LocationHome)
                .Where(b => b.User.LocationHomeId.HasValue)
                .ToList()
                .GroupBy(b => b.User.LocationHome)
                .Select(l => new MapHomeLocationDto(_mapper.Map<LocationHomeDto>(l.Key), l.Count()));
        }
    }
}
