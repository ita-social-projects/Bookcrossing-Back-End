using System.ComponentModel;
using Domain.RDBMS.Attributes;

namespace Domain.RDBMS.Enums
{
    public enum SettingKey
    {
        [Namespace("Timespans")]
        [DefaultValue("10.00:00:00")]
        [Description("'Request autocancel' - is the time between book 'Request' and its automatic cancelation. By changing this value - you change time period for users to receive book from book holder.")]
        RequestAutoCancelTimespan,
        [Namespace("Timespans")]
        [DefaultValue("9.00:00:00")]
        [Description("'Reminder for user' - is the time between book's ‘Request' and sending a reminder for the user about autocancelation.")]
        RequestAutoCancelRemindTimespan
    }
}
