using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Dto;
using Application.Dto.QueryParams;
using Domain.RDBMS.Entities;

namespace Application.Services.Interfaces
{
    public interface IBookService
    {
        /// <summary>
        /// Retrieve book by ID
        /// </summary>
        /// <param name="bookId">Book's ID</param>
        /// <returns>returns Book DTO</returns>
        Task<BookGetDto> GetByIdAsync(int bookId);

        /// <summary>
        /// Retrieve all books
        /// </summary>
        /// <returns>returns list of Book DTOs</returns>
        Task<PaginationDto<BookGetDto>> GetAllAsync(BookQueryParams parameters);

        /// <summary>
        /// Update specified book
        /// </summary>
        /// <param name="book">Book DTO instance</param>
        /// <returns></returns>
        Task<bool> UpdateAsync(BookPutDto book);

        /// <summary>
        /// Remove book from database
        /// </summary>
        /// <param name="bookId">Book's ID</param>
        /// <returns></returns>
        Task<bool> RemoveAsync(int bookId);

        /// <summary>
        /// Create new book and add it into Database
        /// </summary>
        /// <param name="book">Book DTO instance</param>
        /// <returns>Returns inserted Book's ID</returns>
        Task<BookGetDto> AddAsync(BookPostDto book);

        /// <summary>
        /// Retrieve books registered by user
        /// </summary>
        ///  <param name="parameters">filter parametrs</param>
        /// <returns></returns>
        Task<PaginationDto<BookGetDto>> GetRegisteredAsync(BookQueryParams parameters);

        /// <summary>
        /// Retrieve books registered by current user
        /// </summary>
        /// <returns>IQueryable collection of books</returns>
        Task<IQueryable<Book>> GetRegisteredAsync();

        /// <summary>
        /// Retrieve books current owned by user
        /// </summary>
        ///  <param name="parameters">filter parametrs</param>
        /// <returns></returns>
        Task<PaginationDto<BookGetDto>> GetCurrentOwned(BookQueryParams parameters);

        /// <summary>
        /// Retrieve books read by current user
        /// </summary>
        ///  <param name="parameters">filter parametrs</param>
        /// <returns></returns>
        /// 

        Task<PaginationDto<BookGetDto>> GetCurrentRead(BookQueryParams parameters);

        /// <summary>
        /// Retrieve books currently read by user
        /// </summary>
        ///  <param name="parameters">filter parametrs</param>
        /// <returns></returns>
        /// 

        Task<List<BookGetDto>> GetCurrentOwnedById(int id);


        Task<int> GetCurrentOwnedByIdCount(int userId);

        Task<PaginationDto<BookGetDto>> GetReadBooksAsync(BookQueryParams parameters);

        /// <summary>
        /// Retrieve books that were read by current user
        /// </summary>
        /// <returns></returns>
        IQueryable<Book> GetAlreadyReadBooks();

        /// <summary>
        /// Retrieve books in read status
        /// </summary>
        /// <returns></returns>
        IQueryable<Book> GetBooksInReadStatus();

        /// <summary>
        /// Change book`s status to available
        /// </summary>
        ///  <param name="bookId">Book Id</param>
        /// <returns></returns>
        Task<bool> ActivateAsync(int bookId);

        /// <summary>
        /// Change book`s status to InActive
        /// </summary>
        ///  <param name="bookId">Book Id</param>
        /// <returns></returns>
        Task<bool> DeactivateAsync(int bookId);

        /// <summary>
        /// Get number of times registered books were read
        /// </summary>
        /// <returns>Number of times</returns>
        Task<int> GetNumberOfTimesRegisteredBooksWereReadAsync(int userId);

        /// <summary>
        /// Get number of books that are in read status
        /// </summary>
        /// <param name="userId"> User id </param>
        /// <returns> Number of books </returns>
        Task<int> GetNumberOfBooksInReadStatusAsync(int userId);

        /// <summary>
        /// Add rating mark for book from user
        /// </summary>
        /// <param name="userId">Id of user who is trying to set rating</param>
        /// <param name="rating">Mark</param>
        /// <returns></returns>
        Task<bool> SetRating(BookRatingQueryParams ratingQueryParams);

        /// <summary>
        /// Get rating for book of specific user
        /// </summary>
        /// <param name="bookId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<double> GetRating(int bookId, int userId);

        /// <summary>
        /// Get books transitions between users
        /// </summary>
        /// <returns></returns>
        IEnumerable<Request> GetBooksTransitions();

        public  void SendMailForActivated(Book book, User user);
        public void SendMailForDeactivated(Book book, User user);


        public void SendNotificationToUser(int userId, int bookId, string message, string messageUk);

        /// <summary>
        /// Get locations with books quantity
        /// </summary>
        /// <returns></returns>
        IEnumerable<MapLocationDto> GetBooksQuantityOnLocations();

    }
}
