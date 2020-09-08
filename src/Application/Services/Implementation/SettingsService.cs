using System;
using System.ComponentModel;
using System.Data.Entity.Core;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Application.Dto.Settings;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.RDBMS;
using Domain.RDBMS.Attributes;
using Domain.RDBMS.Entities;
using Domain.RDBMS.Enums;

namespace Application.Services.Implementation
{
    public class SettingsService : ISettingsService
    {
        private readonly IRepository<Setting> _settingsRepository;
        private readonly IMapper _mapper;

        public SettingsService(IRepository<Setting> settingsRepository, IMapper mapper)
        {
            _settingsRepository = settingsRepository;
            _mapper = mapper;
        }

        public async Task<DescribedSettingDto> GetSettingAsync(SettingKey key)
        {
            var setting = await GetSettingEntityAsync(key);
            DescribedSettingDto settingDto;
            if (setting == null)
            {
                var descriptionAttribute = GetEnumAttribute<DescriptionAttribute>(key);

                settingDto = new DescribedSettingDto
                {
                    Description = descriptionAttribute?.Description,
                    Key = key
                };
            }
            else
            {
                settingDto = _mapper.Map<DescribedSettingDto>(setting);
            }

            var defaultValueAttribute = GetEnumAttribute<DefaultValueAttribute>(key);
            settingDto.DefaultValue = defaultValueAttribute?.Value?.ToString();
            return settingDto;
        }

        public async Task<int> GetIntAsync(SettingKey key)
        {
            return Convert.ToInt32(await GetSettingValueAsync(key));
        }

        public async Task<double> GetDoubleAsync(SettingKey key)
        {
            return Convert.ToDouble(await GetSettingValueAsync(key), new NumberFormatInfo());
        }

        public async Task<string> GetStringAsync(SettingKey key)
        {
            return await GetSettingValueAsync(key);
        }

        public async Task<TimeSpan> GetTimeSpanAsync(SettingKey key)
        {
            if (!TimeSpan.TryParse(await GetSettingValueAsync(key), out var timeSpan))
            {
                var defaultValueAttribute = GetEnumAttribute<DefaultValueAttribute>(key);
                timeSpan = TimeSpan.Parse(defaultValueAttribute?.Value?.ToString() ?? "0");
            }

            return timeSpan;
        }

        public async Task SetSettingValueAsync(SettingKey key, SettingDto settingDto)
        {
            var setting = await GetSettingEntityAsync(key);
            if (setting == null)
            {
                var namespaceAttribute = GetEnumAttribute<NamespaceAttribute>(key);
                var descriptionAttribute = GetEnumAttribute<DescriptionAttribute>(key);

                var namespaceValue = namespaceAttribute?.Namespace ?? Setting.DefaultNamespace;
                var descriptionValue = descriptionAttribute?.Description;
                setting = new Setting
                {
                    Namespace = namespaceValue,
                    Description = descriptionValue,
                    Key = key
                };

                _settingsRepository.Add(setting);
            }

            setting.Value = settingDto.Value;
            await _settingsRepository.SaveChangesAsync();
        }

        protected virtual async Task<Setting> GetSettingEntityAsync(SettingKey key)
        {
            var namespaceAttribute = GetEnumAttribute<NamespaceAttribute>(key);
            var namespaceValue = namespaceAttribute?.Namespace ?? Setting.DefaultNamespace;

            return await _settingsRepository.FindByIdAsync(namespaceValue, key);
        }

        protected virtual async Task<string> GetSettingValueAsync(SettingKey key)
        {
            var setting = await GetSettingEntityAsync(key);

            var defaultValueAttribute = GetEnumAttribute<DefaultValueAttribute>(key);
            var defaultValue = defaultValueAttribute?.Value?.ToString();

            return setting?.Value ?? defaultValue;
        }

        protected virtual T GetEnumAttribute<T>(SettingKey key) where T : Attribute
        {
            var enumType = key.GetType();
            var keyInfo = enumType.GetMember(key.ToString())
                .FirstOrDefault(member => member.DeclaringType == enumType);

            return keyInfo?.GetCustomAttribute<T>();
        }
    }
}
