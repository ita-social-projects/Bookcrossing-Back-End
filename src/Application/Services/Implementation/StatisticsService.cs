using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Dto;
using Application.Dto.Statistics;
using Application.Services.Interfaces;
using Domain.RDBMS;
using Domain.RDBMS.Entities;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace Application.Services.Implementation
{
    public class StatisticsService : IStatisticsService
    {
        /// <summary>
        /// Requirements that says "shows top 5 book genres"
        /// </summary>
        private const int PieChartDataLimit = 5;
        private readonly IBookService _bookService;
        private readonly IRepository<Genre> _genreRepository;
        private readonly IRepository<Location> _locationRepository;
        private readonly IRepository<Book> _bookRepository;

        public StatisticsService(
            IBookService bookService,
            IRepository<Genre> genreRepository,
            IRepository<Location> locationRepository,
            IRepository<Book> bookRepository
        )
        {
            _bookService = bookService;
            _genreRepository = genreRepository;
            _locationRepository = locationRepository;
            _bookRepository = bookRepository;
        }

        public async Task<PieChartData> GetUserDonationsData(DateTime from, string language = "en")
        {
            var registeredBooks = (await _bookService.GetRegisteredAsync())
                .Where(b => b.DateAdded >= from)
                .Include(b => b.BookGenre)
                .ThenInclude(b => b.Genre);

            return TransformBookGenresToPieData(registeredBooks, language);
        }

        public PieChartData GetUserReadData(string language)
        {
            var readBooks = _bookService.GetAlreadyReadBooks();
            
            return TransformBookGenresToPieData(readBooks, language);
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

        public async Task<StatisticsChartData> GetReadingStatisticsData(StatisticsQueryParams query)
        {
            var transitions = _bookService.GetBooksTransitions();

            if (query.Cities?.Length > 0)
            {
                transitions = transitions.Where(r => query.Cities.Contains(r.User.UserRoom.Location.City));
            }

            if (query.Offices?.Length > 0)
            {
                transitions = transitions.Where(r => query.Offices.Contains(r.User.UserRoom.Location.OfficeName));
            }

            if (query.From.HasValue)
            {
                transitions = transitions.Where(r => r.RequestDate >= query.From.Value);
            }

            if (query.To.HasValue)
            {
                transitions = transitions.Where(r => r.RequestDate <= query.To.Value);
            }

            if (query.Genres?.Length > 0)
            {
                var predicate = PredicateBuilder.New<Request>();
                foreach (var id in query.Genres)
                {
                    var tempId = id;
                    predicate = predicate.Or(g => g.Book.BookGenre.Any(g => g.Genre.Id == tempId));
                }

                transitions = transitions.Where(predicate);
            }

            DateTime from;
            if (query.From.HasValue)
            {
                from = query.From.Value;
            }
            else
            {
                try
                {
                    from = transitions.Min(t => t.RequestDate);
                }
                catch (Exception)
                {
                    from = DateTime.Today;
                }
            }
            DateTime to = query.To ?? DateTime.Today;
            var genres = query.Genres?.Length > 0
                ? _genreRepository.GetAll().Where(g => query.Genres.Contains(g.Id)).Select(g => query.language == "en" ? g.Name : g.NameUk).ToList()
                : _genreRepository.GetAll().Select(g => query.language == "en" ? g.Name : g.NameUk).ToList();


            var data = new Dictionary<string, List<int>>();
            var periods = new List<string>();

            if (to.Year - from.Year >= 2) // more than 2 years (years)
            {
                foreach (var genre in genres)
                {
                    List<int> counts = new List<int>();


                    for (var currentYear = from.Year; currentYear < to.AddYears(1).Year; currentYear++)
                    {
                        if (!periods.Contains(currentYear.ToString()))
                        {
                            periods.Add(currentYear.ToString());
                        }

                        counts.Add(transitions
                            .Where(r => r.RequestDate.Year == currentYear)
                            .Count(r => r.Book.BookGenre.Exists(g => query.language == "en" ? g.Genre.Name == genre : g.Genre.NameUk == genre))
                        );
                    }

                    data.Add(genre, counts);
                }
            }
            else if (from.AddMonths(1) < to) // more then month (months)
            {
                foreach (var genre in genres)
                {
                    List<int> counts = new List<int>();

                    DateTime current = from;
                    for (; current <= to; current = current.AddMonths(1))
                    {
                        if (current.Year == to.Year && current.Month == to.Month && current < to)
                        {
                            current = to;
                        }

                        if (!periods.Contains(current.ToString("dd-MM-yyyy")))
                        {
                            periods.Add(current.ToString("dd-MM-yyyy"));
                        }

                        counts.Add(transitions
                            .Where(r => r.RequestDate.Year == current.Year && r.RequestDate.Month == current.Month)
                            .Count(r => r.Book.BookGenre.Exists(g => query.language == "en" ? g.Genre.Name == genre : g.Genre.NameUk == genre))
                        );
                    }

                    if (current.Month == to.Month && current > to)
                    {
                        if (!periods.Contains(to.ToString("dd-MM-yyyy")))
                        {
                            periods.Add(to.ToString("dd-MM-yyyy"));
                        }

                        counts.Add(transitions
                            .Where(r => r.RequestDate.Year == to.Year && r.RequestDate.Month == to.Month)
                            .Count(r => r.Book.BookGenre.Exists(g => query.language == "en" ? g.Genre.Name == genre : g.Genre.NameUk == genre))
                        );
                    }

                    data.Add(genre, counts);
                }
            }
            else // less than month (days)
            {
                foreach (var genre in genres)
                {
                    List<int> counts = new List<int>();
                    for (var current = from; current <= to; current = current.AddDays(1))
                    {
                        if (!periods.Contains(current.ToString("dd-MM-yyyy")))
                        {
                            periods.Add(current.ToString("dd-MM-yyyy"));
                        }

                        counts.Add(transitions
                            .Where(r => r.RequestDate.Date == current.Date)
                            .Count(r => r.Book.BookGenre.Exists(g => query.language == "en" ? g.Genre.Name == genre : g.Genre.NameUk == genre))
                        );
                    }

                    data.Add(genre, counts);
                }
            }

            return new StatisticsChartData(periods, data);
        }

        public async Task<StatisticsChartData> GetDonationStatisticsData(StatisticsQueryParams query)
        {
            var books = _bookRepository.GetAll();
            if (query.Cities?.Length > 0)
            {
                books = books.Where(b => query.Cities.Contains(b.User.UserRoom.Location.City));
            }

            if (query.Offices?.Length > 0)
            {
                books = books.Where(b => query.Offices.Contains(b.User.UserRoom.Location.OfficeName));
            }

            if (query.From.HasValue)
            {
                books = books.Where(b => b.DateAdded >= query.From.Value);
            }

            if (query.To.HasValue)
            {
                books = books.Where(b => b.DateAdded <= query.To.Value);
            }

            if (query.Genres?.Length > 0)
            {
                var predicate = PredicateBuilder.New<Book>();
                foreach (var id in query.Genres)
                {
                    var tempId = id;
                    predicate = predicate.Or(b => b.BookGenre.Any(g => g.Genre.Id == tempId));
                }

                books = books.Where(predicate);
            }


            DateTime from;
            if (query.From.HasValue)
            {
                from = query.From.Value;
            }
            else
            {
                try
                {
                    from = books.Min(t => t.DateAdded);
                }
                catch (Exception)
                {
                    from = DateTime.Today;
                }
            }
            DateTime to = query.To ?? DateTime.Today;
            var cities = query.Cities?.Length > 0
                ? _locationRepository.GetAll().Where(l => query.Cities.Contains(l.City)).Select(l => l.City).Distinct()
                : _locationRepository.GetAll().Select(l => l.City).Distinct();

            var data = new Dictionary<string, List<int>>();
            var periods = new List<string>();

            if (to.Year - from.Year >= 2) // more than 2 years (years)
            {
                foreach (var city in cities)
                {
                    List<int> counts = new List<int>();
                    for (var currentYear = from.Year; currentYear < to.AddYears(1).Year; currentYear++)
                    {
                        if (!periods.Contains(currentYear.ToString()))
                        {
                            periods.Add(currentYear.ToString());
                        }

                        counts.Add(books
                            .Where(b => b.DateAdded.Year == currentYear)
                            .Count(b => b.User.UserRoom.Location.City == city)
                        );
                    }

                    data.Add(city, counts);
                }
            }
            else if (from.AddMonths(1) < to) // more then month (months)
            {
                foreach (var city in cities)
                {
                    List<int> counts = new List<int>();

                    DateTime current = from;
                    for (; current <= to; current = current.AddMonths(1))
                    {
                        if (current.Year == to.Year && current.Month == to.Month && current < to)
                        {
                            current = to;
                        }

                        if (!periods.Contains(current.ToString("dd-MM-yyyy")))
                        {
                            periods.Add(current.ToString("dd-MM-yyyy"));
                        }

                        counts.Add(books
                            .Where(b => b.DateAdded.Year == current.Year && b.DateAdded.Month == current.Month)
                            .Count(b => b.User.UserRoom.Location.City == city)
                        );
                    }

                    if (current.Month == to.Month && current > to)
                    {
                        if (!periods.Contains(to.ToString("dd-MM-yyyy")))
                        {
                            periods.Add(to.ToString("dd-MM-yyyy"));
                        }

                        counts.Add(books
                            .Where(b => b.DateAdded.Year == current.Year && b.DateAdded.Month == current.Month)
                            .Count(b => b.User.UserRoom.Location.City == city)
                        );
                    }

                    data.Add(city, counts);
                }
            }
            else // less than month (days)
            {
                foreach (var city in cities)
                {
                    List<int> counts = new List<int>();
                    for (var current = from; current <= to; current = current.AddDays(1))
                    {
                        if (!periods.Contains(current.ToString("dd-MM-yyyy")))
                        {
                            periods.Add(current.ToString("dd-MM-yyyy"));
                        }

                        counts.Add(books
                            .Where(b => b.DateAdded.Date == current.Date)
                            .Count(b => b.User.UserRoom.Location.City == city)
                        );
                    }

                    data.Add(city, counts);
                }
            }

            return new StatisticsChartData(periods, data);
        }

        protected PieChartData TransformBookGenresToPieData(IQueryable<Book> books, string language)
        {
            var booksGenres = books.SelectMany(
                b => b.BookGenre,
                (b, g) => new { Book = b, Genre = g.Genre });

            var sortedGroups = booksGenres.GroupBy(b => language == "en" ? b.Genre.Name : b.Genre.NameUk)
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
