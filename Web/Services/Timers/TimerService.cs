using Infrastructure.Data.Telegram;
using Infrastructure.Services.Telegram;
using Web.Interfaces.Timers;

namespace Web.Services.Timers
{
    public class TimerService : BackgroundService
    {
        private IServiceProvider _service;
        private ILogger<TimerService> _logger;
        private Timer? _timer;

        public TimerService(IServiceProvider services, ILogger<TimerService> logger)
        {
            _service = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("TimerService запущен");

            await DoWork(stoppingToken);
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            _logger.LogInformation("TimerService ВЫПОЛНЯЕТСЯ");

            using (var scope = _service.CreateScope())
            {
                var _timerService = scope.ServiceProvider.GetRequiredService<IScopedProcessingTimerService>();
                var task1 = _timerService.Notifications(stoppingToken);
                var task2 = _timerService.ReconnectApi(stoppingToken);
                var task3 = _timerService.ReconnectWeb(stoppingToken);
                await Task.WhenAll(task1, task2);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("TimerService остановлен");

            await base.StopAsync(cancellationToken);
        }
    }
}
