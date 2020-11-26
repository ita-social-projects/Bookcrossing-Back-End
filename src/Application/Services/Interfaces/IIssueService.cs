using Application.Dto;
using Application.Dto.QueryParams;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IIssueService
    {
        /// <summary>
        /// Retrieve Issue by ID
        /// </summary>
        /// <param name="issueId">Issue's ID</param>
        /// <returns>returns Issue DTO</returns>
        Task<IssueDto> GetById(int issueId);

        /// <summary>
        /// Retrieve all Issues
        /// </summary>
        /// <returns>returns list of Issues DTOs</returns>
        Task<IEnumerable<IssueDto>> GetAll();

        /// <summary>
        /// Retrieve Pagination for Issue
        /// </summary>
        /// <param name="fullPaginationQuery">QueryParameters containing page index, pageSize, searchQuery and if it's a first Request</param>
        /// <returns>Returns Pagination with Page result and Total amount of items</returns>
        Task<PaginationDto<IssueDto>> GetAll(FullPaginationQueryParams fullPaginationQuery);

        /// <summary>
        /// Update specified Issue
        /// </summary>
        /// <param name="issueDto">Issue's DTO instance</param>
        /// <returns></returns>
        Task<bool> Update(IssueDto issueDto);

        /// <summary>
        /// Remove issue from database
        /// </summary>
        /// <param name="issueId">Issue's ID</param>
        /// <returns>Returns removed issue's DTO</returns>
        Task<bool> Remove(int issueId);

        /// <summary>
        /// Create new issue and add it into Database
        /// </summary>
        /// <param name="issue">New Issue DTO instance</param>
        /// <returns>Returns created Issue's DTO </returns>
        Task<IssueDto> Add(IssueDto issueDto);
    }
}
