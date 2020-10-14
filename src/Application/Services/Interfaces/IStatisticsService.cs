using System;
using System.Threading.Tasks;
using Application.Dto;
using Application.Dto.Statistics;

namespace Application.Services.Interfaces
{
    public interface IStatisticsService
    {
        /// <summary>
        /// Retrieves data about user donations
        /// </summary>
        /// <param name="from">Start date for search</param>
        /// <returns></returns>
        Task<PieChartData> GetUserDonationsData(DateTime from, string language = "en");

        /// <summary>
        /// Retrieves data about the books read by the user
        /// </summary>
        /// <returns></returns>
        PieChartData GetUserReadData(string language = "en");

        /// <summary>
        /// Retrieves data about languages of book read by user
        /// </summary>
        /// <returns></returns>
        PieChartData GetUserMostReadLanguagesData();

        /// <summary>
        /// Get statistics data about book's genres that are reading
        /// </summary>
        /// <param name="query">filters</param>
        Task<StatisticsChartData> GetReadingStatisticsData(StatisticsQueryParams query);

        /// <summary>
        /// Get statistics data about donations
        /// </summary>
        /// <param name="query">filters</param>
        /// <returns></returns>
        Task<StatisticsChartData> GetDonationStatisticsData(StatisticsQueryParams query);
    }
}
