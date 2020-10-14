using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Dto.Comment.Book;
using Domain.NoSQL.Entities;

namespace Application.Services.Interfaces
{
    public interface IBookChildCommentService
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
        /// <param name="rootId">Id of root comment of the book</param>
        /// <returns></returns>
        Task UpdateAIRating(string rootId);

        /// <summary>
        /// Update specified comment
        /// </summary>
        /// <param name="updateDto">Book commet update DTO instance</param>
        /// <param name="updateDto.Ids">
        /// If ids length greater than 1, child comment will be updated. 
        /// </param>
        /// <returns>Number of updated comments</returns>
        Task<int> Update(ChildUpdateDto updateDto);

        /// <summary>
        /// Remove commnet from database
        /// </summary>
        /// <param name="ids">
        /// If ids length greater than 1, child comment will be deleted. 
        /// </param>
        /// <returns>Number of removed comments</returns>
        Task<int> Remove(IEnumerable<string> ids);

        /// <summary>
        /// Create new comment and add it into Database
        /// </summary>
        /// <param name="insertDto">Commnet DTO instance</param>
        /// <param name="insertDto.Ids">
        /// If ids length greater than 0, dto will  insert in childe array of comments. 
        /// </param>
        /// <returns>Number of inserted commnets</returns>
        Task<int> Add(ChildInsertDto insertDto);
    }
}
