using System;
using System.Collections.Generic;
using System.Text;
using Domain.RDBMS.Enums;

namespace Application.Dto.Settings
{
    public class SettingDto
    {
        public SettingKey Key { get; set; }

        public string Value { get; set; }
    }
}
