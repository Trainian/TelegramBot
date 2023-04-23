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
        private IConfiguration _configuration;
        private ILogger<ScopedProcessingTimerService> _logger;

        public ScopedProcessingTimerService(TelegramBotService telegramService, IConfiguration configuration, ILogger<ScopedProcessingTimerService> logger)
        {
            _telegramService = telegramService;
            _configuration = configuration;
            _logger = logger;
        }
        public async Task Notifications(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Метод уведомления");
                _telegramService.CheckNotifications(null);
                await Task.Delay(60000, stoppingToken);
            }
        }
        public async Task ReconnectApi(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Метод запроса к API");
                _telegramService.ReconnectApi(null);
                await Task.Delay(300000, stoppingToken);
            }
        }

        public async Task ReconnectWeb(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Метод запроса к сайту");
                var Url = new UriBuilder((String.Concat(_configuration["ApplicationUrl"], "/Index"))).Uri;
                var client = new HttpClient() { BaseAddress = Url };
                var response = await client.GetAsync(Url);
                _logger.LogInformation("Статус запроса к сайту: " + response.StatusCode.ToString());
                await Task.Delay(600000, stoppingToken);
            }
        }
    }
}
