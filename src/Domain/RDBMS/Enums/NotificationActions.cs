using System;

namespace Domain.RDBMS.Enums
{
    [Flags]
    public enum NotificationActions
    {
        None = 0,
        Open = 1,
        Request = 2,
        StartReading = 4,
        Message = 8
    }
}
