namespace Web.Interfaces.Timers
{
    public interface IScopedProcessingTimerService
    {
        Task Notifications(CancellationToken stoppingToken);
        Task ReconnectApi(CancellationToken stoppingToken);
        Task ReconnectWeb(CancellationToken stoppingToken);
    }
}
