using Infrastructure.Services.Telegram;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

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
            var firstStart = configuration["Telegram:FirstStart"];

            Api = new BotClient(Token);
            User = Api.GetMe();

            _commandHelper = new BotCommandHelper(this);

            if(firstStart == "true")
            {
                Api.DeleteMyCommands();
                Api.SetMyCommands(
                    new BotCommand("help", "Как работать с ботом ?"),
                    new BotCommand("tasks", "Просмотреть задачу"),
                    new BotCommand("problemedit", "Редактировать задачу"),
                    new BotCommand("deliveredproblems", "Поставленные мной задачи"),
                    new BotCommand("performedproblems", "Поставленные мне задачи"),
                    new BotCommand("1cbotproblems", "Поставленные ботом 1С задачи"),
                    new BotCommand("notresponsible", "Не принятые задачи"),
                    new BotCommand("settings", "Настройки"),
                    new BotCommand("register", "Зарегестрироваться на сервере"));

                Api.DeleteWebhook();
                Api.SetWebhook(Url, secretToken: WebHookToken);
                configuration["Telegram:FirstStart"] = "false";
            }
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
