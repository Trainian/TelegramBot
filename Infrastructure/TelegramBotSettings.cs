using Microsoft.Extensions.Configuration;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace Infrastructure.Settings
{
    public class TelegramBotSettings : IBotProperties
    {
        private readonly BotCommandHelper _commandHelper;
        private static bool _firstStart;
        public TelegramBotSettings(IConfiguration configuration)
        {
            var Token = configuration["Telegram:BotToken"];
            var Url = String.Concat(configuration["ApplicationUrl"], "/api/telegram");
            var WebHookToken = configuration["Telegram:WebhookToken"];

            Api = new BotClient(Token);
            User = Api.GetMe();

            _commandHelper = new BotCommandHelper(this);

            if(!_firstStart)
            {
                Api.DeleteMyCommands();
                Api.SetMyCommands(
                    new BotCommand("help", "Как работать с ботом ?"),
                    new BotCommand("tasks", "Просмотреть задачу"),
                    new BotCommand("problemedit", "Редактировать задачу(проблему)"),
                    new BotCommand("deliveredproblems", "Показать ВСЕ поставленные МНОЙ задачи(проблемы)"),
                    new BotCommand("performedproblems", "Показать ВСЕ посталвенные МНЕ задачи(проблемы)"),
                    new BotCommand("register", "Зарегестрироваться на сервере"));

                Api.DeleteWebhook();
                Api.SetWebhook(Url, secretToken: WebHookToken);
                _firstStart = true;
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
