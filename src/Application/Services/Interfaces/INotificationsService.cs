using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Dto;
using Domain.RDBMS.Enums;

namespace Application.Services.Interfaces
{
    public interface INotificationsService
    {
        Task NotifyAsync(int userId, string message, string messageUk, int? bookId = null,
            NotificationActions action = NotificationActions.None);

        Task<IEnumerable<NotificationDto>> GetAllForCurrentUserAsync();

        Task MarkAsReadAsync(int id);

        Task MarkAllAsReadForCurrentUserAsync();

        Task AddAsync(MessageDto message);

        Task AddAsync(string message);

        Task RemoveAsync(int id);

        Task RemoveAllForCurrentUserAsync();
    }
}
