using Infrastructure.Data.Telegram;
using Infrastructure.Services.Telegram;

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

                await _timerService.DoWork(stoppingToken);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("TimerService остановлен");

            await base.StopAsync(cancellationToken);
        }
    }
}
