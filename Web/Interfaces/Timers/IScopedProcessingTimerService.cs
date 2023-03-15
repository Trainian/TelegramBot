namespace Web.Interfaces.Timers
{
    public interface IScopedProcessingTimerService
    {
        Task Notifications(CancellationToken stoppingToken);
        Task Reconnect(CancellationToken stoppingToken);
    }
}
