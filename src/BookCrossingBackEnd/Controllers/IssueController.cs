using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Dto;
using Application.Dto.QueryParams;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BookCrossingBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IssueController : ControllerBase
    {

        private readonly IIssueService _issueService;
        private readonly ILogger _logger;

        public IssueController(IIssueService issueService, ILogger<IssueController> logger)
        {
            _issueService = issueService;
            _logger = logger;
        }

        [HttpGet("{id:min(1)}")]
        public async Task<ActionResult<IssueDto>> GetIssue(int id)
        {
            _logger.LogInformation("Getting issue {id}", id);
            var issue = await _issueService.GetById(id);
            if (issue == null)
            {
                _logger.LogWarning("GetIssue({Id}) NOT FOUND", id);
                return NotFound();
            }
            return Ok(issue);
        }

        // GET: api/Issues
        [HttpGet]
        public async Task<ActionResult<List<IssueDto>>> GetAllIssues()
        {
            _logger.LogInformation("Getting all issues");
            return Ok(await _issueService.GetAll());
        }


        // PUT: api/Issue
        [HttpPut]
        public async Task<IActionResult> PutIssue(IssueDto issueDto)
        {
            _logger.LogInformation("Update issue {IssueDto}", issueDto);
            var updated = await _issueService.Update(issueDto);
            if (!updated)
            {
                _logger.LogWarning("Update issue ({IssueDto}) NOT FOUND", issueDto);
                return NotFound();
            }
            return NoContent();
        }

        // POST: api/Issue
        [HttpPost]
        public async Task<ActionResult<IssueDto>> PostIssue([FromBody] IssueDto issueDto)
        {
            _logger.LogInformation("Post issue {IssueDto}", issueDto);
            var insertedIssue = await _issueService.Add(issueDto);
            return CreatedAtAction("GetIssue", new { id = insertedIssue.Id }, insertedIssue);
        }

        // DELETE: api/Issue/id
        [HttpDelete("{id:min(1)}")]
        public async Task<IActionResult> DeleteIssue(int id)
        {
            _logger.LogInformation("Delete issue {Id}", id);
            var issue = await _issueService.Remove(id);
            if (issue == false)
            {
                _logger.LogWarning("Delete issue ({Id}) NOT FOUND", id);
                return NotFound();
            }
            return Ok();
        }

        [HttpGet("paginated")]
        public async Task<ActionResult<PaginationDto<IssueDto>>> GetAllIssues([FromQuery] FullPaginationQueryParams fullPaginationQuery)
        {
            _logger.LogInformation("Getting all paginated issues");
            return await _issueService.GetAll(fullPaginationQuery);
        }

    }
}
