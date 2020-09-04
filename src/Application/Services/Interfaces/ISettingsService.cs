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
        Task<DescribedSettingDto> GetSettingAsync(SettingKey key);

        Task<int> GetIntAsync(SettingKey key);

        Task<double> GetDoubleAsync(SettingKey key);

        Task<string> GetStringAsync(SettingKey key);

        Task<TimeSpan> GetTimeSpanAsync(SettingKey key);

        Task SetSettingValueAsync(SettingKey key, SettingDto settingDto);
    }
}
