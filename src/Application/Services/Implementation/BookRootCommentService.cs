using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Dto.Comment.Book;
using Application.Services.Interfaces;
using Domain.NoSQL;
using Domain.NoSQL.Entities;
using Domain.RDBMS;
using Domain.RDBMS.Entities;

namespace Application.Services.Implementation
{
    public class BookRootCommentService : IBookRootCommentService
    {
        private readonly IRootRepository<BookRootComment> _rootCommentRepository;
        private readonly ICommentOwnerMapper _commentOwnerMapper;
        private readonly IRepository<Book> _bookRepository;
        public BookRootCommentService(IRootRepository<BookRootComment> rootCommentRepository, ICommentOwnerMapper commentOwnerMapper,
            IRepository<Book> bookRepository)
        {
            _rootCommentRepository = rootCommentRepository;
            _commentOwnerMapper = commentOwnerMapper;
            _bookRepository = bookRepository;
        }

        public async Task<int> Add(RootInsertDto insertDto)
        {
            var comment =  await _rootCommentRepository.InsertOneAsync(
                new BookRootComment(true) 
                {
                    Text = insertDto.Text,
                    BookId = insertDto.BookId,
                    OwnerId = insertDto.OwnerId,
                    Date = DateTime.Now.ToUniversalTime().ToString(),
                });
            
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

            return deletedCount;
        }

        public async Task<int> Update(RootUpdateDto updateDto)
        {
            var updateResult = await _rootCommentRepository.UpdateByIdAsync(updateDto.Id, new BookRootComment() { Text = updateDto.Text });
            var comment = await _rootCommentRepository.FindByIdAsync(updateDto.Id);

            return Convert.ToInt32(updateResult.ModifiedCount);
        }

    }
}
