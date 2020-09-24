using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Dto;
using Application.Services.Implementation;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.RDBMS;
using Domain.RDBMS.Entities;
using FluentAssertions;
using Infrastructure.RDBMS;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;

namespace ApplicationTest.Services
{
    [TestFixture]
    public class SuggestionMessageServiceTest
    {
        private BookCrossingContext _context;
        private SuggestionMessageService _messageService;
        private Mock<IRepository<SuggestionMessage>> _messageRepositoryMock;
        private Mock<IPaginationService> _paginationServiceMock;
        private IMapper _mapper;

        private IEnumerable<SuggestionMessage> _messages;
        private Mock<IQueryable<SuggestionMessage>> _messagesQueryableMock;
        private IEnumerable<SuggestionMessageDto> _messagesDto;
        private SuggestionMessage _message;
        private SuggestionMessageDto _messageDto;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _messageRepositoryMock = new Mock<IRepository<SuggestionMessage>>();
            _paginationServiceMock = new Mock<IPaginationService>();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new Application.MapperProfilers.SuggestionMessageProfile());
            });
            _mapper = mappingConfig.CreateMapper();

            var options = new DbContextOptionsBuilder<BookCrossingContext>().UseInMemoryDatabase(databaseName: "Fake DB").ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning)).Options;
            _context = new BookCrossingContext(options);
            
            _messageService = new SuggestionMessageService(_messageRepositoryMock.Object, _mapper, _paginationServiceMock.Object);

            MockData();
        }

        #region Data
        private void MockData()
        {
            _messages = new List<SuggestionMessage>
            {
                new SuggestionMessage()
                {
                    Id = 1,
                    Summary = "Error",
                    Text = "Some text",
                    State = MessageState.Read,
                },
                new SuggestionMessage()
                {
                    Id = 2,
                    Summary = "Suggest",
                    Text = "Some text 2",
                    State = MessageState.Unread,
                },
            };

            _messagesDto = _messages.Select(message => new SuggestionMessageDto
            {
                Id = message.Id,
                Summary = message.Summary,
                Text = message.Text,
                State = message.State
            });

            _messagesQueryableMock = _messages.AsQueryable().BuildMock();
            _message = _messages.FirstOrDefault();
            _messageDto = _messagesDto.FirstOrDefault();
        }



        #endregion


        [SetUp]
        public void SetUp()
        {
            _messageRepositoryMock.Reset();
        }

        [Test]
        public async Task GetMessageById_MessageExists_ReturnsMessageDto()
        {
            _messageRepositoryMock.Setup(s => s.GetAll()).Returns(_messagesQueryableMock.Object);

            var messageResult = await _messageService.GetById(_message.Id);

            messageResult.Should().BeEquivalentTo(_messageDto);
        }

        [Test]
        public async Task GetAll_MessagesExist_ReturnsListOfMessageDtos()
        {
            _messageRepositoryMock.Setup(s => s.GetAll()).Returns(_messagesQueryableMock.Object);

            var messageResult = await _messageService.GetAll();

            messageResult.Should().BeEquivalentTo(_messagesDto);
        }

        [Test]
        public async Task Remove_MessageExists_RemoveMessageInDb()
        {
            _messageRepositoryMock.Setup(s => s.FindByIdAsync(_message.Id))
                .ReturnsAsync(_message);

            var messageResult = await _messageService.Remove(_message.Id);

            _messageRepositoryMock.Verify(obj => obj.Remove(_message), Times.Once);
        }

        [Test]
        public async Task Remove_MessageExists_ShouldSaveChangesAfterRemove()
        {
            _messageRepositoryMock.Setup(s => s.FindByIdAsync(_message.Id))
                .ReturnsAsync(_message);

            var messageResult = await _messageService.Remove(_message.Id);

            _messageRepositoryMock.Verify(obj => obj.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task Remove_MessageExists_ReturnsMessageDto()
        {
            _messageRepositoryMock.Setup(s => s.FindByIdAsync(_message.Id))
                .ReturnsAsync(_message);

            var messageResult = await _messageService.Remove(_message.Id);

            messageResult.Should().BeEquivalentTo(_messageDto);
        }

        [Test]
        public async Task Remove_MessageNotExist_ReturnsNull()
        {
            _messageRepositoryMock.Setup(s => s.FindByIdAsync(_message.Id))
                .ReturnsAsync(value: null);

            var messageResult = await _messageService.Remove(_message.Id);

            messageResult.Should().BeNull();
        }

        [Test]
        public async Task Update_ShouldUpdateMessageInDatabase()
        {
            await _messageService.Update(_messageDto);

            _messageRepositoryMock.Verify(obj => obj.Update(It.IsAny<SuggestionMessage>()));
        }

        [Test]
        public async Task Update_MessageUpdated_ShouldSaveChangesInDatabase()
        {
            await _messageService.Update(_messageDto);

            _messageRepositoryMock.Verify(obj => obj.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task Update_MessageUpdated_ReturnsTrue()
        {
            _messageRepositoryMock.Setup(s => s.SaveChangesAsync())
                .ReturnsAsync(1);

            var res = await _messageService.Update(_messageDto);

            res.Should().BeTrue();
        }

        [Test]
        public async Task Update_MessageNotUpdated_ReturnsTrue()
        {
            _messageRepositoryMock.Setup(s => s.SaveChangesAsync())
                .ReturnsAsync(0);

            var res = await _messageService.Update(_messageDto);

            res.Should().BeFalse();
        }

        [Test]
        public async Task Add_ShouldAddMessageToDatabase()
        {
            var res = await _messageService.Add(_messageDto);

            _messageRepositoryMock.Verify(obj => obj.Add(It.IsAny<SuggestionMessage>()),Times.Once);
        }

        [Test]
        public async Task Add_MessageAdded_ShouldSaveChangesInDatabase()
        {
            await _messageService.Add(_messageDto);

            _messageRepositoryMock.Verify(obj => obj.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task Add_MessageAdded_ShouldReturnsId()
        {
            var res = await _messageService.Add(_messageDto);

            res.Should().Be(_messageDto.Id);
        }
    }
}
