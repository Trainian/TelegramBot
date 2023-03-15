using Infrastructure.Settings;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Microsoft.Extensions.Logging;
using Telegram.BotAPI.AvailableMethods.FormattingOptions;
using ApplicationCore.Entities.Telegram;
using ApplicationCore.Enums;
using Infrastructure.Extensions;
using Infrastructure.Static;

namespace Infrastructure.Services.Telegram
{
    public partial class TelegramBotService : AsyncTelegramBotBase<TelegramBotSettings>
    {
        protected override async Task OnCommandAsync(Message message, string commandName, string commandParameters, CancellationToken cancellationToken)
        {
            var args = commandParameters.Split(' ');
            var isRegistered = await UserIsRegistered(message.From!.Id);
            var markup = new InlineKeyboardMarkup();
            TelegramUser? user;

            string answer = "";
#if DEBUG
            _logger.LogInformation("Params: {0}", args.Length);
#endif

            switch (commandName)
            {
                case "help":
                    answer = string.Format("📝<b>---Как добавить новую задачу---</b>📝\n\n" +
                        "1️⃣ Для добавления новой задачи, достаточно отправить сообщение или изображение с подписью к нему.\n\n" +
                        "2️⃣ После чего, бот у вас уточнит что сделатṁ, ответив на это сообщение вы добавите новую задачу.");
                    await Api.SendMessageAsync(message.Chat.Id, answer, parseMode: ParseMode.HTML, cancellationToken: cancellationToken);
                    break;

                case "start":
                    user = await _service.GetUserTelegramByTelegramId(message.From.Id);
                    Positions? position = user?.Position ?? null;
                    answer = "Меню";
                    markup = GetIKStart(position);
                    await Api.SendMessageAsync(message.Chat.Id, answer, replyMarkup: markup);
                    break;

                case "register":
                    answer = await _service.CreateTelegramUser(message.From!);
                    await Api.SendMessageAsync(message.Chat.Id, answer, parseMode: ParseMode.HTML, cancellationToken: cancellationToken);
                    break;

                case "test":
                    
                    break;

                default:
                    if (message.Chat.Type == ChatType.Private)
                    {
                        await Api.SendMessageAsync(message.Chat.Id, "Нераспознанная команда.", cancellationToken: cancellationToken);
                    }
                    break;
            }
        }
    }
}
