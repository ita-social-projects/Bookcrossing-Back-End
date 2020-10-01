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
        private Mock<IRepository<Book>> _bookRepositoryMock;
        private Mock<IQueryable<LocationHome>> _locationsQueryableMock;

        private IMapper _mapper;
        private IEnumerable<LocationHome> _locations;
        private IEnumerable<User> _users;
        private LocationHome _location;
        private int _locationId;
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
            _bookRepositoryMock = new Mock<IRepository<Book>>();
        }
        private void SetupData()
        {
            _locations = SetupLocations;

            _users = SetupUsers;

            _location = _locations.First();
            _locationId = _location.Id;

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
                _usersRepositoryMock.Object,
                _bookRepositoryMock.Object);
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
            _locationRepositoryMock.Setup(m => m.FindByIdAsync(_locationId)).ReturnsAsync(_location);

            var locationResult = await _locationService.GetById(_locationId);

            locationResult.Should().BeEquivalentTo(_locationDto);
        }


        #region Remove
        [Test]
        public async Task RemoveLocation_LocationExists_VerifyRemoval()
        {
            _locationRepositoryMock.Setup(m => m.FindByIdAsync(_locationId)).ReturnsAsync(_location);
            _locationRepositoryMock.Setup(m => m.Remove(_location));

            var result = await _locationService.Remove(_locationId);

            _locationRepositoryMock.Verify(obj => obj.Remove(_location), Times.Once);
        }

        [Test]
        public async Task RemoveLocation_LocationExists_VerifySaveChanges()
        {
            _locationRepositoryMock.Setup(m => m.FindByIdAsync(_locationId)).ReturnsAsync(_location);
            _locationRepositoryMock.Setup(m => m.Remove(_location));

            var result = await _locationService.Remove(_locationId);

            _locationRepositoryMock.Verify(obj => obj.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task RemoveLocation_LocationExists_ReturnsDto()
        {
            _locationRepositoryMock.Setup(m => m.FindByIdAsync(_locationId)).ReturnsAsync(_location);
            _locationRepositoryMock.Setup(m => m.Remove(_location));

            var result = await _locationService.Remove(_locationId);

            result.Should().BeEquivalentTo(_locationDto);
        }

        [Test]
        public async Task RemoveLocation_LocationNotExist_ReturnsNull()
        {
            _locationRepositoryMock.Setup(s => s.FindByIdAsync(_location))
                .ReturnsAsync(value: null);

            var locationResult = await _locationService.Remove(_locationId);

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
            private Mock<IMapper> _mapperMock;
            private int _userId;
            private User _user;

            [SetUp]
            public void SetUpAdd()
            {
                _usersRepositoryMock.Reset();
                _locationRepositoryMock.Reset();
            }

            private void SetUpMapper()
            {
                _mapperMock = new Mock<IMapper>();
                _mapperMock.Setup(m => m.Map<LocationHomeDto>(_location)).Returns(_locationDto);
                _mapperMock.Setup(m => m.Map<LocationHome>(_locationDto)).Returns(_location);
                _mapperMock.Setup(m => m.Map<LocationHomePostDto>(_location)).Returns(_locationPostDto);
                _mapperMock.Setup(m => m.Map<LocationHome>(_locationPostDto)).Returns(_location);
            }

            [OneTimeSetUp]
            public void OneTimeSetupAdd()
            {
                _user = _users.First();
                _userId = _user.Id;

                SetUpMapper();

                _locationService = new LocationHomeService(
                _locationRepositoryMock.Object,
                _mapperMock.Object,
                _usersRepositoryMock.Object,
                _bookRepositoryMock.Object);
            }

            [Test]
            public async Task Add_UserExists_VerifyLocationUpdate()
            {
                
                _usersRepositoryMock.Setup(m => m.FindByIdAsync(_userId)).ReturnsAsync(_user);

                await _locationService.Update(_locationPostDto);

                _locationRepositoryMock.Verify(obj => obj.Update(_location), Times.Once);
            }

            [Test]
            public async Task Add_UserExists_VerifySaveChangesAsync()
            {
                _usersRepositoryMock.Setup(m => m.FindByIdAsync(_userId)).ReturnsAsync(_user);

                await _locationService.Add(_locationPostDto);

                _locationRepositoryMock.Verify(obj => obj.SaveChangesAsync(), Times.Once);
            }

            [Test]
            public async Task Add_UserExists_VerifyUserUpdate()
            {
                _usersRepositoryMock.Setup(m => m.FindByIdAsync(_userId)).ReturnsAsync(_user);

                _usersRepositoryMock.Setup(m => m.Update(_user));

                await _locationService.Add(_locationPostDto);

                _usersRepositoryMock.Verify(obj => obj.Update(_user), Times.Once);
            }

            [Test]
            public async Task Add_UserExists_ReturnsZero()
            {
                _usersRepositoryMock.Setup(m => m.FindByIdAsync(_userId)).ReturnsAsync(_user);

                var result = await _locationService.Add(_locationPostDto);
                
                result.Should().Be(_userId);
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
                int expectedId = 0;
                _usersRepositoryMock.Setup(m => m.FindByIdAsync(_locationPostDto.UserId)).ReturnsAsync(value: null);

                var result = await _locationService.Add(_locationPostDto);

                result.Should().Be(expectedId);
            }
        }
        
    }
}
