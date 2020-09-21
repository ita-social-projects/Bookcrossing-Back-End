using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Dto;
using Application.Services.Implementation;
using AutoMapper;
using Domain.RDBMS;
using Domain.RDBMS.Entities;
using FluentAssertions;
using Infrastructure.RDBMS;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;

namespace ApplicationTest.Services
{
    [TestFixture]
    public class LocationHomeServiceTests
    {
        private BookCrossingContext _context;
        private LocationHomeService _locationService;
        private Mock<IRepository<LocationHome>> _locationRepositoryMock;
        private Mock<IRepository<User>> _usersRepositoryMock;
        private Mock<IMapper> _mapperMock;

        private User _user;
        private List<LocationHome> _locations;
        private Mock<IQueryable<LocationHome>> _locationsQueryableMock;
        private List<LocationHomeDto> _locationsDto;
        private LocationHome _location;
        private LocationHomeDto _locationDto;
        private LocationHomePostDto _locationPostDto;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _locationRepositoryMock = new Mock<IRepository<LocationHome>>();
            _usersRepositoryMock = new Mock<IRepository<User>>();
            _mapperMock = new Mock<IMapper>();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new Application.MapperProfilers.LocationHomeProfile());
                mc.AddProfile(new Application.MapperProfilers.UserProfile());
            });
            _user = new User()
            {
                Id = 0,
                AzureId = "",
                BirthDate = new System.DateTime(2000, 10, 10),
                Book = new List<Book>(),
                Email = "",
                FirstName = "User",
                IsDeleted = false,
                IsEmailAllowed = true,
                LastName = "Lastname",
                LocationHomeId = 1
            };
            var _mapper = mappingConfig.CreateMapper();
            var options = new DbContextOptionsBuilder<BookCrossingContext>().UseInMemoryDatabase(databaseName: "Fake DB").ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning)).Options;
            _context = new BookCrossingContext(options);
            _locationService = new LocationHomeService(_locationRepositoryMock.Object, _mapperMock.Object, _usersRepositoryMock.Object);
               
            MockData();
        }

        [SetUp]
        public void SetUp()
        {
            _locationRepositoryMock.Reset();
        }

        [Test]
        public async Task GetLocationById_LocationExists_ReturnsLocationDto()
        {
            _locationRepositoryMock.Setup(obj => obj.GetAll())
                .Returns(_locationsQueryableMock.Object);
            _mapperMock.Setup(obj => obj.Map<LocationHomeDto>(_location))
                .Returns(_locationDto);

            var locationResult = await _locationService.GetById(_location.Id);

            locationResult.Should().Be(_locationDto);
        }

        [Test]
        public async Task GetAll_NoParametersPassed_ReturnsListOfLocationDtos()
        {
            _locationRepositoryMock.Setup(s => s.GetAll()).Returns(_locationsQueryableMock.Object);
            _mapperMock.Setup(obj => obj.Map<List<LocationHomeDto>>(
                    It.Is<List<LocationHome>>(x => ListsHasSameElements(x, _locations))))
                .Returns(_locationsDto);

            var locationResult = await _locationService.GetAll();

            locationResult.Should().BeEquivalentTo(_locationsDto);
        }
       
        [Test]
        public async Task RemoveLocation_LocationExists_ReturnsLocationDtoRemoved()
        {
            _locationRepositoryMock.Setup(s => s.FindByIdAsync(_location.Id))
                .ReturnsAsync(_location);
            _mapperMock.Setup(obj => obj.Map<LocationHomeDto>(_location))
                .Returns(_locationDto);

            var locationResult = await _locationService.Remove(_location.Id);

            _locationRepositoryMock.Verify(obj => obj.Remove(_location), Times.Once);
            _locationRepositoryMock.Verify(obj => obj.SaveChangesAsync(), Times.Once);

            locationResult.Should().Be(_locationDto);
        }

        [Test]
        public async Task RemoveLocation_LocationNotExist_ReturnsNull()
        {
            _locationRepositoryMock.Setup(s => s.FindByIdAsync(_location.Id))
                .ReturnsAsync(value: null);

            var locationResult = await _locationService.Remove(_location.Id);

            locationResult.Should().BeNull();
        }

        [Test]
        public async Task Update_ShouldUpdateLocationInDatabase()
        {
            _mapperMock.Setup(obj => obj.Map<LocationHome>(_locationDto))
                .Returns(_location);

            await _locationService.Update(_locationDto);

            _locationRepositoryMock.Verify(obj => obj.Update(_location), Times.Once);
            _locationRepositoryMock.Verify(obj => obj.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task Add_ShouldAddLocationToDatabase()
        {
            _mapperMock.Setup(obj => obj.Map<LocationHome>(_locationPostDto))
                .Returns(_location);

            _usersRepositoryMock.Setup(obj => obj.FindByIdAsync(It.IsAny<int>())).ReturnsAsync(_user);

            await _locationService.Add(_locationPostDto);

            _locationRepositoryMock.Verify(obj => obj.Add(_location), Times.Once);
            _locationRepositoryMock.Verify(obj => obj.SaveChangesAsync(), Times.Once);
            _usersRepositoryMock.Verify(obj => obj.Update(_user), Times.Once);
            _usersRepositoryMock.Verify(obj => obj.SaveChangesAsync(), Times.Once);
        }

        private void MockData()
        {
            _locations = new List<LocationHome>
            {
                new LocationHome()
                {
                    Id = 1,
                    City = "Lviv",
                    IsActive = true,
                    Street = "Panasa Myrnogo",
                    Latitude = 49.8263716,
                    Longitude = 23.9449697
                },
                new LocationHome()
                {
                    Id = 2,
                    City = "Lviv",
                    IsActive = true,
                    Street = "Gorodotska",
                    Latitude = 49.8263716,
                    Longitude = 23.9449697
                }
            };

            _locationsDto = _locations.Select(location => new LocationHomeDto
            {
                Id = location.Id,
                City = location.City,
                IsActive = location.IsActive,
                Street = location.Street
            }).ToList();

            _locationPostDto = _locations.Select(location => new LocationHomePostDto
            {
                Id = location.Id,
                City = location.City,
                IsActive = location.IsActive,
                Street = location.Street,
                UserId = 1
            }).ToList().FirstOrDefault();

            _locationsQueryableMock = _locations.AsQueryable().BuildMock();
            _location = _locations.FirstOrDefault();
            _locationDto = _locationsDto.FirstOrDefault();
        }

        private bool ListsHasSameElements(List<LocationHome> obj1, List<LocationHome> obj2)
        {
            var tempList1 = obj1.Except(obj2).ToList();
            var tempList2 = obj2.Except(obj1).ToList();

            return !(tempList1.Any() || tempList2.Any());
        }
    }
}
