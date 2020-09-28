using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Dto;
using Application.Dto.Email;
using Application.Dto.QueryParams;
using Application.QueryableExtension;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.NoSQL;
using Domain.NoSQL.Entities;
using Domain.RDBMS;
using Domain.RDBMS.Entities;
using Domain.RDBMS.Enums;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using MimeKit;

namespace Application.Services.Implementation
{
    public class BookService : IBookService
    {
        private readonly IRepository<Book> _bookRepository;
        private readonly IRepository<BookAuthor> _bookAuthorRepository;
        private readonly IRepository<BookGenre> _bookGenreRepository;
        private readonly IRepository<BookRating> _bookRatingRepository;
        private readonly IRepository<Language> _bookLanguageRepository;
        private readonly IRepository<User> _userLocationRepository;
        private readonly IRepository<Request> _requestRepository;
        private readonly IUserResolverService _userResolverService;
        private readonly IPaginationService _paginationService;
        private readonly IRootRepository<BookRootComment> _rootCommentRepository;
        private readonly IImageService _imageService;
        private readonly IMapper _mapper;
        private readonly IHangfireJobScheduleService _hangfireJobScheduleService;
        private readonly IEmailSenderService _emailSenderService;
        private readonly IWishListService _wishListService;
        private readonly INotificationsService _notificationsService;



        public BookService(IRepository<Book> bookRepository, IMapper mapper, IRepository<BookAuthor> bookAuthorRepository, IRepository<BookGenre> bookGenreRepository,
            IRepository<Language> bookLanguageRepository, IRepository<User> userLocationRepository, IPaginationService paginationService, IRepository<Request> requestRepository,
            IUserResolverService userResolverService, IImageService imageService, IHangfireJobScheduleService hangfireJobScheduleService, IEmailSenderService emailSenderService,
            IRootRepository<BookRootComment> rootCommentRepository, IWishListService wishListService, IRepository<BookRating> bookRatingRepository, INotificationsService notificationsService)
        {
            _bookRepository = bookRepository;
            _bookAuthorRepository = bookAuthorRepository;
            _bookGenreRepository = bookGenreRepository;
            _bookRatingRepository = bookRatingRepository;
            _bookLanguageRepository = bookLanguageRepository;
            _userLocationRepository = userLocationRepository;
            _requestRepository = requestRepository;
            _paginationService = paginationService;
            _mapper = mapper;
            _imageService = imageService;
            _userResolverService = userResolverService;
            _hangfireJobScheduleService = hangfireJobScheduleService;
            _emailSenderService = emailSenderService;
            _rootCommentRepository = rootCommentRepository;
            _wishListService = wishListService;
            _notificationsService = notificationsService;
        }

        public async Task<BookGetDto> GetByIdAsync(int bookId)
        {
            return _mapper.Map<BookGetDto>(await _bookRepository.GetAll()
                                                               .Include(p => p.BookAuthor)
                                                               .ThenInclude(x => x.Author)
                                                               .Include(p => p.BookGenre)
                                                               .ThenInclude(x => x.Genre)
                                                               .Include(x => x.Language)
                                                               .Include(p => p.User)
                                                               .ThenInclude(x => x.UserRoom)
                                                               .ThenInclude(x => x.Location)
                                                               .Include(p => p.Language)
                                                               .FirstOrDefaultAsync(p => p.Id == bookId));
        }

        public async Task<BookGetDto> AddAsync(BookPostDto bookDto)
        {
            var book = _mapper.Map<Book>(bookDto);
            if (bookDto.Image != null)
            {
                book.ImagePath = await _imageService.UploadImage(bookDto.Image);
            }
            _bookRepository.Add(book);
            await _bookRepository.SaveChangesAsync();
            return _mapper.Map<BookGetDto>(book);
        }

        public async Task<bool> RemoveAsync(int bookId)
        {
            var book = await _bookRepository.FindByIdAsync(bookId);
            if (book == null)
            {
                return false;
            }
            if (book.ImagePath != null)
            {
                _imageService.DeleteImage(book.ImagePath);
            }
            _bookRepository.Remove(book);
            var affectedRows = await _bookRepository.SaveChangesAsync();
            return affectedRows > 0;
        }

        public async Task<bool> UpdateAsync(BookPutDto bookDto)
        {
            var book = _mapper.Map<Book>(bookDto);
            var oldBook = await _bookRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(a => a.Id == book.Id);
            if (oldBook == null)
            {
                return false;
            }
            if (bookDto.FieldMasks.Contains("Image"))
            {
                string imagePath;
                bookDto.FieldMasks.Remove("Image");
                bookDto.FieldMasks.Add("ImagePath");
                if (oldBook.ImagePath != null)
                {
                    _imageService.DeleteImage(oldBook.ImagePath);
                }
                if (bookDto.Image != null)
                {
                    imagePath = await _imageService.UploadImage(bookDto.Image);
                }
                else
                {
                    imagePath = null;
                }
                book.ImagePath = imagePath;
            }
            await _bookRepository.Update(book, bookDto.FieldMasks);
            if (bookDto.UserId != oldBook.UserId)
            {
                var user = await _userLocationRepository.FindByIdAsync(oldBook.UserId.Value);
                string emailMessageForUser = $" Administrator has successfully received your book '{oldBook.Name}'";
                SendMailForOwnership(book, user, emailMessageForUser );
                SendNotificationToUser(oldBook.UserId.Value, book.Id, emailMessageForUser);

                var userId = _userResolverService.GetUserId();
                var admin = await _userLocationRepository.FindByIdAsync(userId);

                string emailMessageForAdmin = $"You became the current owner of the book '{oldBook.Name}'";
                SendMailForOwnership(book, admin, emailMessageForAdmin );
                SendNotificationToUser(userId, book.Id, emailMessageForAdmin );
            }
            var affectedRows = await _bookRepository.SaveChangesAsync();
            var isDatabaseUpdated = affectedRows > 0;
            if (isDatabaseUpdated &&
                bookDto.FieldMasks.Contains("State") &&
                bookDto.State == BookState.Available)
            {
                await _wishListService.NotifyAboutAvailableBookAsync(book.Id);
            }
            return isDatabaseUpdated;
        }

        public async Task<PaginationDto<BookGetDto>> GetAllAsync(BookQueryParams parameters)
        {


            if (parameters.LocationFilterOn == true && 
                (parameters.HomeLocations == null || parameters.HomeLocations.Length == 0) &&
                (parameters.Locations == null || parameters.Locations.Length == 0))
            {
                return new PaginationDto<BookGetDto>()
                {
                    TotalCount = 0,
                    Page = new List<BookGetDto>()
                };
            }


            var query = GetFilteredQuery(_bookRepository.GetAll(), parameters);
            if (parameters.HomeLocations?.Length > 0)
            {
                var queryBooksAtHome = GetFilteredQuery(_bookRepository.GetAll(), parameters, false);
                query = query.Union(queryBooksAtHome);
            }
            if (parameters.SortableParams != null)
            {
                query = query.OrderBy(parameters.SortableParams);
            }
            return await _paginationService.GetPageAsync<BookGetDto, Book>(query, parameters);
        }

        public IQueryable<Book> GetBooksInReadStatus()
        {
            var userId = _userResolverService.GetUserId();
            return _bookRepository.GetAll().Where(b => b.UserId == userId && b.State == BookState.Reading);
        }

        private async Task<IEnumerable<int>> FindRegisteredBooksAsync(int userId)
        {
            //registered books            
            var allRequests = await _requestRepository.GetAll()
                .Select(x => new { Owner = x.Owner.Id, Time = x.RequestDate, Book = x.Book })
                .ToListAsync();

            var firstRequests = allRequests.GroupBy(a => new { a.Book })
                .Select(g => new
                {
                    g.Key.Book,
                    MinTime = g.Min(book => book.Time)
                }).ToList();

            var firstRequestsBookId = firstRequests.Select(g => g.Book.Id);

            var userBooks = await _bookRepository.GetAll()
                .Where(x => x.UserId == userId)
                .Select(x => x.Id)
                .ToListAsync();

            var userCurrentBooks = userBooks.Except(firstRequestsBookId);

            var userFirstBooks = allRequests.Where(a => a.Owner == userId && a.Time == firstRequests.Single(b => b.Book == a.Book).MinTime).Select(a => a.Book.Id);
            //all user books
            var allBooks = userCurrentBooks.Union(userFirstBooks);

            return allBooks;
        }

        public async Task<PaginationDto<BookGetDto>> GetRegisteredAsync(BookQueryParams parameters)
        {
            var userId = _userResolverService.GetUserId();
            var registeredBooks = await FindRegisteredBooksAsync(userId);
            var query = _bookRepository.GetAll().Where(x => registeredBooks.Contains(x.Id));
            query = GetFilteredQuery(query, parameters);

            return await _paginationService.GetPageAsync<BookGetDto, Book>(query, parameters);
        }

        public async Task<IQueryable<Book>> GetRegisteredAsync()
        {
            var userId = _userResolverService.GetUserId();
            var registeredBooks = await FindRegisteredBooksAsync(userId);

            return _bookRepository.GetAll().Where(x => registeredBooks.Contains(x.Id));
        }

        public async Task<int> GetNumberOfTimesRegisteredBooksWereReadAsync(int userId)
        {
            var booksTransitions = await _requestRepository.GetAll()
                .Where(r => r.ReceiveDate != null && r.OwnerId != r.UserId)
                .GroupBy(r => r.BookId)
                .Select(r => new { BookId = r.Key, Count = r.Count() })
                .ToListAsync();
            var registeredBooks = await FindRegisteredBooksAsync(userId);

            return booksTransitions
                .Where(x => registeredBooks.Contains(x.BookId))
                .Sum(c => c.Count);
        }

        public async Task<PaginationDto<BookGetDto>> GetCurrentOwned(BookQueryParams parameters)
        {
            var userId = _userResolverService.GetUserId();
            var query = _bookRepository.GetAll().Where(p => p.UserId == userId);
            query = GetFilteredQuery(query, parameters);

            return await _paginationService.GetPageAsync<BookGetDto, Book>(query, parameters);
        }

        public async Task<List<BookGetDto>> GetCurrentOwnedById(int id)
        {
            var query = _mapper.Map<List<BookGetDto>>(await _bookRepository.GetAll().Where(p => p.UserId == id).ToListAsync());

            return query;
        }

        public async Task<int> GetCurrentOwnedByIdCount(int userId)
        {
            var books = _bookRepository.GetAll().Where(p => p.UserId == userId);
            return books.Count();
        }

        public async Task<PaginationDto<BookGetDto>> GetReadBooksAsync(BookQueryParams parameters)
        {
            var userId = _userResolverService.GetUserId();
            var ownedBooks = _requestRepository.GetAll().Where(a => a.OwnerId == userId).Select(a => a.Book);
            var currentlyOwnedBooks = _bookRepository.GetAll().Where(a => a.UserId == userId);
            var readBooks = ownedBooks.Union(currentlyOwnedBooks);
            var query = GetFilteredQuery(readBooks, parameters);
            return await _paginationService.GetPageAsync<BookGetDto, Book>(query, parameters);
        }

        public IQueryable<Book> GetAlreadyReadBooks()
        {
            var userId = _userResolverService.GetUserId();
            var ownedBooks = _requestRepository.GetAll()
                .Where(a => a.OwnerId == userId && a.ReceiveDate != null && a.OwnerId != a.UserId)
                .Select(a => a.Book);
            var currentlyOwnedReadBooks = _bookRepository.GetAll()
                .Where(b => b.UserId == userId && b.State == BookState.Available);

            return ownedBooks.Union(currentlyOwnedReadBooks);
        }

        public async Task<bool> ActivateAsync(int bookId)
        {
            var book = _bookRepository.GetAll()
                .Include(i => i.User).Where(x => x.Id == bookId).ToList()
                .FirstOrDefault();
            if (book == null)
            {
                return false;
            }

            book.State = BookState.Available;
            await _bookRepository.Update(book, new List<string>() { "State" });
            var isDatabaseUpdated = await _bookRepository.SaveChangesAsync() > 0;

            if (isDatabaseUpdated)
            {
                if (_userLocationRepository.FindByCondition(u => u.Email == book.User.Email).Result.IsEmailAllowed)
                {
                    SendMailForActivated(book, book.User);

                    SendNotificationToUser(book.User.Id, book.Id, $"The status of your book '{book.Name}' have successfully changed to 'Active'");

                    var userId = _userResolverService.GetUserId();
                    var user = await _userLocationRepository.FindByIdAsync(userId);

                    SendMailForActivated(book, user);

                    SendNotificationToUser(userId, book.Id, $"You have successfully change the book's status to 'Active' for '{book.Name}'");
                }
                await _wishListService.NotifyAboutAvailableBookAsync(book.Id);
            }
            return true;
        }

        public async Task<bool> DeactivateAsync(int bookId)
        {
            var book = _bookRepository.GetAll()
                .Include(i => i.User).Where(x => x.Id == bookId).ToList()
                .FirstOrDefault();
            if (book == null)
            {
                return false;
            }

            if (book.State == BookState.Requested)
            {
                var request = _requestRepository.GetAll()
                    .Include(i => i.Book)
                    .Include(i => i.Book)
                    .Include(i => i.User).Where(x => x.BookId == bookId).ToList()
                    .Last();
                if (_userLocationRepository.FindByCondition(u => u.Email == book.User.Email).Result.IsEmailAllowed)
                {
                    SendMailForActivated(book, request.User);
                }
                await _hangfireJobScheduleService.DeleteRequestScheduleJob(request.Id);
                _requestRepository.Remove(request);
                await _requestRepository.SaveChangesAsync();
            }
            if (_userLocationRepository.FindByCondition(u => u.Email == book.User.Email).Result.IsEmailAllowed)
            {
                SendMailForDeactivated(book,book.User);

                SendNotificationToUser(book.User.Id,book.Id, $"The status of your book '{book.Name}' have successfully changed to Inactive");

                var userId = _userResolverService.GetUserId();
                var user = await _userLocationRepository.FindByIdAsync(userId);

                SendMailForDeactivated(book, user);

                SendNotificationToUser(userId, book.Id, $"You have successfully change the book's status to Inactive for '{book.Name}'");
;
            }
            book.State = BookState.InActive;
            await _bookRepository.Update(book, new List<string>() { "State" });
            await _bookRepository.SaveChangesAsync();

            return true;
        }


        public async void SendMailForActivated(Book book, User user)
        {
            var emailMessageForBookActivated = new RequestMessage()
            {
                UserName = user.FirstName + " " + user.LastName,
                BookName = book.Name,
                BookId = book.Id,
                UserAddress = new MailboxAddress($"{user.Email}"),
            };
            await _emailSenderService.SendForBookActivatedAsync(emailMessageForBookActivated);
        }

        public async void SendMailForDeactivated(Book book, User user)
        {
            var emailMessageForBookActivated = new RequestMessage()
            {
                UserName = user.FirstName + " " + user.LastName,
                BookName = book.Name,
                BookId = book.Id,
                UserAddress = new MailboxAddress($"{user.Email}"),
            };
            await _emailSenderService.SendForBookDeactivatedAsync(emailMessageForBookActivated);
        }

        public async Task SendMailForOwnership(Book book, User user, string emailMessage)
        {
            var emailMessageForBookActivated = new RequestMessage()
            {
                UserName = user.FirstName+" "+user.LastName,
                BookName = book.Name,
                BookId = book.Id,
                UserAddress = new MailboxAddress($"{user.Email}"),
            };
            await _emailSenderService.SendForOwnershipAsync(emailMessageForBookActivated, emailMessage);
        }

        public async void SendNotificationToUser(int userId, int bookId, string message)
        {
            await _notificationsService.NotifyAsync(
               userId,
               message,
               bookId,
               NotificationAction.Open);
        }

        public async Task<int> GetNumberOfBooksInReadStatusAsync(int userId)
        {
            var currentlyOwnedBooks = _bookRepository.GetAll()
                .Where(a => a.UserId == userId && a.State == BookState.Reading);

            return await currentlyOwnedBooks.CountAsync();
        }

        public async Task<bool> SetRating(BookRatingQueryParams ratingQueryParams)
        {
            var book = await _bookRepository.FindByIdAsync(ratingQueryParams.BookId);
            if (book == null)
            {
                return false;
            }

            var bookRating = new BookRating(ratingQueryParams.BookId, ratingQueryParams.UserId, ratingQueryParams.Rating);
            _bookRatingRepository.Add(bookRating);
            await _bookRatingRepository.SaveChangesAsync();
            var avgRating = _bookRatingRepository.GetAll()
                .Where(b => b.BookId == ratingQueryParams.BookId)
                .Average(b => b.Rating);
            book.Rating = avgRating;
            _bookRepository.Update(book);
            await _bookRepository.SaveChangesAsync();

            return true;
        }

        public async Task<double> GetRating(int bookId, int userId)
        {
            var rating = await _bookRatingRepository.FindByIdAsync(bookId, userId);

            return rating != null ? rating.Rating : 0;
        }

        public IEnumerable<Request> GetBooksTransitions()
        {
            return _requestRepository.GetAll()
                .Where(r => r.ReceiveDate != null && r.OwnerId != r.UserId)
                .Include(r => r.User)
                .ThenInclude(u => u.UserRoom)
                .ThenInclude(r => r.Location)
                .Include(r => r.Book)
                .ThenInclude(b => b.BookGenre)
                .ThenInclude(g => g.Genre);
        }

        private IQueryable<Book> GetFilteredQuery(IQueryable<Book> query, BookQueryParams parameters, bool byLocation = true)
        {
            if (parameters.ShowAvailable == true)
            {
                query = query.Where(b => b.State == BookState.Available);
            }
            if (byLocation && parameters.Locations != null)
            {
                var predicate = PredicateBuilder.New<Book>();
                foreach (var id in parameters.Locations)
                {
                    var tempId = id;
                    predicate = predicate.Or(b => 
                        b.User.UserRoom.LocationId == id && 
                        (b.User.LocationHomeId == null || !b.User.LocationHome.IsActive)
                    );
                }
                query = query.Where(predicate);
            }
            if (!byLocation && parameters.HomeLocations != null)
            {
                var predicate = PredicateBuilder.New<Book>();
                foreach (var id in parameters.HomeLocations)
                {
                    var tempId = id;
                    predicate = predicate.Or(b => 
                        b.User.LocationHomeId == id &&
                        b.User.LocationHome.IsActive
                    );
                }
                query = query.Where(predicate);
            }
            if (parameters.SearchTerm != null)
            {
                var term = parameters.SearchTerm.Split(" ");
                if (term.Length == 1)
                {
                    query = query.Where(x => x.Name.Contains(parameters.SearchTerm) || x.BookAuthor.Any(a => a.Author.LastName.Contains(term[term.Length - 1]) || a.Author.FirstName.Contains(term[0])));
                }
                else
                {
                    query = query.Where(x => x.Name.Contains(parameters.SearchTerm) || x.BookAuthor.Any(a => a.Author.LastName.Contains(term[term.Length - 1]) && a.Author.FirstName.Contains(term[0])));
                }
            }
            if (parameters.Genres != null)
            {
                var predicate = PredicateBuilder.New<Book>();
                foreach (var id in parameters.Genres)
                {
                    var tempId = id;
                    predicate = predicate.Or(g => g.BookGenre.Any(g => g.Genre.Id == tempId));
                }
                query = query.Where(predicate);
            }
            if (parameters.Languages != null)
            {
                var predicate = PredicateBuilder.New<Book>();
                foreach (var id in parameters.Languages)
                {
                    predicate = predicate.Or(g => g.Language.Id == id);
                }
                query = query.Where(predicate);
            }

            return query
                .Include(p => p.BookAuthor)
                .ThenInclude(x => x.Author)
                .Include(p => p.BookGenre)
                .ThenInclude(x => x.Genre)
                .Include(x => x.Language)
                .Include(p => p.User)
                .ThenInclude(x => x.UserRoom)
                .ThenInclude(x => x.Location)
                .Include(x => x.Language);
        }

        public IEnumerable<MapLocationDto> GetBooksQuantityOnLocations()
        {
            return _bookRepository.GetAll()
                .Include(b => b.User)
                .ThenInclude(u => u.UserRoom)
                .ThenInclude(r => r.Location)
                .Where(b => b.User.UserRoom.Location.IsActive && b.User.LocationHomeId == null)
                .AsEnumerable()
                .GroupBy(b => b.User.UserRoom.Location)
                .Select(l => new MapLocationDto(_mapper.Map<Location, LocationDto>(l.Key), l.Count()));
        }
    }
}
