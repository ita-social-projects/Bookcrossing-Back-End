using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Dto;
using Application.Dto.QueryParams;
using Application.Services.Implementation;
using Application.Services.Interfaces;
using AutoMapper;
using BookCrossingBackEnd.Migrations;
using Domain.RDBMS;
using Domain.RDBMS.Entities;
using FluentAssertions;
using Infrastructure.RDBMS;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MimeKit.Cryptography;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;

namespace ApplicationTest.Services
{
    [TestFixture]
    public class MessageServiceTest
    {
        private BookCrossingContext _context;
        private SuggestionMessageService _messageService;
        private Mock<IRepository<SuggestionMessage>> _messageRepositoryMock;
        private Mock<IPaginationService> _paginationServiceMock;
        private Mock<IMapper> _mapperMock;

        private List<SuggestionMessage> _messages;
        private Mock<IQueryable<SuggestionMessage>> _messagesQueryableMock;
        private List<SuggestionMessageDto> _messagesDto;
        private SuggestionMessage _message;
        private SuggestionMessageDto _messageDto;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _messageRepositoryMock = new Mock<IRepository<SuggestionMessage>>();
            _mapperMock = new Mock<IMapper>();
            _paginationServiceMock = new Mock<IPaginationService>();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new Application.MapperProfilers.SuggestionMessageProfile());
                mc.AddProfile(new Application.MapperProfilers.UserProfile());
            });
            var _mapper = mappingConfig.CreateMapper();
            var options = new DbContextOptionsBuilder<BookCrossingContext>().UseInMemoryDatabase(databaseName: "Fake DB").ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning)).Options;
            _context = new BookCrossingContext(options);
            
            _messageService = new SuggestionMessageService(_messageRepositoryMock.Object, _mapperMock.Object, _paginationServiceMock.Object);

            MockData();
        }

        [SetUp]
        public void SetUp()
        {
            _messageRepositoryMock.Reset();
        }

        [Test]
        public async Task GetMessageById_MessageExists_ReturnsMessageDto()
        {
            _messageRepositoryMock.Setup(s => s.GetAll())
                .Returns(_messagesQueryableMock.Object);
            _mapperMock.Setup(obj => obj.Map<SuggestionMessageDto>(_message))
                .Returns(_messageDto);

            var messageResult = await _messageService.GetById(_message.Id);

            messageResult.Should().Be(_messageDto);
        }

        [Test]
        public async Task GetAll_NoParametersPassed_ReturnsListOfMessageDtos()
        {
            _messageRepositoryMock.Setup(s => s.GetAll()).Returns(_messagesQueryableMock.Object);
            _mapperMock.Setup(obj => obj.Map<List<SuggestionMessageDto>>(
                    It.Is<List<SuggestionMessage>>(x => ListsHasSameElements(x, _messages))))
                .Returns(_messagesDto);

            var messageResult = await _messageService.GetAll();

            messageResult.Should().BeEquivalentTo(_messagesDto);
        }


        [Test]
        public async Task RemoveMessage_MessageExists_ReturnsMessageDtoRemoved()
        {
            _messageRepositoryMock.Setup(s => s.FindByIdAsync(_message.Id))
                .ReturnsAsync(_message);
            _mapperMock.Setup(obj => obj.Map<SuggestionMessageDto>(_message))
                .Returns(_messageDto);

            var messageResult = await _messageService.Remove(_message.Id);

            _messageRepositoryMock.Verify(obj => obj.Remove(_message), Times.Once);
            _messageRepositoryMock.Verify(obj => obj.SaveChangesAsync(), Times.Once);

            messageResult.Should().Be(_messageDto);
        }

        [Test]
        public async Task RemoveMessage_MessageNotExist_ReturnsNull()
        {
            _messageRepositoryMock.Setup(s => s.FindByIdAsync(_message.Id))
                .ReturnsAsync(value: null);

            var messageResult = await _messageService.Remove(_message.Id);

            messageResult.Should().BeNull();
        }

        [Test]
        public async Task Update_ShouldUpdateMessageInDatabase()
        {
            _mapperMock.Setup(obj => obj.Map<SuggestionMessage>(_messageDto))
                .Returns(_message);

            await _messageService.Update(_messageDto);

            _messageRepositoryMock.Verify(obj => obj.Update(_message), Times.Once);
            _messageRepositoryMock.Verify(obj => obj.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task Add_ShouldAddMessageToDatabase()
        {
            _mapperMock.Setup(obj => obj.Map<SuggestionMessage>(_messageDto))
                .Returns(_message);

            await _messageService.Add(_messageDto);

            _messageRepositoryMock.Verify(obj => obj.Add(_message), Times.Once);
            _messageRepositoryMock.Verify(obj => obj.SaveChangesAsync(), Times.Once);
        }

        #region Data
        private void MockData()
        {
            List<User> users = GetUsers();
            _messages = new List<SuggestionMessage>
            {
                new SuggestionMessage()
                {
                    Id = 1,
                    Summary = "Error",
                    Text = "Some text",
                    UserId = 1,
                    State = MessageState.Read,
                },
                new SuggestionMessage()
                {
                    Id = 2,
                    Summary = "Suggest",
                    Text = "Some text 2",
                    UserId = 2,
                    State = MessageState.Unread,
                },
            };
            
            _messagesDto = _messages.Select(message => new SuggestionMessageDto
            {
                Id = message.Id,
                Summary = message.Summary,
                Text = message.Text,
                State = message.State
            }).ToList();

            _messagesQueryableMock = _messages.AsQueryable().BuildMock();
            _message = _messages.FirstOrDefault();
            _messageDto = _messagesDto.FirstOrDefault();
        }

        private List<User> GetUsers()
        {
            return new List<User>
            {
                new User
                {
                    Id = 1, UserRoom = new UserRoom { Location = new Location { Id = 1 }, RoomNumber = "1", LocationId = 1 }
                },
                new User
                {
                    Id = 2, UserRoom = new UserRoom { Location = new Location { Id = 2 }, RoomNumber = "2", LocationId = 2 }
                }
            };
        }
        private bool ListsHasSameElements(List<SuggestionMessage> obj1, List<SuggestionMessage> obj2)
        {
            var tempList1 = obj1.Except(obj2).ToList();
            var tempList2 = obj2.Except(obj1).ToList();

            return !(tempList1.Any() || tempList2.Any());
        }

        #endregion
    }
}
