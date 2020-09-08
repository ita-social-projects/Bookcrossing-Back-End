using System;

namespace Domain.RDBMS.Enums
{
    [Flags]
    public enum NotificationAction
    {
        None,
        Open,
        Request,
        StartReading
    }
}
