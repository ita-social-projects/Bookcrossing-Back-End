using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Application.Dto.Settings;
using Application.Services.Interfaces;
using BookCrossingBackEnd.Controllers;
using Domain.RDBMS.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace ApplicationTest.Controllers
{
    [TestFixture]
    class SettingsControllerTests
    {
        private Mock<ISettingsService> _settingsServiceMock;
        private SettingsController _controller;

        [OneTimeSetUp]
        public void InitializeClass()
        {
            _settingsServiceMock = new Mock<ISettingsService>();
            _controller = new SettingsController(_settingsServiceMock.Object);
        }

        [Test]
        public async Task GetSetting_ServiceReturnedNull_ReturnsNotFoundResult()
        {
            var settingKey = SettingKey.RequestAutoCancelTimespan;
            _settingsServiceMock.Setup(obj => obj.GetSettingAsync(settingKey))
                .ReturnsAsync(value: null);

            var result = await _controller.GetSetting(settingKey);

            result.Result
                .Should()
                .BeOfType<NotFoundResult>();
        }

        [Test]
        public async Task GetSetting_ServiceReturnedDescribedSettingDto_ReturnsObjectInResult()
        {
            var settingKey = SettingKey.RequestAutoCancelTimespan;
            var settingDto = new DescribedSettingDto();
            _settingsServiceMock.Setup(obj => obj.GetSettingAsync(settingKey))
                .ReturnsAsync(settingDto);

            var result = await _controller.GetSetting(settingKey);

            result.Value
                .Should()
                .Be(settingDto);
        }

        [Test]
        public async Task PutSetting_NoExceptionsWasThrown_ReturnNoContentResult()
        {
            var settingKey = SettingKey.RequestAutoCancelTimespan;
            var settingDto = new SettingDto { Key = settingKey };

            var result = await _controller.PutSetting(settingKey, settingDto);

            result
                .Should()
                .BeOfType<NoContentResult>();
        }
    }
}
