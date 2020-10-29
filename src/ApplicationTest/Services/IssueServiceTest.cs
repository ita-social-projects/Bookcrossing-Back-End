using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Dto;
using Application.Services.Implementation;
using Application.Services.Interfaces;
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
    class IssueServiceTest
    {

        private IssueService _issueService;
        private Mock<IRepository<Issue>> _issueRepositoryMock;
        private Mock<IPaginationService> _paginationMock;
        private IMapper _mapper;
        private Mock<IQueryable<Issue>> _issuesQueryableMock;
        private List<Issue> _issues;
        private List<IssueDto> _issuesDto;
        private Issue _issue;
        private IssueDto _issueDto;

        [OneTimeSetUp]
        public void ClassSetup()
        {
            var options = new DbContextOptionsBuilder<BookCrossingContext>()
                .UseInMemoryDatabase(databaseName: "Fake DB")
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new Application.MapperProfilers.IssueProfile());
            });

            _mapper = mappingConfig.CreateMapper();
            _issueRepositoryMock = new Mock<IRepository<Issue>>();
            _paginationMock = new Mock<IPaginationService>();
            _issueService = new IssueService(
                _issueRepositoryMock.Object,
                _mapper,
                _paginationMock.Object);

            MockData();
        }

        [SetUp]
        public void TestSetup()
        {
            _issueRepositoryMock.Invocations.Clear();
        }

        #region GetById

        [Test]
        public async Task GetIssueById_IssueExists_Returns_IssueDtoWithRequestedId()
        {
            var issueId = 1;
            var issue = new Issue { Id = issueId };
            var issueDto = new IssueDto { Id = issueId };
            _issueRepositoryMock.Setup(s => s.FindByIdAsync(issueId))
                .ReturnsAsync(issue);

            var issueResult = await _issueService.GetById(issueId);

            issueResult.Should().BeEquivalentTo(issueDto);
        }

        [Test]
        public async Task GetIssueById_IssueDoesNotExist_ReturnsNull()
        {
            var issueId = 1;
            _issueRepositoryMock.Setup(s => s.FindByIdAsync(issueId))
                .ReturnsAsync(value: null);

            var issueResult = await _issueService.GetById(issueId);

            issueResult.Should().BeNull();
        }

        #endregion GetById

        #region Post

        //mock mapper to return concrete instancw of dto
        //or check by id to not mock mapper
        [Test]
        public async Task AddIssue_IssueIsValid_Returns_IssueDto()
        {
            var issueDto = new IssueDto();

            var issueResult = await _issueService.Add(issueDto);

            _issueRepositoryMock.Verify(x => x.Add(It.IsAny<Issue>()), Times.Once);
            _issueRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        #endregion Post

        #region Delete

        //Make new test for verifying and for true check
        [Test]
        public async Task RemoveIssue_IssueExists_ReturnsTrue()
        {
            var issueId = 1;
            var issue = new Issue { Id = issueId };
            _issueRepositoryMock.Setup(s => s.FindByIdAsync(issueId))
                .ReturnsAsync(issue);
            _issueRepositoryMock.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            var issueResult = await _issueService.Remove(issueId);

            _issueRepositoryMock.Verify(x => x.Remove(issue), Times.Once);
            _issueRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);

            issueResult.Should().BeTrue();
        }

        [Test]
        public async Task RemoveIssue_IssueDoesNotExist_ReturnsFalse()
        {
            var issueId = 1;
            _issueRepositoryMock.Setup(s => s.FindByIdAsync(issueId))
                .ReturnsAsync(value: null);

            var issueResult = await _issueService.Remove(issueId);

            issueResult.Should().BeFalse();
        }

        #endregion Delete

        #region Update

        [Test]
        public async Task UpdateIssue_IssueDoesNotExists_ReturnsFalse()
        {
            _issueRepositoryMock.Setup(s => s.GetAll()).Returns(_issuesQueryableMock.Object);
            _issueRepositoryMock.Setup(s => s.Update(It.IsAny<Issue>()));
            _issueRepositoryMock.Setup(s => s.SaveChangesAsync()).ReturnsAsync(0);
            var iss = new IssueDto() { Id = 1 };

            var result = await _issueService.Update(iss);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task UpdateIssue_IssueExists_ReturnsTrue()
        {
            _issueRepositoryMock.Setup(s => s.GetAll()).Returns(_issuesQueryableMock.Object);
            _issueRepositoryMock.Setup(s => s.Update(It.IsAny<Issue>()));
            _issueRepositoryMock.Setup(s => s.SaveChangesAsync()).ReturnsAsync(1);
            var iss = new IssueDto() { Id = 1 };

            var result = await _issueService.Update(iss);

            Assert.IsTrue(result);
        }

        #endregion Update

        #region GetAll

        [Test]
        public async Task GetAll_NoParametersPassed_ReturnsListOfIssueDtos()
        {
            var localIssues = _issues.AsQueryable();
            _issueRepositoryMock.Setup(s => s.GetAll()).Returns(localIssues);

            var issueResult = await _issueService.GetAll();

            //make expected or mock mapper for concrete instance
            issueResult.Should().BeEquivalentTo(_mapper.Map<IEnumerable<IssueDto>>(localIssues));
        }

        #endregion

        private void MockData()
        {
            _issues = new List<Issue>
            {
                new Issue()
                {
                    Id = 1,
                    Name = "General"
                },
                new Issue()
                {
                    Id = 2,
                    Name = "Support"
                }
            };

            _issuesDto = _issues.Select(issue => new IssueDto
            {
                Id = issue.Id,
                Name = issue.Name
            }).ToList();

            _issuesQueryableMock = _issues.AsQueryable().BuildMock();
            _issue = _issues.FirstOrDefault();
            _issueDto = _issuesDto.FirstOrDefault();
        }

    }
}
