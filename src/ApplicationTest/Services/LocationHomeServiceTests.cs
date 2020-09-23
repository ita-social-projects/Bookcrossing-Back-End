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
        #region Fields
        private BookCrossingContext _context;
        private LocationHomeService _locationService;

        private Mock<IRepository<LocationHome>> _locationRepositoryMock;
        private Mock<IRepository<User>> _usersRepositoryMock;
        private Mock<IQueryable<LocationHome>> _locationsQueryableMock;

        private IMapper _mapper;
        private IEnumerable<LocationHome> _locations;
        private IEnumerable<User> _users;
        private LocationHome _location;
        private LocationHomeDto _locationDto;
        private LocationHomePostDto _locationPostDto;
        #endregion

        #region SetupFunctions
        public void SetupMapper()
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new Application.MapperProfilers.LocationHomeProfile());
                mc.AddProfile(new Application.MapperProfilers.UserProfile());
            });
            _mapper = mappingConfig.CreateMapper();
        }
        public void SetupRepositories()
        {
            _locationRepositoryMock = new Mock<IRepository<LocationHome>>();
            _usersRepositoryMock = new Mock<IRepository<User>>();
            _locationsQueryableMock = new Mock<IQueryable<LocationHome>>();
        }
        private void SetupData()
        {
            _locations = SetupLocations;

            _users = SetupUsers;

            _location = _locations.First();

            _locationDto = _mapper.Map<LocationHomeDto>(_location);

            _locationPostDto = _mapper.Map<LocationHomePostDto>(_location);
            _locationPostDto.UserId = 1;
        }
        private void SetupContext()
        {
            var options = new DbContextOptionsBuilder<BookCrossingContext>()
                .UseInMemoryDatabase(databaseName: "Fake DB")
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _context = new BookCrossingContext(options);

            _locationService = new LocationHomeService(
                _locationRepositoryMock.Object,
                _mapper,
                _usersRepositoryMock.Object);
        }
        #endregion

        private IEnumerable<LocationHome> SetupLocations
        {
            get
            {
                yield return new LocationHome()
                {
                    Id = 1,
                    City = "Lviv",
                    IsActive = true,
                    Street = "Panasa Myrnogo",
                    Latitude = 1,
                    Longitude = 0
                };
                yield return new LocationHome()
                {
                    Id = 2,
                    City = "Lviv",
                    IsActive = true,
                    Street = "Gorodotska",
                    Latitude = 0,
                    Longitude = 1
                };
            }
        }
        private IEnumerable<User> SetupUsers
        {
            get
            {
                yield return new User
                {
                    Id = 1,
                    LocationHomeId = 1,
                    FirstName = "Volodymyr",
                    LastName = "Zelenskiy"
                };
                yield return new User
                {
                    Id = 2,
                    LocationHomeId = 2,
                    FirstName = "Donald",
                    LastName = "Trump"
                };
            }
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            SetupMapper();

            SetupRepositories();

            SetupData();

            SetupContext();
        }

        [SetUp]
        public void SetUp()
        {
            _locationRepositoryMock.Reset();
        }

        [Test]
        public async Task GetLocationById_LocationExists_ReturnsLocationDto()
        {
            _locationRepositoryMock.Setup(m => m.FindByIdAsync(_location.Id)).ReturnsAsync(_location);

            var locationResult = await _locationService.GetById(_location.Id);

            locationResult.Should().BeEquivalentTo(_mapper.Map<LocationHomeDto>(_location));
        }

        [Test]
        public async Task GetAll_NoParametersPassed_ReturnssIEnumerableOfLocationHomeDtos()
        {
            _locationsQueryableMock = _locations.AsQueryable().BuildMock();
            _locationRepositoryMock.Setup(m => m.GetAll()).Returns(_locationsQueryableMock.Object);

            var locationResult = await _locationService.GetAll();

            locationResult.Should().BeEquivalentTo(_mapper.Map<IEnumerable<LocationHomeDto>>(_locations));
        }

        #region Remove
        [Test]
        public async Task RemoveLocation_LocationExists_VerifyRemoval()
        {
            var location = _locations.Last();
            _locationRepositoryMock.Setup(m => m.FindByIdAsync(location.Id)).ReturnsAsync(location);
            _locationRepositoryMock.Setup(m => m.Remove(location));

            var result = await _locationService.Remove(location.Id);

            _locationRepositoryMock.Verify(obj => obj.Remove(location), Times.Once);
        }

        [Test]
        public async Task RemoveLocation_LocationExists_VerifySaveChanges()
        {
            var location = _locations.Last();
            _locationRepositoryMock.Setup(m => m.FindByIdAsync(location.Id)).ReturnsAsync(location);
            _locationRepositoryMock.Setup(m => m.Remove(location));

            var result = await _locationService.Remove(location.Id);

            _locationRepositoryMock.Verify(obj => obj.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task RemoveLocation_LocationExists_ReturnsDto()
        {
            var location = _locations.Last();
            _locationRepositoryMock.Setup(m => m.FindByIdAsync(location.Id)).ReturnsAsync(location);
            _locationRepositoryMock.Setup(m => m.Remove(location));

            var result = await _locationService.Remove(location.Id);

            result.Should().BeEquivalentTo(_mapper.Map<LocationHomeDto>(location));
        }

        [Test]
        public async Task RemoveLocation_LocationNotExist_ReturnsNull()
        {
            _locationRepositoryMock.Setup(s => s.FindByIdAsync(_locations.First().Id))
                .ReturnsAsync(value: null);

            var locationResult = await _locationService.Remove(_locations.First().Id);

            locationResult.Should().BeNull();
        }
        #endregion

        #region Update
        [Test]
        public async Task Update_ShouldUpdate_VerifySaveChangesAsync()
        {
            _locationRepositoryMock.Setup(m => m.Update(_locations.First()));

            await _locationService.Update(_locationDto);

            _locationRepositoryMock.Verify(obj => obj.SaveChangesAsync(), Times.Once);
        }
        #endregion

        [TestFixture]
        public class LocationHomeServiceAddTests: LocationHomeServiceTests
        {
            [SetUp]
            public void SetUpAdd()
            {
                _usersRepositoryMock.Reset();
            }
            [Test]
            public async Task Add_UserExists_VerifyAdd()
            {
                _usersRepositoryMock.Setup(m => m.FindByIdAsync(It.IsAny<int>())).ReturnsAsync(_users.First());

                await _locationService.Add(_locationPostDto);

                _locationRepositoryMock.Verify(obj => obj.Add(It.IsAny<LocationHome>()), Times.Once);
            }

            [Test]
            public async Task Add_UserExists_VerifySaveChangesAsync()
            {
                _usersRepositoryMock.Setup(m => m.FindByIdAsync(It.IsAny<int>())).ReturnsAsync(_users.First());

                await _locationService.Add(_locationPostDto);

                _locationRepositoryMock.Verify(obj => obj.SaveChangesAsync(), Times.Once);
            }

            [Test]
            public async Task Add_UserExists_Update()
            {
                _usersRepositoryMock.Setup(m => m.FindByIdAsync(It.IsAny<int>())).ReturnsAsync(_users.First());
                _usersRepositoryMock.Setup(m => m.Update(It.IsAny<User>()));

                await _locationService.Add(_locationPostDto);

                _usersRepositoryMock.Verify(obj => obj.Update(It.IsAny<User>()), Times.Once);
            }

            [Test]
            public async Task Add_UserExists_ReturnsZero()
            {
                _usersRepositoryMock.Setup(m => m.FindByIdAsync(It.IsAny<int>())).ReturnsAsync(_users.First());
                var result = await _locationService.Add(_locationPostDto);

                result.Should().Be(1);
            }

            [Test]
            public async Task Add_UserDoesNotExists_VerifySaveChanges()
            {
                _usersRepositoryMock.Setup(m => m.FindByIdAsync(_locationPostDto.UserId)).ReturnsAsync(value: null);

                await _locationService.Add(_locationPostDto);

                _usersRepositoryMock.Verify(obj => obj.SaveChangesAsync(), Times.Never);
            }

            [Test]
            public async Task Add_UserDoesNotExists_ReturnsZero()
            {
                _usersRepositoryMock.Setup(m => m.FindByIdAsync(_locationPostDto.UserId)).ReturnsAsync(value: null);

                var result = await _locationService.Add(_locationPostDto);

                result.Should().Be(0);
            }
        }
        

        private bool ListsHasSameElements(IEnumerable<LocationHome> obj1, IEnumerable<LocationHome> obj2)
        {
            var tempList1 = obj1.Except(obj2);
            var tempList2 = obj2.Except(obj1);

            return !(tempList1.Any() || tempList2.Any());
        }
    }
}
