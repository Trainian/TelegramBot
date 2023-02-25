using ApplicationCore.Services.Api;
using Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace Infrastructure.Services.Telegram
{
    public partial class TelegramBotService : AsyncTelegramBotBase<TelegramBotSettings>
    {
        private ITelegramApiService _service;
        private ILogger<TelegramBotService> _logger;
        private IConfiguration _configuration;


        public TelegramBotService(
            TelegramBotSettings botProperties,
            ITelegramApiService service,
            IConfiguration configuration,
            ILogger<TelegramBotService> logger) : base(botProperties)
        {
            _service = service;
            _configuration = configuration;
            _logger = logger;
        }
    }
}
