using Infrastructure.Settings;
using Telegram.BotAPI;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Telegram
{
    public partial class TelegramBotService : AsyncTelegramBotBase<TelegramBotSettings>
    {
        protected override async Task OnBotExceptionAsync(BotRequestException exp, CancellationToken cancellationToken)
        {
            _logger.LogError("BotRequestException: {Message}", exp.Message);
        }

        protected override async Task OnExceptionAsync(Exception exp, CancellationToken cancellationToken)
        {
            _logger.LogError("Exception: {Message}", exp.Message);
        }
    }
}
