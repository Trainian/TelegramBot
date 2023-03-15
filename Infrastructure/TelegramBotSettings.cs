using Infrastructure.Services.Telegram;
using Infrastructure.Static;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;
using ApplicationCore.Enums;

namespace Infrastructure.Settings
{
    public class TelegramBotSettings : IBotProperties
    {
        private readonly BotCommandHelper _commandHelper;

        public TelegramBotSettings(IConfiguration configuration, ILogger<TelegramBotSettings> logger)
        {
            var Token = configuration["Telegram:BotToken"];
            var Url = String.Concat(configuration["ApplicationUrl"], "/api/telegram");
            var WebHookToken = configuration["Telegram:WebhookToken"];

            Api = new BotClient(Token);
            User = Api.GetMe();

            _commandHelper = new BotCommandHelper(this);

            Api.DeleteMyCommands();
            Api.SetMyCommands(new BotCommand[]
            {
                new BotCommand("start", "Открыть меню"),
                new BotCommand("help", "Как работать с ботом ?"),
                new BotCommand("register", "Регистрация")
            });

            Api.DeleteWebhook();
            Api.SetWebhook(Url, secretToken: WebHookToken);
            configuration["Telegram:FirstStart"] = "false";

            //To Set WebHook
            //https://api.telegram.org/bot{token}/setWebhook?url=https://{site}/api/telegram&secret_token={secret token}
            //To WebHook Info
            //https://api.telegram.org/bot{token}/getWebhookInfo


        }
        public BotClient Api { get; }

        public User User { get; }

        public IBotCommandHelper CommandHelper => _commandHelper;
    }
}
