using System;
using System.Threading.Tasks;
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
        Task<PieChartData> GetUserDonationsData(DateTime from);

        /// <summary>
        /// Retrieves data about the books read by the user
        /// </summary>
        /// <returns></returns>
        PieChartData GetUserReadData();

        /// <summary>
        /// Retrieves data about languages of book read by user
        /// </summary>
        /// <returns></returns>
        PieChartData GetUserMostReadLanguagesData();
    }
}
