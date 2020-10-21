using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Dto.Email;
using Application.Services.Interfaces;
using Domain.RDBMS;
using Domain.RDBMS.Entities;
using Domain.RDBMS.Enums;
using Hangfire;

namespace Application.Services.Implementation
{
    public class HangfireJobSchedulerService : IHangfireJobScheduleService
    {
        private const SettingKey RequestAutoCancelSettingKey = SettingKey.RequestAutoCancelTimespan;
        private const SettingKey RequestAutoCancelRemindKey = SettingKey.RequestAutoCancelRemindTimespan;

        private readonly IRepository<ScheduleJob> _scheduleRepository;
        private readonly ISettingsService _settingsService;

        public HangfireJobSchedulerService(IRepository<ScheduleJob> scheduleRepository, ISettingsService settingsService)
        {
            _scheduleRepository = scheduleRepository;
            _settingsService = settingsService;
        }
        public async Task ScheduleRequestJob(RequestMessage message)
        {
            var reminderTimespan = await _settingsService.GetTimeSpanAsync(RequestAutoCancelRemindKey);
            var autoCancelTimespan = await _settingsService.GetTimeSpanAsync(RequestAutoCancelSettingKey);

            var secondJobId = BackgroundJob.Schedule<RequestService>(x => x.RemoveAsync(message.RequestId),
                autoCancelTimespan);
            _scheduleRepository.Add(new ScheduleJob { ScheduleId = secondJobId, RequestId = message.RequestId });
            if (reminderTimespan >= autoCancelTimespan)
            {
                await _scheduleRepository.SaveChangesAsync();
                return;
            }

            if (message.User.IsEmailAllowed)
            {
                var emailJobId = BackgroundJob.Schedule<EmailSenderService>(x => x.SendReceiveConfirmationAsync(
                        message.UserName, message.BookName,
                        message.BookId, message.RequestId, message.UserAddress.ToString()),
                    reminderTimespan);
                _scheduleRepository.Add(new ScheduleJob { ScheduleId = emailJobId, RequestId = message.RequestId });
            }

            var notificationJobId = BackgroundJob.Schedule<NotificationsService>(
                x => x.NotifyAsync(
                    message.User.Id,
                    $"You have requested '{message.BookName}'. Please click 'Start reading' if the book is " +
                    $"received otherwise the book request will be automatically canceled in " +
                    $"{autoCancelTimespan - reminderTimespan}",
                    $"Ви подали запит на книгу '{message.BookName}'. Будь ласка, натисніть 'Почати читати', якщо отримали книгу," +
                    $" в іншому випадку запит на книгу буде автоматично скасовано через " +
                    $"{autoCancelTimespan - reminderTimespan}",
                    message.BookId,
                    NotificationAction.StartReading),
                reminderTimespan);
            _scheduleRepository.Add(new ScheduleJob { ScheduleId = notificationJobId, RequestId = message.RequestId });
            await _scheduleRepository.SaveChangesAsync();
            
        }
        public async Task DeleteRequestScheduleJob(int requestId)
        {
            var records = _scheduleRepository.GetAll().Where(x => x.RequestId == requestId).ToList();
            foreach (var jobId in records)
            {
                BackgroundJob.Delete(jobId.ScheduleId);
            }
            _scheduleRepository.RemoveRange(records);
            await _scheduleRepository.SaveChangesAsync();
        }
    }
}
