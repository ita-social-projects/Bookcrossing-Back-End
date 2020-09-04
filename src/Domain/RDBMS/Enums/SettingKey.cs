using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;
using Domain.RDBMS.Attributes;

namespace Domain.RDBMS.Enums
{
    public enum SettingKey
    {
        [Namespace("Timespans")]
        [DefaultValue("10.00:00:00")]
        [Description("Request auto-cancel - is the time between book 'Request' and automatic cancel of request." +
        "By changing this value - you change time period for request to be canceled")]
        RequestAutoCancelTimespan
    }
}
