namespace Web.Services.Timers
{
    public interface IScopedProcessingTimerService
    {
        Task DoWork(CancellationToken stoppingToken);
    }
}
