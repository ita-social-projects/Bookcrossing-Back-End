using System;
using Domain.RDBMS.Enums;

namespace Domain.RDBMS.Entities
{
    public class Notification : IEntityBase
    {
        public int Id { get; set; }

        public string Message { get; set; }

        public int UserId { get; set; }

        public int? ReceiverUserId { get; set; }

        public int? BookId { get; set; }

        public NotificationAction Action { get; set; }

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
