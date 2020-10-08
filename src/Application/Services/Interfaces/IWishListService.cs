using System.Threading.Tasks;
using Application.Dto;
using Application.Dto.QueryParams;

namespace Application.Services.Interfaces
{
    public interface IWishListService
    {
        Task<PaginationDto<BookGetDto>> GetWishesOfCurrentUserAsync(PageableParams pageableParams);

        Task AddWishAsync(int bookId);

        Task RemoveWishAsync(int bookId);

        Task NotifyAboutAvailableBookAsync(int bookId);

        Task<bool> CheckIfBookInWishListAsync(int bookId);

        /// <summary>
        /// Get number of wished list in user's wish-list
        /// </summary>
        /// <param name="userId"> The user of the wish list </param>
        /// <returns> Number of wished books </returns>
        Task<int> GetNumberOfWishedBooksAsync(int userId);

        /// <summary>
        /// Gets number of users that wish the book with bookId
        /// </summary>
        /// <param name="bookId"></param>
        /// <returns>Number of users who wish the book with bookId</returns>
        Task<int> GetNumberOfBookWishersByBookIdAsync(int bookId);
    }
}
