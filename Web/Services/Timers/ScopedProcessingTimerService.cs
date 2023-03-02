using ApplicationCore.Repositories.Telegram;
using Infrastructure.Data.Telegram;
using Infrastructure.Repositories.Telegram;
using Infrastructure.Services.Telegram;
using Microsoft.AspNetCore.Components;

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
        public async Task DoWork(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _telegramService.CheckNotifications(null);
                _logger.LogInformation("ScopedProcessingTimerService");
                await Task.Delay(20000, stoppingToken);
            }
        }
    }
}
