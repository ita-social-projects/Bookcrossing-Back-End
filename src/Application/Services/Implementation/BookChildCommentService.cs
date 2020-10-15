using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Application.Dto.Comment.Book;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.NoSQL;
using Domain.NoSQL.Entities;
using Domain.RDBMS;
using Domain.RDBMS.Entities;
using MongoDB.Driver;

namespace Application.Services.Implementation
{
    public class BookChildCommentService : IBookChildCommentService
    {
        private readonly IChildRepository<BookRootComment, BookChildComment> _childCommentRepository;
        private readonly IRootRepository<BookRootComment> _rootCommentRepository;
        private readonly IBookRootCommentService _bookRootCommentService;
        private readonly IMapper _mapper;
        private readonly IRepository<Book> _bookRepository;
        private readonly ISentimentAnalisysService _sentimentAnalisysService;

        public BookChildCommentService(IChildRepository<BookRootComment, BookChildComment> childCommentRepository,
            IBookRootCommentService bookRootCommentService,
            IMapper mapper, IRepository<Book> bookRepository,
            ISentimentAnalisysService sentimentAnalisysService,
            IRootRepository<BookRootComment> rootCommentRepository)
        {
            _sentimentAnalisysService = sentimentAnalisysService;
            _childCommentRepository = childCommentRepository;
            _bookRootCommentService = bookRootCommentService;
            _rootCommentRepository = rootCommentRepository;
            _bookRepository = bookRepository;
            _mapper = mapper;
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
        public async Task UpdateAIRating(string rootId)
        {
            var comment = await _rootCommentRepository.FindByIdAsync(rootId);
            var book = await _bookRepository.FindByIdAsync(comment.BookId);
            var comments = await _rootCommentRepository.FindManyAsync(root => root.BookId == book.Id);
            if (comments.Any())
            {
                book.PredictedRating = GetAvaragePredictedRating(comments);
                _bookRepository.Update(book);
                await _bookRepository.SaveChangesAsync();
            }
        }
        public async Task<int> Add(ChildInsertDto insertDto)
        {
            string rootId = insertDto.Ids.First();
            List<(string nestedArrayName, string itemId)> path = insertDto.Ids.Skip(1).Select(x => ("Comments", x)).ToList();

            var updateResult = await _childCommentRepository.PushAsync(
                rootId,
                new BookChildComment(true)
                {
                    Text = insertDto.Text,
                    OwnerId = insertDto.OwnerId,
                    Date = DateTime.UtcNow.ToString("O"),
                    PredictedRating = await _sentimentAnalisysService.Predict(insertDto.Text)
                },
                path,
                "Comments");

            await UpdateAIRating(rootId);

            return Convert.ToInt32(updateResult.ModifiedCount);
        }

        public async Task<int> Remove(IEnumerable<string> ids)
        {
            string rootId = ids.First();
            string childId = ids.Last();
            var rootComment = await _bookRootCommentService.GetById(rootId);
            var childComment = await FindChild(rootComment.Comments, childId);
            if (childComment?.Comments?.Any() == true)
            {
                return await SetAsDeleted(ids, childComment, rootId);
            }
            var result = await Delete(ids, rootId, childId);

            await UpdateAIRating(rootId);

            return result;
        }

        protected async Task<int> SetAsDeleted(IEnumerable<string> ids, ChildDto childComment, string rootId)
        {
            var children = childComment.Comments.Select(c => _mapper.Map<ChildDto, BookChildComment>(c)).ToList();
            var path = ids.Skip(1).Select(x => ("Comments", x)).ToList();
            var result = (int)(await _childCommentRepository.SetAsync(
                rootId,
                new BookChildComment() { IsDeleted = true, Text = childComment.Text, Comments = children,
                    Date = DateTime.UtcNow.ToString("O")
                },
                path
            )).ModifiedCount;

            await UpdateAIRating(rootId);

            return result;
        }

        protected async Task<int> Delete(IEnumerable<string> ids, string rootId, string childId)
        {
            var path = ids.Skip(1).SkipLast(1).Select(x => ("Comments", x)).ToList();
            UpdateResult updateResult = await _childCommentRepository.PullAsync(
                rootId,
                childId,
                path,
                "Comments");
            RootDto rootComment = await _bookRootCommentService.GetById(rootId);
            if (rootComment.IsDeleted && !HasActiveComments(rootComment.Comments))
            {
                await _bookRootCommentService.Remove(rootId);
            }
            else if (rootComment.Comments.Any())
            {
                await ClearCommentBranch(rootComment, ids.Skip(1).SkipLast(1));
            }

            await UpdateAIRating(rootId);

            return (int)updateResult.ModifiedCount;
        }

        public async Task<int> Update(ChildUpdateDto updateDto)
        {
            string rootId = updateDto.Ids.First();
            string childId = updateDto.Ids.Last();
            var rootComment = await _bookRootCommentService.GetById(rootId);
            if (rootComment == null)
            {
                return 0;
            }

            var childComment = await FindChild(rootComment.Comments, childId);
            if (childComment == null)
            {
                return 0;
            }

            var children = childComment.Comments.Select(c => _mapper.Map<ChildDto, BookChildComment>(c)).ToList();

            List<(string nestedArrayName, string itemId)> path = updateDto.Ids.Skip(1).Select(x => ("Comments", x)).ToList();

            var updateResult = await _childCommentRepository.SetAsync(
                rootId,
                new BookChildComment() { Text = updateDto.Text, Comments = children, Date = DateTime.UtcNow.ToString("O"),
                    PredictedRating = await _sentimentAnalisysService.Predict(updateDto.Text) },
                path);

            await UpdateAIRating(rootId);

            return Convert.ToInt32(updateResult.MatchedCount);
        }

        protected async Task<ChildDto> FindChild(IEnumerable<ChildDto> children, string childId)
        {
            var searchedChild = children.FirstOrDefault(c => c.Id == childId);
            if (searchedChild != null)
            {
                return searchedChild;
            }

            foreach (var child in children)
            {
                if (child.Comments.Any())
                {
                    var result = await FindChild(child.Comments, childId);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }

        protected async Task ClearCommentBranch(RootDto root, IEnumerable<string> ids)
        {
            var path = new List<(string nestedArrayName, string itemId)>();
            foreach (var id in ids)
            {
                var child = await FindChild(root.Comments, id);
                if (child.IsDeleted && !HasActiveComments(child.Comments))
                {
                    var rootId = root.Id;
                    var childId = child.Id;

                    await _childCommentRepository.PullAsync(
                        rootId,
                        childId,
                        path,
                        "Comments");

                    return;
                }

                path.Add(("Comments", id));
            }

            await UpdateAIRating(root.Id);
        }

        protected bool HasActiveComments(IEnumerable<ChildDto> children)
        {
            if (!children.Any())
            {
                return false;
            }

            if (children.Any(c => c.IsDeleted == false))
            {
                return true;
            }

            foreach (var child in children)
            {
                if (HasActiveComments(child.Comments))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
