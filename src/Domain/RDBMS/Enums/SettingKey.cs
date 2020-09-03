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
        RequestAutoCancelTimespan
    }
}
