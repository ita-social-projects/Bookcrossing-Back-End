using System;
using System.Collections.Generic;
using System.Text;
using Domain.RDBMS.Enums;

namespace Application.Dto.Settings
{
    public class DescribedSettingDto: SettingDto
    {
        public string Description { get; set; }
    }
}
