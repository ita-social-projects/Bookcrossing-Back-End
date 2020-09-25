using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Dto;
using Application.Dto.QueryParams;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.RDBMS;
using Domain.RDBMS.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Implementation
{
    public class SuggestionMessageService : ISuggestionMessageService
    {
        private readonly IRepository<SuggestionMessage> _messageRepository;
        private readonly IMapper _mapper;
        private readonly IPaginationService _paginationService;

        public SuggestionMessageService(IRepository<SuggestionMessage> messageRepository, IMapper mapper, IPaginationService paginationService)
        {
            _messageRepository = messageRepository;
            _paginationService = paginationService;
            _mapper = mapper;
        }

        public async Task<int> Add(SuggestionMessageDto messageDto)
        {
            var message = _mapper.Map<SuggestionMessage>(messageDto);
            _messageRepository.Add(message);
            await _messageRepository.SaveChangesAsync();

            return message.Id;
        }

        public async Task<IEnumerable<SuggestionMessageDto>> GetAll()
        {
            return _mapper.Map<IEnumerable<SuggestionMessageDto>>(await _messageRepository.GetAll().AsNoTracking()
                                                                                   .Include(u => u.User)
                                                                                   .ThenInclude(r => r.Role)
                                                                                   .ToArrayAsync());
        }

        public async Task<PaginationDto<SuggestionMessageDto>> GetAll(FullPaginationQueryParams fullPaginationQuery)
        {   
            var query = _messageRepository.GetAll().AsNoTracking().Include(u => u.User).ThenInclude(r => r.Role).IgnoreQueryFilters();

            return await _paginationService.GetPageAsync<SuggestionMessageDto, SuggestionMessage>(query, fullPaginationQuery);
        }

        public async Task<SuggestionMessageDto> GetById(int messageId)
        {
            return _mapper.Map<SuggestionMessageDto>(await _messageRepository.GetAll().AsNoTracking()
                                                                                      .Include(u => u.User)
                                                                                      .ThenInclude(r => r.Role)
                                                                                      .AsNoTracking()
                                                                                      .FirstOrDefaultAsync(m => m.Id == messageId));
        }

        public async Task<SuggestionMessageDto> Remove(int messageId)
        {
            var message = await _messageRepository.FindByIdAsync(messageId);
            if (message == null)
            {
                return null;
            }  

            _messageRepository.Remove(message);
            await _messageRepository.SaveChangesAsync();

            return _mapper.Map<SuggestionMessageDto>(message);
        }

        public async Task<bool> Update(SuggestionMessageDto messageDto)
        {
            var message = _mapper.Map<SuggestionMessage>(messageDto);
            _messageRepository.Update(message);
            var affectedRows = await _messageRepository.SaveChangesAsync();

            return affectedRows > 0;
        }
    }
}
