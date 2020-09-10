using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Dto;
using Domain.RDBMS.Enums;

namespace Application.Services.Interfaces
{
    public interface INotificationsService
    {
        Task NotifyAsync(int userId, string message, int? bookId = null,
            NotificationAction action = NotificationAction.None);

        Task<IEnumerable<NotificationDto>> GetAllForCurrentUserAsync();

        Task MarkAsReadAsync(int id);

        Task MarkAllAsReadForCurrentUserAsync();

        Task AddAsync(MessageDto message);

        Task RemoveAsync(int id);

        Task RemoveAllForCurrentUserAsync();
    }
}
