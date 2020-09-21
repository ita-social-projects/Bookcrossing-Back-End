using Application.Dto;
using Application.Dto.QueryParams;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface ISuggestionMessageService
    {
        /// <summary>
        /// Retrieve message by ID
        /// </summary>
        /// <param name="messageId">Message's ID</param>
        /// <returns>returns Message DTO</returns>
        Task<SuggestionMessageDto> GetById(int messageId);

        /// <summary>
        /// Retrieve all messages
        /// </summary>
        /// <returns>returns list of Message DTOs</returns>
        Task<List<SuggestionMessageDto>> GetAll();
        /// <summary>
        /// Retrieve Pagination for Message
        /// </summary>
        /// <param name="fullPaginationQuery">QueryParameters containing page index, pageSize, searchQuery and if it's a first Request</param>
        /// <returns>Returns Pagination with Page result and Total amount of items</returns>
        Task<PaginationDto<SuggestionMessageDto>> GetAll(FullPaginationQueryParams fullPaginationQuery);

        /// <summary>
        /// Update specified message
        /// </summary>
        /// <param name="message">Message DTO instance</param>
        /// <returns></returns>
        Task<bool> Update(SuggestionMessageDto message);

        /// <summary>
        /// Remove message from database
        /// </summary>
        /// <param name="messageId">Message's ID</param>
        /// <returns>Returns removed Message DTO</returns>
        Task<SuggestionMessageDto> Remove(int messageId);

        /// <summary>
        /// Create new message and add it into Database
        /// </summary>
        /// <param name="message">Message DTO instance</param>
        /// <returns>Returns inserted Message's ID</returns>
        Task<int> Add(SuggestionMessageDto message);
    }
}
