using System;
using System.Threading.Tasks;
using Application.Dto;
using Application.Dto.Statistics;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookCrossingBackEnd.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticsController : ControllerBase
    {
        private readonly IWishListService _wishListService;
        private readonly IUserResolverService _userResolverService;
        private readonly IBookService _bookService;
        private readonly IRequestService _requestService;
        private readonly IStatisticsService _statisticsService;

        public StatisticsController(
            IStatisticsService statisticsService,
            IWishListService wishListService, 
            IUserResolverService userResolverService,
            IRequestService requestService,
            IBookService bookService
        )
        {
            _statisticsService = statisticsService;
            _wishListService = wishListService;
            _requestService = requestService;
            _userResolverService = userResolverService;
            _bookService = bookService;
        }

        [HttpGet("counters")]
        public async Task<ActionResult<CountersSetDto>> GetCounters()
        {
            var userId = _userResolverService.GetUserId();
            var wishedCount = await _wishListService.GetNumberOfWishedBooksAsync(userId);
            var requestedCount = await _requestService.GetNumberOfRequestedBooksAsync(userId);
            var readCount = await _bookService.GetNumberOfBooksInReadStatusAsync(userId);
            var numberOfTimesRegisteredBooksWereRead = await _bookService.GetNumberOfTimesRegisteredBooksWereReadAsync(userId);
            var countersDto = new CountersSetDto()
            {
                WishedBooksCount = wishedCount,
                RequestedBooksCount = requestedCount,
                ReadBooksCount = readCount,
                RegisteredBooksWereReadCount = numberOfTimesRegisteredBooksWereRead
            };

            return countersDto;
        }

        [HttpGet("reading")]
        public async Task<ActionResult<StatisticsChartData>> GetReadingStatistics([FromQuery] StatisticsQueryParams query)
        {
            return await _statisticsService.GetReadingStatisticsData(query);
        }

        [HttpGet("donation")]
        public async Task<ActionResult<StatisticsChartData>> GetDonationStatisticsAsync([FromQuery] StatisticsQueryParams query)
        {
            return await _statisticsService.GetDonationStatisticsData(query);
        }

        [HttpGet("userDonations")]
        public async Task<ActionResult<PieChartData>> GetUserDonationsData([FromQuery] int? amountOfDays)
        {
            var startDate = amountOfDays.HasValue ? DateTime.Now.AddDays(-amountOfDays.Value) : DateTime.MinValue;

            return Ok(await _statisticsService.GetUserDonationsData(startDate));
        }

        [HttpGet("userRead")]
        public ActionResult<PieChartData> GetUserReadData()
        {
            return Ok(_statisticsService.GetUserReadData());
        }

        [HttpGet("bookLanguages")]
        public ActionResult<PieChartData> GetUserMostReadLanguages()
        {
            return Ok(_statisticsService.GetUserMostReadLanguagesData());
        }
    }
}
