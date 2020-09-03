using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Application.Dto.Settings;
using Domain.RDBMS.Entities;
using Domain.RDBMS.Enums;

namespace Application.Services.Interfaces
{
    public interface ISettingsService
    {
        Task<DescribedSettingDto> GetSetting(SettingKey key);

        Task<int> GetInt(SettingKey key);

        Task<double> GetDouble(SettingKey key);

        Task<string> GetString(SettingKey key);

        Task<TimeSpan> GetTimeSpan(SettingKey key);

        Task SetSettingValue(SettingKey key, SettingDto settingDto);
    }
}
