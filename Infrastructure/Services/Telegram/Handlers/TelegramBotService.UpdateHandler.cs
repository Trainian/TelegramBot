using Infrastructure.Settings;
using Telegram.BotAPI;
using Telegram.BotAPI.GettingUpdates;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Telegram
{
    public partial class TelegramBotService : AsyncTelegramBotBase<TelegramBotSettings>
    {
        public override async Task OnUpdateAsync(Update update, CancellationToken cancellationToken)
        {
#if DEBUG
            _logger.LogInformation("New update with id: {0}. Type: {1}", update?.UpdateId, update?.Type.ToString("F"));
#endif

            await base.OnUpdateAsync(update, cancellationToken);
        }
    }
}
