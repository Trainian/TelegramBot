using Microsoft.AspNetCore.Mvc;
using Telegram.BotAPI.GettingUpdates;
using Infrastructure.Services.Telegram;

namespace Web.Controllers
{
    [Route("api/telegram")]
    [ApiController]
    public class TelegramController
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TelegramController> _logger;
        private readonly TelegramBotService _bot;

        public TelegramController(ILogger<TelegramController> logger, IConfiguration configuration, TelegramBotService bot) : base()
        {
            _logger = logger;
            _configuration = configuration;
            _bot = bot;
        }

        [HttpGet]
        public IActionResult Get([FromHeader(Name = "X-Telegram-Bot-Api-Secret-Token")] string webhookToken)
        {
            if (_configuration["Telegram:WebhookToken"] != webhookToken)
            {
                _logger.LogWarning("Failed access!");
                return new UnauthorizedResult();
            }
            return new OkResult();
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromHeader(Name = "X-Telegram-Bot-Api-Secret-Token")] string webhookToken, [FromBody] Update update, CancellationToken cancellationToken)
        {
            if (_configuration["Telegram:WebhookToken"] != webhookToken)
            {
#if DEBUG
                _logger.LogWarning("Failed access");
#endif
                return new UnauthorizedResult();
            }
            if (update == default)
            {
#if DEBUG
                _logger.LogWarning("Invalid update detected");
#endif
                return new BadRequestResult();
            }
            await _bot.OnUpdateAsync(update, cancellationToken);

            return await Task.FromResult(new OkResult());
        }
    }
}
