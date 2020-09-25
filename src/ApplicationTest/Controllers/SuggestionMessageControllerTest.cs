using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dto;
using Application.Dto.QueryParams;
using Application.Services.Interfaces;
using BookCrossingBackEnd.Controllers;
using Domain.RDBMS.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;


namespace ApplicationTest.Controllers
{
    public class SuggestionMessageControllerTest
    {
        private Mock<ISuggestionMessageService> _messageService;
        private SuggestionMessageController _messageController;
        private Mock<ILogger<SuggestionMessageController>> _logger;
        private IEnumerable<SuggestionMessage> _messages;
        private IEnumerable<SuggestionMessageDto> _messagesDto;
        private SuggestionMessage _message;
        private SuggestionMessageDto _messageDto;
        [OneTimeSetUp]
        public void Setup()
        {
            _messageService = new Mock<ISuggestionMessageService>();
            _logger = new Mock<ILogger<SuggestionMessageController>>();
            _messageController = new SuggestionMessageController(_messageService.Object, _logger.Object);
            MockData();
        }

        private void MockData()
        {
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
            });

            _message = _messages.FirstOrDefault();
            _messageDto = _messagesDto.FirstOrDefault();
        }

        [Test]
        public async Task GetAllMessagesAsync_Success_ReturnsOkObject()
        {
            _messageService.Setup(s => s.GetAll()).ReturnsAsync(_messagesDto);

            var result = await _messageController.GetAllMessages();

            var okResult = result.Result;
            okResult.Should().BeOfType<OkObjectResult>();
        }

        [Test]
        public async Task GetAllMessages_AnyFullPaginationQueryParams_ReturnsPaginatedDtoListOfMessageGetDto()
        {
            _messageService.Setup(s => s.GetAll(It.IsAny<FullPaginationQueryParams>()))
                .ReturnsAsync(new PaginationDto<SuggestionMessageDto>()
                {
                    Page = _messagesDto.ToList(),
                    TotalCount = 1
                });

            var result = await _messageController.GetAllMessages(It.IsAny<FullPaginationQueryParams>());

            result.Should().BeOfType<ActionResult<PaginationDto<SuggestionMessageDto>>>();
        }

        [Test]
        public async Task GetMessageAsync_MessageExists_ReturnsOkObjectResult()
        {
            _messageService.Setup(s => s.GetById(It.IsAny<int>())).ReturnsAsync(_messageDto);

            var messageResult = await _messageController.GetMessage(It.IsAny<int>());

            var okResult = messageResult.Result;
            okResult.Should().BeOfType<OkObjectResult>();
        }

        [Test]
        public async Task GetMessageAsync_MessageDoesNotExist_Returns_NotFoundResult()
        {
            _messageService.Setup(s => s.GetById(It.IsAny<int>())).ReturnsAsync(null as SuggestionMessageDto);

            var result = await _messageController.GetMessage(It.IsAny<int>());

            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public async Task PutMessage_MessageExists_Returns_NoContent()
        {
            _messageService.Setup(s => s.Update(It.IsAny<SuggestionMessageDto>())).ReturnsAsync(true);

            var result = await _messageController.PutMessage(It.IsAny<SuggestionMessageDto>());

            result.Should().BeOfType<NoContentResult>();
        }

        [Test]
        public async Task PutMessage_MessageDoesNotExist_Return_NotFound()
        {
            _messageService.Setup(s => s.Update(It.IsAny<SuggestionMessageDto>())).ReturnsAsync(false);

            var result = await _messageController.PutMessage(It.IsAny<SuggestionMessageDto>());

            result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public async Task PostMessage_Returns_CreatedAtActionResult()
        {
            _messageService.Setup(m => m.Add(It.IsAny<SuggestionMessageDto>())).ReturnsAsync(_message.Id);

            var createdAtActionResult = await _messageController.PostMessage(_messageDto);

            createdAtActionResult.Result.Should().BeOfType<CreatedResult>();
        }

        [Test]
        public async Task DeleteMessage_MessageExists_Returns_OkResult()
        {
            _messageService.Setup(s => s.Remove((It.IsAny<int>()))).ReturnsAsync(_messageDto);

            var result = await _messageController.DeleteMessage(It.IsAny<int>());

            result.Should().BeOfType<OkResult>();
        }

        [Test]
        public async Task DeleteMessage_DoesNotExist_Returns_NotFoundResult()
        {
            _messageService.Setup(s => s.Remove(It.IsAny<int>())).ReturnsAsync(null as SuggestionMessageDto);

            var result = await _messageController.DeleteMessage(It.IsAny<int>());

            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
