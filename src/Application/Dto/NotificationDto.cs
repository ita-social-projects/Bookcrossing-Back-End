using System;
using Domain.RDBMS.Enums;

namespace Application.Dto
{
    public class NotificationDto
    {
        public int Id { get; set; }

        public string Message { get; set; }

        public int? BookId { get; set; }

        public NotificationAction Action { get; set; }

        public bool IsSeen { get; set; }

        public DateTime Date { get; set; }
    }
}
