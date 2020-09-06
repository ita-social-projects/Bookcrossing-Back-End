using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Dto.Statistics;
using Application.Services.Interfaces;
using Domain.RDBMS.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementation
{
    public class StatisticsService : IStatisticsService
    {
        /// <summary>
        /// Requirements that says "shows top 5 book genres"
        /// </summary>
        private const int PieChartDataLimit = 5;
        private readonly IBookService _bookService;

        public StatisticsService(IBookService bookService)
        {
            _bookService = bookService;
        }

        public async Task<PieChartData> GetUserDonationsData(DateTime from)
        {
            var registeredBooks = (await _bookService.GetRegisteredAsync())
                .Where(b => b.DateAdded >= from)
                .Include(b => b.BookGenre)
                .ThenInclude(b => b.Genre);

            return TransformBookGenresToPieData(registeredBooks);
        }

        public PieChartData GetUserReadData()
        {
            var readBooks = _bookService.GetAlreadyReadBooks();
            
            return TransformBookGenresToPieData(readBooks);
        }

        public PieChartData GetUserMostReadLanguagesData()
        {
            var readBooks = _bookService.GetAlreadyReadBooks()
                .Union(_bookService.GetBooksInReadStatus());

            var sortedGroups = readBooks
                .GroupBy(b => b.Language.Name)
                .Select(b => new {Language = b.Key, Count = b.Count()})
                .OrderByDescending(g => g.Count)
                .ToList();

            if (sortedGroups.Count() > PieChartDataLimit)
            {
                var lastElementCount = sortedGroups.ElementAt(PieChartDataLimit - 1).Count;
                var topBooks = sortedGroups
                    .Where(b => b.Count >= lastElementCount)
                    .ToList();
                int othersCount = sortedGroups.Skip(topBooks.Count()).Sum(b => b.Count);
                
                topBooks.Add(new { Language = "Others", Count = othersCount });
                sortedGroups = topBooks;
            }

            var topDictionary = sortedGroups
                .ToDictionary(g => g.Language, c => c.Count);

            return new PieChartData(sortedGroups.Count(), topDictionary);
        }

        protected PieChartData TransformBookGenresToPieData(IQueryable<Book> books)
        {
            var booksGenres = books.SelectMany(
                b => b.BookGenre,
                (b, g) => new { Book = b, Genre = g.Genre });
            var sortedGroups = booksGenres.GroupBy(b => b.Genre.Name)
                .Select(b => new { Genre = b.Key, Count = b.Count() })
                .OrderByDescending(g => g.Count)
                .ToList();

            if (sortedGroups.Count > PieChartDataLimit)
            {
                var lastElementCount = sortedGroups.ElementAt(PieChartDataLimit - 1).Count;
                var topBooks = sortedGroups
                    .Where(b => b.Count >= lastElementCount)
                    .ToList();
                int othersCount = sortedGroups.Skip(topBooks.Count()).Sum(b => b.Count);
                topBooks.Add(new { Genre = "Others", Count = othersCount });
                sortedGroups = topBooks;
            }

            var topDictionary = sortedGroups
                .ToDictionary(g => g.Genre, c => c.Count);

            return new PieChartData(books.Count(), topDictionary);
        }
    }
}
