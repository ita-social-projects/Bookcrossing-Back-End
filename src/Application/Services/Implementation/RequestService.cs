using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
using Microsoft.EntityFrameworkCore.Internal;
using MimeKit;

namespace Application.Services.Implementation
{
    public class RequestService : IRequestService
    {
        private readonly IRepository<Request> _requestRepository;
        private readonly IRepository<Book> _bookRepository;
        private readonly IMapper _mapper;
        private readonly IEmailSenderService _emailSenderService;
        private readonly IRepository<User> _userRepository;
        private readonly IPaginationService _paginationService;
        private readonly IHangfireJobScheduleService _hangfireJobScheduleService;
        private readonly IWishListService _wishListService;
        private readonly INotificationsService _notificationsService;

        public RequestService(
            IRepository<Request> requestRepository,
            IRepository<Book> bookRepository,
            IMapper mapper,
            IEmailSenderService emailSenderService,
            IRepository<User> userRepository,
            IPaginationService paginationService,
            IHangfireJobScheduleService hangfireJobScheduleService,
            IWishListService wishListService,
            INotificationsService notificationsService)
        {
            _requestRepository = requestRepository;
            _bookRepository = bookRepository;
            _mapper = mapper;
            _emailSenderService = emailSenderService;
            _userRepository = userRepository;
            _paginationService = paginationService;
            _hangfireJobScheduleService = hangfireJobScheduleService;
            _wishListService = wishListService;
            _notificationsService = notificationsService;
        }

        /// <inheritdoc />
        public async Task<RequestDto> MakeAsync(int userId, int bookId)
        {
            var book = await _bookRepository.GetAll().Include(x => x.User).FirstOrDefaultAsync(x => x.Id == bookId);
            var isNotAvailableForRequest = book == null || book.State != BookState.Available;
            if (isNotAvailableForRequest)
            {
                return null;
            }

            if (userId == book.UserId)
            {
                throw new InvalidOperationException("You cannot request your book");
            }

            var user = await _userRepository.FindByIdAsync(userId);
            if (user.IsDeleted)
            {
                throw new InvalidOperationException("As deleted user you cannot request books");
            }

            var request = new Request()
            {
                BookId = book.Id,
                OwnerId = book.UserId,
                UserId = userId,
                RequestDate = DateTime.UtcNow
            };
            _requestRepository.Add(request);
            await _requestRepository.SaveChangesAsync();
            book.State = BookState.Requested;
            await _bookRepository.SaveChangesAsync();
            if (book.User.IsEmailAllowed)
            {
                var emailMessageForRequest = new RequestMessage()
                {
                    OwnerName = book.User.FirstName + " " + book.User.LastName,
                    BookName = book.Name,
                    RequestDate = request.RequestDate,
                    RequestId = request.Id,
                    OwnerAddress = new MailboxAddress($"{book.User.Email}"),
                    UserName = user.FirstName + " " + user.LastName
                };
                await _emailSenderService.SendForRequestAsync(emailMessageForRequest);
            }

            await _notificationsService.NotifyAsync(
                book.User.Id,
                $"Your book '{book.Name}' was requested by {user.FirstName} {user.LastName}",
                $"Надійшов запит щодо вашої книги '{book.Name}' від  {user.FirstName} {user.LastName}",
                book.Id,
                NotificationActions.Open);
            await _notificationsService.NotifyAsync(
                user.Id,
                $"The book '{book.Name}' successfully requested.",
                $"Запит щодо книги '{book.Name}' успішно подано",
                book.Id,
                NotificationActions.Open);

            var emailMessageForReceiveConfirmation = new RequestMessage()
            {
                UserName = user.FirstName + " " + user.LastName,
                BookName = book.Name,
                BookId = book.Id,
                RequestId = request.Id,
                UserAddress = new MailboxAddress($"{user.Email}"),
                User = user
            };
            await _hangfireJobScheduleService.ScheduleRequestJob(emailMessageForReceiveConfirmation);

            return _mapper.Map<RequestDto>(request);
        }

        /// <inheritdoc />
        public async Task<RequestDto> GetByBookAsync(Expression<Func<Request, bool>> predicate, RequestsQueryParams query)
        {
            Request request = null;
            if (query.First)
            {
                request = await _requestRepository.GetAll()
                    .Include(i => i.Book).ThenInclude(i => i.BookAuthor).ThenInclude(i => i.Author)
                    .Include(i => i.Book).ThenInclude(i => i.BookGenre).ThenInclude(i => i.Genre)
                    .Include(i => i.Book).ThenInclude(i => i.Language)
                    .Include(i => i.Owner).ThenInclude(i => i.UserRoom).ThenInclude(i => i.Location)
                    .Include(i => i.User).ThenInclude(i => i.UserRoom).ThenInclude(i => i.Location)
                    .FirstOrDefaultAsync(predicate);
            }
            else if (query.Last)
            {
                request = _requestRepository.GetAll()
                    .Include(i => i.Book).ThenInclude(i => i.BookAuthor).ThenInclude(i => i.Author)
                    .Include(i => i.Book).ThenInclude(i => i.BookGenre).ThenInclude(i => i.Genre)
                    .Include(i => i.Book).ThenInclude(i => i.Language)
                    .Include(i => i.Owner).ThenInclude(i => i.UserRoom).ThenInclude(i => i.Location)
                    .Include(i => i.User).ThenInclude(i => i.UserRoom).ThenInclude(i => i.Location).Where(predicate).ToList()
                    .Last();
            }

            if (request == null)
            {
                return null;
            }

            return _mapper.Map<RequestDto>(request);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<RequestDto>> GetAllByBookAsync(Expression<Func<Request, bool>> predicate)
        {
            var requests = _requestRepository.GetAll()
                .Include(i => i.Book).ThenInclude(i => i.BookAuthor).ThenInclude(i => i.Author)
                .Include(i => i.Book).ThenInclude(i => i.BookGenre).ThenInclude(i => i.Genre)
                .Include(i => i.Book).ThenInclude(i => i.Language)
                .Include(i => i.Owner).ThenInclude(i => i.UserRoom).ThenInclude(i => i.Location)
                .Include(i => i.User).ThenInclude(i => i.UserRoom).ThenInclude(i => i.Location)
                .Where(predicate);
            if (requests == null)
            {
                return null;
            }

            return _mapper.Map<List<RequestDto>>(requests);
        }

        /// <inheritdoc />
        public async Task<PaginationDto<RequestDto>> GetAsync(Expression<Func<Request, bool>> predicate, BookQueryParams parameters)
        {


            var query = _requestRepository.GetAll()
                .Include(i => i.Book).ThenInclude(i => i.BookAuthor).ThenInclude(i => i.Author)
                .Include(i => i.Book).ThenInclude(i => i.BookGenre).ThenInclude(i => i.Genre)
                .Include(i => i.Book).ThenInclude(i => i.Language)
                .Include(i => i.Owner).ThenInclude(i => i.UserRoom).ThenInclude(i => i.Location)
                .Include(i => i.User).ThenInclude(i => i.UserRoom).ThenInclude(i => i.Location)
                .Where(predicate);

            if (parameters.BookStates != null)
            {
                var wherePredicate = PredicateBuilder.New<Request>();
                foreach (var state in parameters.BookStates)
                {
                    wherePredicate = wherePredicate.Or(g => g.Book.State == state);
                }
                query = query.Where(wherePredicate);
            }

            if (parameters.SearchTerm != null)
            {
                var term = parameters.SearchTerm.Split(" ");
                if (term.Length == 1)
                {
                    query = query.Where(
                        x =>
                            (x.Book.ISBN != null && x.Book.ISBN.Contains(parameters.SearchTerm)) ||
                            x.Book.Name.Contains(parameters.SearchTerm) ||
                            x.Book.BookAuthor.Any(
                                a =>
                                    a.Author.LastName.Contains(term[term.Length - 1]) ||
                                    a.Author.FirstName.Contains(term[0])
                            )
                    );
                }
                else
                {
                    query = query.Where(
                        x =>
                            (x.Book.ISBN != null && x.Book.ISBN.Contains(parameters.SearchTerm)) ||
                            x.Book.Name.Contains(parameters.SearchTerm) ||
                            x.Book.BookAuthor.Any(
                                a =>
                                    a.Author.LastName.Contains(term[term.Length - 1]) &&
                                    a.Author.FirstName.Contains(term[0])
                            )
                    );
                }
            }

            if (parameters.Languages != null)
            {
                var wherePredicate = PredicateBuilder.New<Request>();
                foreach (var id in parameters.Languages)
                {
                    wherePredicate = wherePredicate.Or(g => g.Book.Language.Id == id);
                }
                query = query.Where(wherePredicate);
            }

            if (parameters.Genres != null)
            {
                var wherePredicate = PredicateBuilder.New<Request>();
                foreach (var id in parameters.Genres)
                {
                    var tempId = id;
                    wherePredicate = wherePredicate.Or(g => g.Book.BookGenre.Any(g => g.Genre.Id == tempId));
                }
                query = query.Where(wherePredicate);
            }

            if (parameters.SortableParams != null)
            {
                query = SortRequests(query, parameters.SortableParams);
            }

            return await _paginationService.GetPageAsync<RequestDto, Request>(query, parameters);
        }

        public IQueryable<Request> SortRequests(IQueryable<Request> requests, SortableParams sortableParams)
        {
            switch (sortableParams.OrderByField)
            {
                case "Rating":
                    requests = sortableParams.OrderByAscending ?
                        requests.OrderBy(r => r.Book.Rating):
                        requests.OrderByDescending(r => r.Book.Rating);
                    break;
                case "DateAdded":
                    requests = sortableParams.OrderByAscending ?
                        requests.OrderBy(r => r.Book.DateAdded) :
                        requests.OrderByDescending(r => r.Book.DateAdded);
                    break;
                case "Name":
                    requests = sortableParams.OrderByAscending ?
                        requests.OrderBy(r => r.Book.Name) :
                        requests.OrderByDescending(r => r.Book.Name);
                    break;
                case "PredictedRating":
                    requests = sortableParams.OrderByAscending ?
                        requests.OrderBy(r => r.Book.PredictedRating) :
                        requests.OrderByDescending(r => r.Book.PredictedRating);
                    break;
                case "WishCount":
                    requests = sortableParams.OrderByAscending ?
                        requests.OrderBy(r => r.Book.WishCount) :
                        requests.OrderByDescending(r => r.Book.WishCount);
                    break;
            }

            return requests;
        }


            /// <inheritdoc />
            public async Task<bool> ApproveReceiveAsync(int id)
        {
            var request = await _requestRepository.GetAll()
                .Include(x => x.Book)
                .Include(x => x.User)
                .Include(x => x.Owner)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (request == null)
            {
                return false;
            }

            var book = await _bookRepository.FindByIdAsync(request.BookId);
            book.User = request.User;
            book.State = BookState.Reading;
            _bookRepository.Update(book);
            await _bookRepository.SaveChangesAsync();
            request.ReceiveDate = DateTime.UtcNow;
            _requestRepository.Update(request);
            var affectedRows = await _requestRepository.SaveChangesAsync();
            if (request.Owner.IsEmailAllowed)
            {
                var emailMessage = new RequestMessage()
                {
                    OwnerName = request.Owner.FirstName + " " + request.Owner.LastName,
                    BookName = request.Book.Name,
                    RequestId = request.Id,
                    UserName = request.User.FirstName + " " + request.User.LastName,
                    OwnerAddress = new MailboxAddress($"{request.Owner.Email}")
                };
                await _emailSenderService.SendThatBookWasReceivedToPreviousOwnerAsync(emailMessage);
            }
            if (request.User.IsEmailAllowed)
            {
                var emailMessage = new RequestMessage()
                {
                    OwnerName = request.User.FirstName + " " + request.User.LastName,
                    BookName = request.Book.Name,
                    RequestId = request.Id,
                    OwnerAddress = new MailboxAddress($"{request.User.Email}")
                };

                await _emailSenderService.SendThatBookWasReceivedToNewOwnerAsync(emailMessage);
            }

            await _notificationsService.NotifyAsync(
                request.Owner.Id,
                $"{request.User.FirstName} {request.User.LastName} has successfully received and started reading '{book.Name}'.",
                $"Користувач {request.User.FirstName} {request.User.LastName} успішно отримав  '{book.Name} та розпочав процес читання",
                book.Id,
                NotificationActions.Open);

            await _notificationsService.NotifyAsync(
                request.User.Id,
                $"You became a current owner of the book '{book.Name}'",
                $"Ви стали поточним власником книги '{book.Name}'",
                book.Id,
                NotificationActions.Open);

            await _hangfireJobScheduleService.DeleteRequestScheduleJob(id);
            return affectedRows > 0;
        }

        /// <inheritdoc />
        public async Task<bool> RemoveAsync(int requestId)
        {
            var request = await _requestRepository.GetAll()
                .Include(x => x.Book)
                .Include(x => x.Owner)
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == requestId);
            if (request == null)
            {
                return false;
            }

            await _hangfireJobScheduleService.DeleteRequestScheduleJob(requestId);
            if (request.Owner.IsEmailAllowed)
            {
                var emailMessage = new RequestMessage()
                {
                    UserName = request.User.FirstName + " " + request.User.LastName,
                    OwnerName = request.Owner.FirstName + " " + request.Owner.LastName,
                    BookName = request.Book.Name,
                    RequestId = request.Id,
                    OwnerAddress = new MailboxAddress($"{request.Owner.Email}")
                };
                await _emailSenderService.SendForCanceledRequestAsync(emailMessage);
            }

            await _notificationsService.NotifyAsync(
                request.Owner.Id,
                $"Your book '{request.Book.Name}' request was canceled.",
                $"Ваш запит щодо книги  '{request.Book.Name}' скасовано",
                request.BookId,
                NotificationActions.Open);

            await _notificationsService.NotifyAsync(
                request.User.Id,
                $"Your request for book '{request.Book.Name}' was canceled.",
                $"Ваш запит щодо книги  '{request.Book.Name}' скасовано",
                request.BookId,
                NotificationActions.Open);

            var book = await _bookRepository.FindByIdAsync(request.BookId);
            book.State = BookState.Available;
            var isBookUpdated = await _bookRepository.SaveChangesAsync() > 0;
            if (isBookUpdated)
            {
                await _wishListService.NotifyAboutAvailableBookAsync(book.Id);
            }

            _requestRepository.Remove(request);

            var affectedRows = await _requestRepository.SaveChangesAsync();
            return affectedRows > 0;
        }

        public async Task<int> GetNumberOfRequestedBooksAsync(int userId)
        {
            return await _requestRepository.GetAll().Where(r => r.UserId == userId && r.ReceiveDate == null).CountAsync();
        }
    }
}
