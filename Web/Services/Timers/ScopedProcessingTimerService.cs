using ApplicationCore.Repositories.Telegram;
using Infrastructure.Data.Telegram;
using Infrastructure.Repositories.Telegram;
using Infrastructure.Services.Telegram;
using Microsoft.AspNetCore.Components;
using Web.Interfaces.Timers;

namespace Web.Services.Timers
{
    public class ScopedProcessingTimerService : IScopedProcessingTimerService
    {
        private TelegramBotService _telegramService;
        private ILogger<ScopedProcessingTimerService> _logger;

        public ScopedProcessingTimerService(TelegramBotService telegramService, ILogger<ScopedProcessingTimerService> logger)
        {
            _telegramService = telegramService;
            _logger = logger;
        }
        public async Task Notifications(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _telegramService.CheckNotifications(null);
                _logger.LogInformation("Notifications method");
                await Task.Delay(60000, stoppingToken);
            }
        }
        public async Task Reconnect(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _telegramService.ReconnectApi(null);
                _logger.LogInformation("Reconnect method");
                await Task.Delay(300000, stoppingToken);
            }
        }
    }
}
