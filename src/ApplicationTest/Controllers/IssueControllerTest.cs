using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Dto;
using Application.Dto.QueryParams;
using Application.Services.Interfaces;
using BookCrossingBackEnd.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace ApplicationTest.Controllers
{
    [TestFixture]
    class IssueControllerTest
    {

        private Mock<IIssueService> _issueService;
        private IssueController _issueController;
        private Mock<ILogger<IssueController>> _logger;

        [OneTimeSetUp]
        public void Setup()
        {
            _issueService = new Mock<IIssueService>();
            _logger = new Mock<ILogger<IssueController>>();
            _issueController = new IssueController(_issueService.Object, _logger.Object);
        }

        [Test]
        public async Task GetAllIssuesAsync_Success_ReturnsOkObjectResultWithRequestedCount()
        {
            var testIssues = GetTestIssues();
            _issueService.Setup(s => s.GetAll()).ReturnsAsync(testIssues);

            var result = await _issueController.GetAllIssues();

            var okResult = result.Result as OkObjectResult;
            okResult.Should().BeOfType<OkObjectResult>();
            var genres = okResult.Value as List<IssueDto>;

            genres.Count().Should().Be(testIssues.Count);
        }

        [Test]
        public async Task GetAllIssues_AnyFullPaginationQueryParams_ReturnsPaginatedDtoListOfIssueGetDto()
        {
            var testIssues = GetTestIssues();

            _issueService.Setup(s => s.GetAll(It.IsAny<FullPaginationQueryParams>()))
                .ReturnsAsync(new PaginationDto<IssueDto>()
                {
                    Page = testIssues,
                    TotalCount = 1
                });

            var result = await _issueController.GetAllIssues(It.IsAny<FullPaginationQueryParams>());

            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<PaginationDto<IssueDto>>>();
            result.Value.Page.Should().NotBeNull().And.NotContainNulls();
        }

        List<IssueDto> GetTestIssues()
        {
            return new List<IssueDto>
            {
                new IssueDto(),
                new IssueDto(),
                new IssueDto()
            };
        }

        [Test]
        public async Task GetIssueAsync_IssueExists_Returns_OkObjectResultWithRequestedId()
        {
            var testIssue = GetTestIssue();
            _issueService.Setup(s => s.GetById(It.IsAny<int>())).ReturnsAsync(testIssue);

            var genreResult = await _issueController.GetIssue(It.IsAny<int>());

            var okResult = genreResult.Result as OkObjectResult;
            okResult.Should().BeOfType<OkObjectResult>();
            var resultIssue = okResult.Value as IssueDto;
            resultIssue.Id.Should().Be(testIssue.Id);
        }

        private IssueDto GetTestIssue()
        {
            return new IssueDto() { Id = 1, Name = "Issue" };
        }

        [Test]
        public async Task GetIssueAsync_IssueDoesNotExist_Returns_NotFoundResult()
        {
            _issueService.Setup(s => s.GetById(It.IsAny<int>())).ReturnsAsync(null as IssueDto);

            var result = await _issueController.GetIssue(It.IsAny<int>());

            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public async Task PutIssue_IssueExists_Returns_NoContent()
        {
            _issueService.Setup(s => s.Update(It.IsAny<IssueDto>())).ReturnsAsync(true);

            var result = await _issueController.PutIssue(It.IsAny<IssueDto>());

            result.Should().BeOfType<NoContentResult>();
        }

        [Test]
        public async Task PutIssue_IssueDoesNotExist_Return_NotFound()
        {
            _issueService.Setup(s => s.Update(It.IsAny<IssueDto>())).ReturnsAsync(false);

            var result = await _issueController.PutIssue(It.IsAny<IssueDto>());

            result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public async Task PostIssue_Returns_CreatedAtActionResult()
        {
            var testIssue = GetTestIssue();
            _issueService.Setup(m => m.Add(It.IsAny<IssueDto>())).ReturnsAsync(testIssue);

            var createdAtActionResult = await _issueController.PostIssue(It.IsAny<IssueDto>());
            var result = (IssueDto)((CreatedAtActionResult)createdAtActionResult.Result).Value;

            result.Should().BeOfType<IssueDto>();
            createdAtActionResult.Result.Should().BeOfType<CreatedAtActionResult>();
            result.Should().BeEquivalentTo(testIssue, options => options.Excluding(a => a.Id));
        }

        [Test]
        public async Task DeleteIssue_IssueExists_Returns_OkResult()
        {
            _issueService.Setup(s => s.Remove((It.IsAny<int>()))).ReturnsAsync(true);

            var result = await _issueController.DeleteIssue(It.IsAny<int>());

            result.Should().BeOfType<OkResult>();
        }

        [Test]
        public async Task DeleteIssue_IssueDoesNotExist_Returns_NotFoundResult()
        {
            _issueService.Setup(s => s.Remove(It.IsAny<int>())).ReturnsAsync(false);

            var result = await _issueController.DeleteIssue(It.IsAny<int>());

            result.Should().BeOfType<NotFoundResult>();
        }

    }
}
