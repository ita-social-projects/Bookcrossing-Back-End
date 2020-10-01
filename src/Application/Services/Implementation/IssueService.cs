using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Dto;
using Application.Dto.QueryParams;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.RDBMS;
using Domain.RDBMS.Entities;

namespace Application.Services.Implementation
{
    public class IssueService : IIssueService
    {
        private readonly IRepository<Issue> _issueRepository;
        private readonly IMapper _mapper;
        private readonly IPaginationService _paginationService;
        public IssueService(IRepository<Issue> issueRepository, IMapper mapper, IPaginationService paginationService)
        {
            _issueRepository = issueRepository;
            _paginationService = paginationService;
            _mapper = mapper;
        }
            
        public async Task<IssueDto> GetById(int issueId)
        {
            return _mapper.Map<IssueDto>(await _issueRepository.FindByIdAsync(issueId));
        }

        public async Task<IssueDto> Add(IssueDto issueDto)
        {
            var issue = _mapper.Map<Issue>(issueDto);
            _issueRepository.Add(issue);
            await _issueRepository.SaveChangesAsync();

            return _mapper.Map<IssueDto>(issue);
        }

        public async Task<bool> Remove(int issueId)
        {
            var issue = await _issueRepository.FindByIdAsync(issueId);
            if (issue == null)
            {
                return false;
            }
            _issueRepository.Remove(issue);
            await _issueRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> Update(IssueDto issueDto)
        {
            var issue = _mapper.Map<Issue>(issueDto);
            _issueRepository.Update(issue);
            var affectedRows = await _issueRepository.SaveChangesAsync();

            return affectedRows > 0;
        }

        public async Task<IEnumerable<IssueDto>> GetAll()
        {
            return _mapper.Map<IEnumerable<IssueDto>>(_issueRepository.GetAll());
        }

        public async Task<PaginationDto<IssueDto>> GetAll(FullPaginationQueryParams fullPaginationQuery)
        {
            var query = _issueRepository.GetAll();
                
            return await _paginationService.GetPageAsync<IssueDto, Issue>(query, fullPaginationQuery);
        }

    }

}
