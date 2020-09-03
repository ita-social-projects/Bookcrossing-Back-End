using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Core;
using System.Linq;
using System.Reflection;
using System.Text;
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

        public async Task<DescribedSettingDto> GetSetting(SettingKey key)
        {
            var setting = await GetSettingEntity(key);

            return _mapper.Map<DescribedSettingDto>(setting);
        }

        public async Task<int> GetInt(SettingKey key)
        {
            return Convert.ToInt32(await GetSettingValue(key));
        }

        public async Task<double> GetDouble(SettingKey key)
        {
            return Convert.ToDouble(await GetSettingValue(key));
        }

        public async Task<string> GetString(SettingKey key)
        {
            return await GetSettingValue(key);
        }

        public async Task<TimeSpan> GetTimeSpan(SettingKey key)
        {
            return TimeSpan.Parse(await GetSettingValue(key));
        }

        public async Task SetSettingValue(SettingKey key, SettingDto settingDto)
        {
            var setting = await GetSettingEntity(key);
            if (setting == null)
            {
                throw new ObjectNotFoundException($"Setting {key.ToString()} was not found");
            }
            
            setting.Value = settingDto.Value;
            await _settingsRepository.SaveChangesAsync();
        }

        protected virtual async Task<Setting> GetSettingEntity(SettingKey key)
        {
            var namespaceAttribute = GetEnumAttribute<NamespaceAttribute>(key);
            var namespaceValue = namespaceAttribute?.Namespace ?? Setting.DefaultNamespace;

            return await _settingsRepository.FindByIdAsync(namespaceValue, key);
        }

        protected virtual async Task<string> GetSettingValue(SettingKey key)
        {
            var setting = await GetSettingEntity(key);

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
