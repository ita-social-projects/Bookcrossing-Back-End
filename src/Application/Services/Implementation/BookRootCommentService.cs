using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Application.Dto.Comment.Book;
using Application.Services.Interfaces;
using Domain.NoSQL;
using Domain.NoSQL.Entities;
using Domain.RDBMS;
using Domain.RDBMS.Entities;
using Org.BouncyCastle.Math.EC.Rfc7748;

namespace Application.Services.Implementation
{
    public class BookRootCommentService : IBookRootCommentService
    {
        private readonly IRootRepository<BookRootComment> _rootCommentRepository;
        private readonly ICommentOwnerMapper _commentOwnerMapper;
        private readonly IRepository<Book> _bookRepository;
        private readonly ISentimentAnalisysService _sentimentAnalisysService;
        public BookRootCommentService(IRootRepository<BookRootComment> rootCommentRepository, ICommentOwnerMapper commentOwnerMapper,
            IRepository<Book> bookRepository, ISentimentAnalisysService sentimentAnalisysService)
        {
            _sentimentAnalisysService = sentimentAnalisysService;
            _rootCommentRepository = rootCommentRepository;
            _commentOwnerMapper = commentOwnerMapper;
            _bookRepository = bookRepository;
        }

        public float GetAvaragePredictedRating(IEnumerable<IBookComment> comments)
        {
            double avarage = 0;
            int count = 0;
            foreach (var comment in comments)
            {
                if (comment.IsDeleted != true)
                {
                    avarage += comment.PredictedRating;
                    count++;
                }
                if (comment.Comments.Any())
                {
                    avarage += GetAvaragePredictedRating(comment.Comments);
                    count++;
                }
            }
            return (float)(avarage / count);
        }
        public async Task UpdateAIRating(int bookId)
        {
            var book = await _bookRepository.FindByIdAsync(bookId);
            var comments = await _rootCommentRepository.FindManyAsync(root => root.BookId == bookId);
            if (comments.Any())
            {
                book.PredictedRating = GetAvaragePredictedRating(comments);
                _bookRepository.Update(book);
                await _bookRepository.SaveChangesAsync();
            }
        }
        public async Task<int> Add(RootInsertDto insertDto)
        {
            var comment = await _rootCommentRepository.InsertOneAsync(
                new BookRootComment(true)
                {
                    Text = insertDto.Text,
                    BookId = insertDto.BookId,
                    OwnerId = insertDto.OwnerId,
                    Date = DateTime.UtcNow.ToString("O"), 
                    PredictedRating = await _sentimentAnalisysService.Predict(insertDto.Text)
                });

            await UpdateAIRating(insertDto.BookId);

            return comment;
        }

        public async Task<IEnumerable<RootDto>> GetAll()
        {
            return await _commentOwnerMapper.MapAsync(await _rootCommentRepository.GetAllAsync());
        }

        public async Task<IEnumerable<RootDto>> GetByBookId(int bookId)
        {
            return await _commentOwnerMapper.MapAsync(await _rootCommentRepository.FindManyAsync(root=>root.BookId==bookId));
        }

        public async Task<RootDto> GetById(string id)
        {
            return await _commentOwnerMapper.MapAsync(await _rootCommentRepository.FindByIdAsync(id));
        }

        public async Task<int> Remove(string id)
        {
            var comment = await _rootCommentRepository.FindByIdAsync(id);
            int deletedCount = 0;
            if (comment.Comments != null && comment.Comments.Any(c => c.IsDeleted == false))
            {
                comment.IsDeleted = true;
                var updateResult = await _rootCommentRepository.UpdateByIdAsync(id, comment);
                deletedCount = Convert.ToInt32(updateResult.ModifiedCount);
            }
            else
            {
                var deleteResult = await _rootCommentRepository.DeleteByIdAsync(id);
                deletedCount = Convert.ToInt32(deleteResult.DeletedCount);
            }

            await UpdateAIRating(comment.BookId);

            return deletedCount;
        }

        public async Task<int> Update(RootUpdateDto updateDto)
        {
            var updateResult = await _rootCommentRepository.UpdateByIdAsync(
                updateDto.Id, 
                new BookRootComment() 
                { 
                    Text = updateDto.Text, 
                    PredictedRating = await _sentimentAnalisysService.Predict(updateDto.Text),
                    Date = DateTime.UtcNow.ToString("O")
                });

            var comment = await _rootCommentRepository.FindByIdAsync(updateDto.Id);
            await UpdateAIRating(comment.BookId);

            return Convert.ToInt32(updateResult.ModifiedCount);
        }

    }
}
