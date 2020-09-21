using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Services.Implementation;
using Application.Services.Interfaces;
using Domain.RDBMS;
using Domain.RDBMS.Entities;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace ApplicationTest.Services
{
    [TestFixture]
    internal class StatisticsServiceTest
    {
        /// <summary>
        /// Requirements that says "shows top 5 book genres"
        /// </summary>
        private const int PieChartDataLimit = 5;

        private IStatisticsService _statisticsService;
        private Mock<IBookService> _bookServiceMock;
        private Mock<IRepository<Genre>> _genreRepositoryMock;
        private Mock<IRepository<Location>> _locationRepositoryMock;
        private Mock<IRepository<Book>> _bookRepositoryMock;

        private List<Book> _books;
        private List<Book> _bookInReadStatus;
        private List<BookGenre> _bookGenres;
        private int _others = 1;

        public StatisticsServiceTest()
        {
            _bookServiceMock = new Mock<IBookService>();
            _genreRepositoryMock = new Mock<IRepository<Genre>>();
            _locationRepositoryMock = new Mock<IRepository<Location>>();
            _bookRepositoryMock = new Mock<IRepository<Book>>();
            _statisticsService = new StatisticsService(
                _bookServiceMock.Object,
                _genreRepositoryMock.Object,
                _locationRepositoryMock.Object,
                _bookRepositoryMock.Object
            );

            MockData();
        }

        [Test]
        public void GetUserReadData_ReturnsPieChartDataObject()
        {
            _bookServiceMock.Setup(m => m.GetAlreadyReadBooks())
                .Returns(_books.AsQueryable);
            var allGenres = _books.SelectMany(
                    b => b.BookGenre,
                    (b, g) => new {Book = b, Genre = g.Genre})
                .GroupBy(b => b.Genre);
            var expectedResult = allGenres.Count() <= PieChartDataLimit ? allGenres.Count() : PieChartDataLimit + _others;

            var result = _statisticsService.GetUserReadData();

            result.Total.Should().Be(expectedResult);
        }

        [Test]
        public async Task GetUserDonationsData_ReturnsPieChartDataObject()
        {
            _bookServiceMock.Setup(m => m.GetRegisteredAsync())
                .ReturnsAsync(_books.AsQueryable());
            var from = new DateTime(2020, 1, 1);
            var expectedResult = _books.Count(b => b.DateAdded >= from);

            var result = await _statisticsService.GetUserDonationsData(from);

            result.Total.Should().Be(expectedResult);
        }

        [Test]
        public void GetUserMostReadLanguagesData_ReturnsPieChartDataObject()
        {
            _bookServiceMock.Setup(m => m.GetAlreadyReadBooks())
                .Returns(_books.AsQueryable);
            _bookServiceMock.Setup(m => m.GetBooksInReadStatus())
                .Returns(_bookInReadStatus.AsQueryable);
            var languages = _books.Union(_bookInReadStatus)
                .GroupBy(b => b.Language.Name);
            var expectedResult = languages.Count() <= PieChartDataLimit ? languages.Count() : languages.Count() + _others;

            var result = _statisticsService.GetUserMostReadLanguagesData();

            result.Total.Should().Be(expectedResult);
        }

        private void MockData()
        {
            _bookGenres = new List<BookGenre>()
            {
                new BookGenre()
                {
                    GenreId = 1,
                    Genre = new Genre() {Id = 1, Name = "Adventure"}
                },
                new BookGenre()
                {
                    GenreId = 2,
                    Genre = new Genre() {Id = 2, Name = "History"}
                },
                new BookGenre()
                {
                    GenreId = 3,
                    Genre = new Genre() {Id = 3, Name = "Romantic novel"}
                },
                new BookGenre()
                {
                    GenreId = 4,
                    Genre = new Genre() {Id = 3, Name = "Action"}
                },
                new BookGenre()
                {
                    GenreId = 5,
                    Genre = new Genre() {Id = 3, Name = "Thriller"}
                },
                new BookGenre()
                {
                    GenreId = 6,
                    Genre = new Genre() {Id = 3, Name = "Drama"}
                }
            };


            _bookInReadStatus = new List<Book>()
            {
                new Book()
                {
                    Id = 404,
                    DateAdded = new DateTime(1999, 6, 9),
                    BookGenre = _bookGenres,
                    State = BookState.Reading,
                    Language = new Language() {Id = 1, Name = "English"}
                },
                new Book()
                {
                    Id = 200,
                    DateAdded = new DateTime(2008, 9, 1),
                    BookGenre = _bookGenres,
                    State = BookState.Reading,
                    Language = new Language() {Id = 1, Name = "Italian"}
                },
                new Book()
                {
                    Id = 500,
                    DateAdded = new DateTime(1996, 2, 28),
                    BookGenre = _bookGenres,
                    State = BookState.Reading,
                    Language = new Language() {Id = 1, Name = "Germany"}
                }
            };

            _books = new List<Book>()
            {
                new Book()
                {
                    Id = 1,
                    DateAdded = new DateTime(2020, 8, 10),
                    BookGenre = _bookGenres,
                    Language = new Language() {Id = 1, Name = "English"}
                },
                new Book()
                {
                    Id = 2,
                    DateAdded = new DateTime(2020, 1, 4),
                    BookGenre = _bookGenres,
                    Language = new Language() {Id = 1, Name = "English"}
                },
                new Book()
                {
                    Id = 3,
                    DateAdded = new DateTime(2020, 3, 21),
                    BookGenre = _bookGenres,
                    Language = new Language() {Id = 1, Name = "English"}
                },
                new Book()
                {
                    Id = 4,
                    DateAdded = new DateTime(2020, 5,3),
                    BookGenre = _bookGenres,
                    Language = new Language() {Id = 2, Name = "Ukrainian"}
                },
                new Book()
                {
                    Id = 5,
                    DateAdded = new DateTime(2020, 9, 4),
                    BookGenre = _bookGenres,
                    Language = new Language() {Id = 3, Name = "Russian"}
                },
                new Book()
                {
                    Id = 6,
                    DateAdded = new DateTime(2020, 3, 21),
                    BookGenre = _bookGenres,
                    Language = new Language() {Id = 4, Name = "Poland"}
                }
            };
        }
    }
}
