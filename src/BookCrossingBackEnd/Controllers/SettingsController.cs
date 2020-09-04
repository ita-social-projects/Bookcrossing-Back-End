using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Dto.Settings;
using Application.Services.Interfaces;
using Domain.RDBMS.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookCrossingBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly ISettingsService _settingsService;

        public SettingsController(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        [HttpGet("{key}")]
        public async Task<ActionResult<DescribedSettingDto>> GetSetting(SettingKey key)
        {
            var setting = await _settingsService.GetSettingAsync(key);
            if (setting == null)
            {
                return NotFound();
            }

            return setting;
        }

        [HttpPut("{key}")]
        public async Task<ActionResult> PutSetting(SettingKey key, [FromBody] SettingDto settingDto)
        {
            if (key != settingDto.Key)
            {
                return BadRequest();
            }

            await _settingsService.SetSettingValueAsync(key, settingDto);
            return NoContent();
        }
    }
}
