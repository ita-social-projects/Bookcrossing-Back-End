using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Dto;
using Application.Dto.QueryParams;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BookCrossingBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuggestionMessageController : ControllerBase
    {
        private readonly ISuggestionMessageService _messageService;
        private readonly ILogger _logger;

        public SuggestionMessageController(ISuggestionMessageService messageService, ILogger<SuggestionMessageController> logger)
        {
            _messageService = messageService;
            _logger = logger;
        }

        [HttpGet("{id:min(1)}")]
        public async Task<ActionResult<SuggestionMessageDto>> GetMessage(int id)
        {
            _logger.LogInformation($"Getting message {id}", id);
            var message = await _messageService.GetById(id);
            if (message == null)
            {
                _logger.LogWarning($"GetMessage({id}) NOT FOUND", id);

                return NotFound();
            }

            return Ok(message);
        }

        // GET: api/SuggestionMessage
        [HttpGet]
        public async Task<ActionResult<List<SuggestionMessageDto>>> GetAllMessages()
        {
            _logger.LogInformation("Getting all messages");

            return Ok(await _messageService.GetAll());
        }

        [HttpGet("paginated")]
        public async Task<ActionResult<PaginationDto<SuggestionMessageDto>>> GetAllMessages([FromQuery] FullPaginationQueryParams fullPaginationQuery)
        {
            _logger.LogInformation("Getting all paginated messages");

            return Ok(await _messageService.GetAll(fullPaginationQuery));
        }

        // PUT: api/SuggestionMessage
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutMessage(SuggestionMessageDto messageDto)
        {
            _logger.LogInformation($"Update message {messageDto}", messageDto);
            var updated = await _messageService.Update(messageDto);
            if (!updated)
            {
                _logger.LogWarning($"Update message ({messageDto}) NOT FOUND", messageDto);

                return NotFound();
            }

            return NoContent();
        }

        // POST: api/SuggestionMessage
        [HttpPost]
        public async Task<ActionResult<SuggestionMessageDto>> PostMessage([FromBody] SuggestionMessageDto messageDto)
        {
            _logger.LogInformation($"Post message {messageDto}", messageDto);
            var insertedMessage = await _messageService.Add(messageDto);

            return Created("GetMessage", insertedMessage);
        }

        // DELETE: api/SuggestionMessage/id
        [HttpDelete("{id:min(1)}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            _logger.LogInformation($"Delete message {id}", id);
            var message = await _messageService.Remove(id);
            if (message == null)
            {
                _logger.LogWarning($"Delete message ({id}) NOT FOUND", id);

                return NotFound();
            }

            return Ok();
        }
    }
}
