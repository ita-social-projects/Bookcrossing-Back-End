using System;
using System.ComponentModel;
using System.Data.Entity.Core;
using System.Globalization;
using System.Threading.Tasks;
using Application.Dto.Settings;
using Application.Services.Implementation;
using AutoMapper;
using Domain.RDBMS;
using Domain.RDBMS.Attributes;
using Domain.RDBMS.Entities;
using Domain.RDBMS.Enums;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

namespace ApplicationTest.Services
{
    [TestFixture]
    internal class SettingsServiceTests
    {
        private Mock<IRepository<Setting>> _settingsRepositoryMock;
        private Mock<IMapper> _mapperMock;
        private SettingsServiceProxy _service;

        private Setting _setting;
        private Setting _settingWithDefaultNamespace;
        private Setting _settingWithNullValue;
        private SettingDto _settingDto;
        private DescribedSettingDto _describedSettingDto;
        private DefaultValueAttribute _defaultValueAttribute;
        private NamespaceAttribute _namespaceAttribute;
        private DescriptionAttribute _descriptionAttribute;

        [OneTimeSetUp]
        public void InitializeClass()
        {
            _settingsRepositoryMock = new Mock<IRepository<Setting>>();
            _mapperMock = new Mock<IMapper>();
            _service = new SettingsServiceProxy(_settingsRepositoryMock.Object, _mapperMock.Object, this);
        }

        [SetUp]
        public void InitializeTest()
        {
            _settingsRepositoryMock.Invocations.Clear();
            MockData();
        }

        [Test]
        public async Task GetSettingAsync_NoExceptionsWasThrown_ReturnsDescribedSettingDtoObject()
        {
            _service.GetSettingEntityAsyncMock = async key =>
            {
                if (key == _setting.Key)
                {
                    return await Task.FromResult(_setting);
                }

                return null;
            };
            _mapperMock.Setup(obj => obj.Map<DescribedSettingDto>(_setting))
                .Returns(_describedSettingDto);

            var result = await _service.GetSettingAsync(_setting.Key);

            result
                .Should()
                .Be(_describedSettingDto);
        }

        [Test]
        public async Task GetSettingAsync_GetSettingEntityAsyncReturnedNull_ReturnsSettingDtoWithValuesFromAttributes()
        {
            var settingKey = SettingKey.RequestAutoCancelRemindTimespan;
            _service.GetSettingEntityAsyncMock = key => null;
            _service.GetEnumAttributeMock = (key, type) =>
            {
                if (key == settingKey && type == _descriptionAttribute.GetType())
                {
                    return _descriptionAttribute;
                }

                if (key == settingKey && type == _defaultValueAttribute.GetType())
                {
                    return _defaultValueAttribute;
                }

                return null;
            };

            var result = await _service.GetSettingAsync(settingKey);

            result.DefaultValue.Should().Be(_defaultValueAttribute.Value?.ToString());
            result.Description.Should().Be(_descriptionAttribute.Description);
            result.Key.Should().Be(settingKey);
            result.Value.Should().BeNull();
        }

        [Test]
        public async Task GetSettingEntityAsync_GetEnumAttributeAsyncReturnedNamespaceAttributeWithValue_ReturnsSettingEntityWithNamespaceFromAttribute()
        {
            _service.GetEnumAttributeMock = (key, type) =>
            {
                if (type == typeof(NamespaceAttribute) && key == _setting.Key)
                {
                    return new NamespaceAttribute(_setting.Namespace);
                }

                return null;
            };
            _settingsRepositoryMock.Setup(obj => obj.FindByIdAsync(_setting.Namespace, _setting.Key))
                .ReturnsAsync(_setting);

            var result = await _service.GetSettingEntityAsyncProxy(_setting.Key);

            result
                .Should()
                .Be(_setting);
        }

        [Test]
        public async Task GetSettingEntityAsync_GetEnumAttributeAsyncReturnedNull_ReturnsSettingEntityWithDefaultNamespace()
        {
            _service.GetEnumAttributeMock = (key, type) => null;
            _settingsRepositoryMock.Setup(obj => obj.FindByIdAsync(Setting.DefaultNamespace, _settingWithDefaultNamespace.Key))
                .ReturnsAsync(_settingWithDefaultNamespace);

            var result = await _service.GetSettingEntityAsyncProxy(_settingWithDefaultNamespace.Key);

            result
                .Should()
                .Be(_settingWithDefaultNamespace);
        }

        [Test]
        public async Task GetSettingValueAsync_GetSettingEntityAsyncReturnedSettingEntityWithNotNullValue_ReturnsValueOfReturnedSetting()
        {
            _service.GetSettingEntityAsyncMock = async key =>
            {
                if (key == _setting.Key)
                {
                    return await Task.FromResult(_setting);
                }

                return null;
            };

            var result = await _service.GetSettingValueAsyncProxy(_setting.Key);

            result
                .Should()
                .Be(_setting.Value);
        }

        [Test]
        public async Task GetSettingValueAsync_GetSettingEntityAsyncReturnedSettingEntityWithNullValue_ReturnsDefaultValueOfReturnedSetting()
        {
            _service.GetSettingEntityAsyncMock = async key =>
            {
                if (key == _settingWithNullValue.Key)
                {
                    return await Task.FromResult(_settingWithNullValue);
                }

                return null;
            };
            _service.GetEnumAttributeMock = (key, type) =>
            {
                if (key == _settingWithNullValue.Key && type == _defaultValueAttribute.GetType())
                {
                    return _defaultValueAttribute;
                }

                return null;
            };

            var result = await _service.GetSettingValueAsyncProxy(_settingWithNullValue.Key);

            result
                .Should()
                .Be(_defaultValueAttribute.Value?.ToString());
        }

        [Test]
        public async Task GetSettingValueAsync_GetSettingEntityAsyncReturnedNull_ReturnsDefaultValueOfSettingKeysAttribute()
        {
            const SettingKey SettingKey = SettingKey.RequestAutoCancelTimespan;
            _service.GetSettingEntityAsyncMock = key => null;
            _service.GetEnumAttributeMock = (key, type) =>
            {
                if (key == SettingKey && type == _defaultValueAttribute.GetType())
                {
                    return _defaultValueAttribute;
                }

                return null;
            };

            var result = await _service.GetSettingValueAsyncProxy(SettingKey);

            result
                .Should()
                .Be(_defaultValueAttribute.Value?.ToString());
        }

        [Test]
        public async Task SetSettingValueAsync_GetSettingEntityAsyncReturnedNull_ThrowsObjectNotFoundException()
        {
            var settingKey = SettingKey.RequestAutoCancelRemindTimespan;
            _service.GetSettingEntityAsyncMock = key => null;
            _service.GetEnumAttributeMock = (key, type) =>
            {
                if (key == settingKey && type == _descriptionAttribute.GetType())
                {
                    return _descriptionAttribute;
                }

                if (key == settingKey && type == _defaultValueAttribute.GetType())
                {
                    return _defaultValueAttribute;
                }

                if (key == settingKey && type == _namespaceAttribute.GetType())
                {
                    return _namespaceAttribute;
                }

                return null;
            };

            await _service.SetSettingValueAsync(settingKey, _settingDto);

            _settingsRepositoryMock.Verify(
                obj => obj.Add(It.Is<Setting>(setting => setting.Key == settingKey &&
                                                         setting.Description == _descriptionAttribute.Description && 
                                                         setting.Namespace == _namespaceAttribute.Namespace)), 
                Times.Once);
            _settingsRepositoryMock.Verify(obj => obj.SaveChangesAsync());
        }

        [Test]
        public async Task SetSettingValueAsync_GetSettingEntityAsyncReturnedSettingEntity_SetsNewValueToSettingEntityAndSaveChangesToDatabase()
        {
            _service.GetSettingEntityAsyncMock = async key =>
            {
                if (key == _setting.Key)
                {
                    return await Task.FromResult(_setting);
                }

                return null;
            };

            await _service.SetSettingValueAsync(_setting.Key, _settingDto);

            _settingsRepositoryMock.Verify(obj => obj.SaveChangesAsync());

            _setting.Value
                .Should()
                .Be(_settingDto.Value);
        }

        [Test]
        public async Task GetIntAsync_NoExceptionsWasThrown_ReturnsValueConvertedToInt()
        {
            var settingKey = SettingKey.RequestAutoCancelTimespan;
            var value = 1;
            _service.GetSettingValueAsyncMock = async key =>
            {
                if (key == settingKey)
                {
                    return await Task.FromResult(value.ToString());
                }

                return null;
            };

            var result = await _service.GetIntAsync(settingKey);

            result
                .Should()
                .Be(value);
        }

        [Test]
        public async Task GetDoubleAsync_NoExceptionsWasThrown_ReturnsValueConvertedToDouble()
        {
            var settingKey = SettingKey.RequestAutoCancelTimespan;
            var value = 1.5;
            _service.GetSettingValueAsyncMock = async key =>
            {
                if (key == settingKey)
                {
                    return await Task.FromResult(value.ToString(new NumberFormatInfo()));
                }

                return null;
            };

            var result = await _service.GetDoubleAsync(settingKey);

            result
                .Should()
                .Be(value);
        }

        [Test]
        public async Task GetStringAsync_NoExceptionsWasThrown_ReturnsValueAsString()
        {
            var settingKey = SettingKey.RequestAutoCancelTimespan;
            var value = "value";
            _service.GetSettingValueAsyncMock = async key =>
            {
                if (key == settingKey)
                {
                    return await Task.FromResult(value);
                }

                return null;
            };

            var result = await _service.GetStringAsync(settingKey);

            result
                .Should()
                .Be(value);
        }

        [Test]
        public async Task GetTimeSpanAsync_NoExceptionsWasThrown_ReturnsValueConvertedToTimeSpan()
        {
            var settingKey = SettingKey.RequestAutoCancelTimespan;
            var value = TimeSpan.FromDays(1);
            _service.GetSettingValueAsyncMock = async key =>
            {
                if (key == settingKey)
                {
                    return await Task.FromResult(value.ToString());
                }

                return null;
            };

            var result = await _service.GetTimeSpanAsync(settingKey);

            result
                .Should()
                .Be(value);
        }

        [Test]
        public async Task GetTimeSpanAsync_TryParseReturnedFalse_ReturnsDefaultValue()
        {
            var settingKey = SettingKey.RequestAutoCancelTimespan;
            var defaultTimeSpanAttribute = new DefaultValueAttribute("9.00:00:00");
            _service.GetSettingValueAsyncMock = async key =>
            {
                if (key == settingKey)
                {
                    return "Invalid time span";
                }

                return null;
            };
            _service.GetEnumAttributeMock = (key, type) =>
            {
                if (key == settingKey && type == defaultTimeSpanAttribute.GetType())
                {
                    return defaultTimeSpanAttribute;
                }

                return null;
            };

            var result = await _service.GetTimeSpanAsync(settingKey);

            result
                .Should()
                .Be(TimeSpan.Parse(defaultTimeSpanAttribute.Value?.ToString()));
        }

        private void MockData()
        {
            _setting = new Setting
            {
                Namespace = "Timespans",
                Key = SettingKey.RequestAutoCancelTimespan,
                Value = "value",
                Description = "description"
            };

            _settingWithDefaultNamespace = new Setting
            {
                Namespace = Setting.DefaultNamespace,
                Key = SettingKey.RequestAutoCancelTimespan,
                Value = "value1",
                Description = "description1"
            };

            _settingWithNullValue = new Setting
            {
                Namespace = "Namespace",
                Key = SettingKey.RequestAutoCancelTimespan,
                Value = null,
                Description = "desc"
            };

            _defaultValueAttribute = new DefaultValueAttribute("defaultValue");
            _descriptionAttribute = new DescriptionAttribute("description");
            _namespaceAttribute = new NamespaceAttribute("namespace");

            _settingDto = new SettingDto
            {
                Key = _setting.Key,
                Value = "new value"
            };

            _describedSettingDto = new DescribedSettingDto
            {
                Key = _setting.Key,
                Value = _setting.Value,
                Description = _setting.Description,
                DefaultValue = _defaultValueAttribute.Value?.ToString()
            };
        }

        private class SettingsServiceProxy : SettingsService
        {
            private readonly SettingsServiceTests _settingsServiceTests;

            public SettingsServiceProxy(
                IRepository<Setting> settingsRepository,
                IMapper mapper,
                SettingsServiceTests settingsServiceTests) : base(settingsRepository, mapper)
            {
                _settingsServiceTests = settingsServiceTests;
            }

            public delegate Task<Setting> GetSettingEntityAsyncDelegate(SettingKey key);

            public delegate Task<string> GetSettingValueAsyncDelegate(SettingKey key);

            public delegate Attribute GetEnumAttributeDelegate(SettingKey key, Type attributeType);

            public GetSettingEntityAsyncDelegate GetSettingEntityAsyncMock { get; set; }

            public GetSettingValueAsyncDelegate GetSettingValueAsyncMock { get; set; }

            public GetEnumAttributeDelegate GetEnumAttributeMock { get; set; }

            public async Task<Setting> GetSettingEntityAsyncProxy(SettingKey key) => await base.GetSettingEntityAsync(key);

            public async Task<string> GetSettingValueAsyncProxy(SettingKey key) => await base.GetSettingValueAsync(key);

            public T GetEnumAttributeProxy<T>(SettingKey key) where T : Attribute => base.GetEnumAttribute<T>(key);

            protected override async Task<Setting> GetSettingEntityAsync(SettingKey key)
            {
                return await(GetSettingEntityAsyncMock?.Invoke(key) ?? Task.FromResult<Setting>(null));
            }

            protected override async Task<string> GetSettingValueAsync(SettingKey key)
            {
                return await(GetSettingValueAsyncMock?.Invoke(key) ?? Task.FromResult<string>(null));
            }

            protected override T GetEnumAttribute<T>(SettingKey key)
            {
                return GetEnumAttributeMock?.Invoke(key, typeof(T)) as T;
            }
        }
    }
}
