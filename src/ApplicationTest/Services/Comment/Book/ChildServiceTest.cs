using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Application.Dto.Comment.Book;
using Application.Services.Implementation;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.NoSQL;
using Domain.NoSQL.Entities;
using Domain.RDBMS;
using FluentAssertions;
using Microsoft.CodeAnalysis.Differencing;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;

namespace ApplicationTest.Services.Comment.Book
{
    [TestFixture]
    class ChildServiceTest
    {
        private IBookChildCommentService _bookChildCommentService;
        private Mock<IChildRepository<BookRootComment, BookChildComment>> _childRepository;
        private Mock<IRepository<Domain.RDBMS.Entities.Book>> _bookRepository;
        private Mock<IBookRootCommentService> _rootRepository;
        private Mock<IRootRepository<BookRootComment>> _rootCommentRepository;
        private Mock<IMapper> _mapper;
        private BookChildCommentServiceProxy _bookChildCommentServiceProxy;
        private Mock<ISentimentAnalisysService> _sentimentService;

        #region DummyData
        private const float _predictedRationAvarage = 1f;
        public IEnumerable<IBookComment> GetCommentTree()
        {
            return new List<BookRootComment>()
            {
                new BookRootComment()
                {
                    PredictedRating = 1f,
                    Comments = new List<BookChildComment>()
                    {
                        new BookChildComment()
                        {
                            PredictedRating = 1f,
                            Comments = Enumerable.Empty<BookChildComment>()
                        },
                        new BookChildComment()
                        {
                            PredictedRating = 1f,
                            Comments = Enumerable.Empty<BookChildComment>()
                        }
                    }
                },
                new BookRootComment()
                {
                    PredictedRating = 1f,
                    Comments = new List<BookChildComment>()
                    {
                        new BookChildComment()
                        {
                            PredictedRating = 1f,
                            Comments = Enumerable.Empty<BookChildComment>()
                        }
                    }
                },
                new BookRootComment()
                {
                    PredictedRating = 1f,
                    Comments = new List<BookChildComment>()
                    {
                        new BookChildComment()
                        {
                            PredictedRating = 1f,
                            Comments = new List<BookChildComment>()
                            {
                                new BookChildComment()
                                {
                                    PredictedRating = 1f,
                                    Comments = Enumerable.Empty<BookChildComment>()
                                }
                            }
                        }
                    }
                }
            };
        }
        #endregion
        public void SetupMocks()
        {
            var bookRootComments = new List<BookRootComment>()
            {
                new BookRootComment(){ IsDeleted = false, PredictedRating = 5 },
                new BookRootComment(){ IsDeleted = false, PredictedRating = 1}
            }.AsEnumerable();
            var comment = new BookRootComment() { Id = It.IsAny<string>() };
            _rootCommentRepository.Setup(m => m.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(comment);
            _bookRepository.Setup(m => m.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(It.IsAny<Domain.RDBMS.Entities.Book>());
            _rootCommentRepository.Setup(m => m.FindManyAsync(root => root.BookId == It.IsAny<int>())).ReturnsAsync(bookRootComments);
        }

        [SetUp]
        public void Setup()
        {
            _childRepository = new Mock<IChildRepository<BookRootComment, BookChildComment>>();
            _rootRepository = new Mock<IBookRootCommentService>();
            _mapper = new Mock<IMapper>();
            _bookRepository = new Mock<IRepository<Domain.RDBMS.Entities.Book>>();
            _sentimentService = new Mock<ISentimentAnalisysService>();
            _rootCommentRepository = new Mock<IRootRepository<BookRootComment>>();

            _bookChildCommentService = new BookChildCommentService(_childRepository.Object, _rootRepository.Object, 
                _mapper.Object, _bookRepository.Object, _sentimentService.Object,
                 _rootCommentRepository.Object);

            _bookChildCommentServiceProxy = new BookChildCommentServiceProxy(
                _childRepository.Object,
                _rootRepository.Object,
                _mapper.Object,
                _sentimentService.Object,
                _rootCommentRepository.Object,
                _bookRepository.Object
            );

            SetupMocks();
        }

        #region Update

        [Test]
        [TestCase(1, "5e9c9ee859231a63bc853bf0", 1)]
        [TestCase(0, null, 0)]
        public async Task UpdateComment_BookChildCommentExistsAndNot_ReturnsNumberOfUpdatedComments(int updateValue, string commentId, int expectedResult)
        {
            ChildUpdateDto updateDto = new ChildUpdateDto()
            {
                Ids = new List<string>() { "5e9c9ee859231a63bc853bf0", "5e9c9ee859231a63bc853bf1" },
            };
            List<(string, string)> path = new List<(string, string)>() { ("Comments", "5e9c9ee859231a63bc853bf1") };

            _rootRepository.Setup(m => m.GetById(It.IsAny<string>()))
                .ReturnsAsync(new RootDto()
                {
                    Comments = new List<ChildDto>()
                    {
                        new ChildDto() { Id = "5e9c9ee859231a63bc853bf1", Comments = new List<ChildDto>() },
                    }
                });


            _childRepository
                .Setup(s => s.SetAsync(
                    "5e9c9ee859231a63bc853bf0",
                    It.IsAny<BookChildComment>(),
                    path))
                .ReturnsAsync(new UpdateResult.Acknowledged(updateValue, updateValue, commentId));

            var result = await _bookChildCommentService.Update(updateDto);

            result.Should().Be(expectedResult);
        }

        #endregion

        #region Remove

        [Test]
        [TestCase(1, "5e9c9ee859231a63bc853bf0", 1)]
        [TestCase(0, null, 0)]
        public async Task RemoveComment_BookChildCommentExistsAndNot_ReturnsNumberOfRemovedComments(int updateValue, string commentId, int expectedResult)
        {
            ChildDeleteDto deleteDto = new ChildDeleteDto()
            {
                Ids = new List<string>() { "5e9c9ee859231a63bc853bf0", "5e9c9ee859231a63bc853bf1" },
            };

            _rootRepository.Setup(m => m.GetById(It.IsAny<string>()))
                .ReturnsAsync(new RootDto() { Comments = new List<ChildDto>() });

            _childRepository
                .Setup(s => s.PullAsync(
                    "5e9c9ee859231a63bc853bf0",
                    "5e9c9ee859231a63bc853bf1",
                    new List<(string, string)>(),
                    "Comments"))
                .ReturnsAsync(new UpdateResult.Acknowledged(updateValue, updateValue, commentId));

            var result = await _bookChildCommentService.Remove(deleteDto.Ids);

            result.Should().Be(expectedResult);
        }

        #endregion

        #region Add

        [Test]
        [TestCase(1, "5e9c9ee859231a63bc853bf0", 1)]
        [TestCase(0, null, 0)]
        public async Task AddComment_BookChildComment_ReturnsNumberOfAddedComments(int updateValue, string commentId, int expectedResult)
        {
            ChildInsertDto insertDto = new ChildInsertDto()
            {
                Ids = new List<string>() { "5e9c9ee859231a63bc853bf0" },
            };
            _childRepository
                .Setup(s => s.PushAsync(
                    "5e9c9ee859231a63bc853bf0",
                    It.IsAny<BookChildComment>(),
                    new List<(string, string)>(),
                    "Comments"))
                .ReturnsAsync(new UpdateResult.Acknowledged(updateValue, updateValue, commentId));

            var result = await _bookChildCommentService.Add(insertDto);

            result.Should().Be(expectedResult);
        }

        #endregion

        #region OtherFunctions
        [Test]
        public async Task SetAsDeleted_ReturnsNumberOfUpdatedObjects()
        {
            var bookChildCommentServiceMock = new BookChildCommentServiceProxy(
                _childRepository.Object,
                _rootRepository.Object,
                _mapper.Object,
                _sentimentService.Object,
                _rootCommentRepository.Object,
                _bookRepository.Object);

            UpdateResult updateResult = new UpdateResult.Acknowledged(1, 1, 1);

            _childRepository.Setup(m => m.SetAsync(
                    It.IsAny<string>(),
                    It.IsAny<BookChildComment>(),
                    It.IsAny<IEnumerable<(string, string)>>()))
                .ReturnsAsync(updateResult);

            var result = await bookChildCommentServiceMock.SetAsDeletedProxy(
                new List<string> { "5e9c9ee859231a63bc853bf1", "5e9c9ee859231a63bc853bf2" },
                new ChildDto()
                {
                    Comments = new List<ChildDto>()
                {
                    new ChildDto() {Id = "5e9c9ee859231a63bc85dt2w"}
                }
                },
                "5e9c9ee859231a63bc853bf0");

            result.Should().Be((int)updateResult.ModifiedCount);
        }

        [Test]
        public async Task Delete_RootCommentIsDeletedAndDoesntContainActiveSubcomments_ReturnsNumberOfUpdatedObjects()
        {
            var bookChildCommentServiceMock = new BookChildCommentServiceProxy(
                _childRepository.Object,
                _rootRepository.Object,
                _mapper.Object,
                _sentimentService.Object,
                _rootCommentRepository.Object,
                _bookRepository.Object
                );
            UpdateResult updateResult = new UpdateResult.Acknowledged(1, 1, 1);

            _childRepository.Setup(m => m.PullAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<(string, string)>>(),
                    It.IsAny<string>()))
                .ReturnsAsync(updateResult);

            _rootRepository.Setup(m => m.GetById(It.IsAny<string>()))
                .ReturnsAsync(new RootDto() { IsDeleted = true, Comments = new List<ChildDto>() });

            var result = await bookChildCommentServiceMock.DeleteProxy(
                new List<string> { "5e9c9ee859231a63bc853bf1", "5e9c9ee859231a63bc853bf2", "5e9c9ee859231a63bc853bf3" },
                "5e9c9ee859231a63bc853bf0",
                "5e9c9ee859231a63bc853bf1");

            _rootRepository.Verify(m => m.Remove(It.IsAny<string>()), Times.Once());
            result.Should().Be((int)updateResult.ModifiedCount);
        }

        [Test]
        public void HasActiveComments_HasAnyActiveSubComments_ReturnTrue()
        {
            List<ChildDto> childDtos = new List<ChildDto>()
            {
                new ChildDto() {IsDeleted = true, Comments = new List<ChildDto>()},
                new ChildDto() {IsDeleted = true, Comments = new List<ChildDto>()
                {
                    new ChildDto() {Comments = new List<ChildDto>()}
                }}
            };

            var result = _bookChildCommentServiceProxy.HasActiveCommentsProxy(childDtos);

            result.Should().Be(true);
        }


        [Test]
        public void HasActiveComments_HasNotAnyActiveSubComments_ReturnFalse()
        {
            List<ChildDto> childDtos = new List<ChildDto>()
            {
                new ChildDto() {IsDeleted = true, Comments = new List<ChildDto>()},
                new ChildDto() {IsDeleted = true, Comments = new List<ChildDto>()
                {
                    new ChildDto() {IsDeleted = true, Comments = new List<ChildDto>()}
                }}
            };

            var result = _bookChildCommentServiceProxy.HasActiveCommentsProxy(childDtos);

            result.Should().Be(false);
        }

        [Test]
        [TestCase("1", true)]
        [TestCase("4", true)]
        [TestCase("5", false)]
        [TestCase("6", true)]
        [TestCase("8", false)]
        public async Task FindChild_WasFounded_ReturnChildDto(string searchChildId, bool found)
        {
            var children = new List<ChildDto>
            {
                new ChildDto() {Id = "1", Comments = new List<ChildDto>()},
                new ChildDto() {Id = "2", Comments = new List<ChildDto>()
                {
                    new ChildDto() {Id = "4", Comments = new List<ChildDto>()}
                }},
                new ChildDto() {Id = "3", Comments = new List<ChildDto>()
                {
                    new ChildDto() {Id = "6", Comments = new List<ChildDto>()
                    {
                        new ChildDto() {Id = "7", Comments = new List<ChildDto>()}
                    }}
                }},
            };

            var result = await _bookChildCommentServiceProxy.FindChildProxy(children, searchChildId);

            (result != null).Should().Be(found);
        }
        #endregion


        [Test]
        public void GetAvaragePredictedRating_ModelValid_ReturnsAvarage()
        {
            var expected = _predictedRationAvarage;

            var actual = _bookChildCommentService.GetAvaragePredictedRating(GetCommentTree());

            actual.Should().Be(expected);
        }

        private class BookChildCommentServiceProxy : BookChildCommentService
        {
            public BookChildCommentServiceProxy(
                IChildRepository<BookRootComment, BookChildComment> childCommentRepository,
                IBookRootCommentService bookRootCommentService,
                IMapper mapper,
                ISentimentAnalisysService sentimentAnalisys,
                IRootRepository<BookRootComment> rootRepository,
                IRepository<Domain.RDBMS.Entities.Book> repositoryBook
            ) : base(childCommentRepository, bookRootCommentService, mapper, repositoryBook, sentimentAnalisys, rootRepository)
            {
            }

            public async Task<int> SetAsDeletedProxy(IEnumerable<string> ids, ChildDto childComment, string rootId)
            {
                return await base.SetAsDeleted(ids, childComment, rootId);
            }

            public async Task<int> DeleteProxy(IEnumerable<string> ids, string rootId, string childId)
            {
                return await base.Delete(ids, rootId, childId);
            }

            public bool HasActiveCommentsProxy(IEnumerable<ChildDto> children)
            {
                return base.HasActiveComments(children);
            }

            public Task<ChildDto> FindChildProxy(IEnumerable<ChildDto> children, string childId)
            {
                return base.FindChild(children, childId);
            }
        }
    }
}