using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Dto.Comment.Book;
using Domain.NoSQL.Entities;

namespace Application.Services.Interfaces
{
    public interface IBookRootCommentService
    {
        /// <summary>
        /// Recurcively counts the avarage of predicted ratings in the comment tree
        /// </summary>
        /// <param name="comments">Tree of comments in the given comment section</param>
        /// <returns>An avarage rating fro the given comment tree of the book</returns>
        float GetAvaragePredictedRating(IEnumerable<IBookComment> comments);
        /// <summary>
        /// Updating book Predicted rating avarage after any change to it's corresponding comment section
        /// </summary>
        /// <param name="bookId">Id of book</param>
        /// <returns></returns>
        Task UpdateAIRating(int bookId);
        /// <summary>
        /// Recursively gets an avarage rating for child comments pathed
        /// </summary>
        /// <param name="children">Child comments which can contain their children aswell</param>
        /// <returns>Avarage rating for the trees of comments</returns>
        Task<RootDto> GetById(string id);

        /// <summary>
        /// Retrieve comment by book's ID
        /// </summary>
        /// <param name="bookId">Book's ID</param>
        /// <returns>returns enumerable of comment DTOs</returns>
        Task<IEnumerable<RootDto>> GetByBookId(int bookId);

        /// <summary>
        /// Retrieve all comments
        /// </summary>
        /// <returns>returns enumerable of comment DTOs</returns>
        Task<IEnumerable<RootDto>> GetAll();

        /// <summary>
        /// Update specified comment
        /// </summary>
        /// <param name="updateDto">Book commet update DTO instance</param>
        /// <returns>Number of updated comments</returns>
        Task<int> Update(RootUpdateDto updateDto);

        /// <summary>
        /// Remove commnet from database
        /// </summary>
        /// <param name="id">Comment's ID</param>
        /// <returns>Number of removed comments</returns>
        Task<int> Remove(string id);

        /// <summary>
        /// Create new comment and add it into Database
        /// </summary>
        /// <param name="insertDto">Commnet DTO instance</param>
        /// <returns>Number of inserted commnets</returns>
        Task<int> Add(RootInsertDto insertDto);
    }
}
