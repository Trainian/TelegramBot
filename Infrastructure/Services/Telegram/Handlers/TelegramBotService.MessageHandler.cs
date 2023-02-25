using Infrastructure.Settings;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Threading;
using Telegram.BotAPI.AvailableMethods.FormattingOptions;

namespace Infrastructure.Services.Telegram
{
    public partial class TelegramBotService : AsyncTelegramBotBase<TelegramBotSettings>
    {
        protected override async Task OnMessageAsync(Message message, CancellationToken cancellationToken)
        {
            // Ignore user 777000 (Telegram)
            if (message!.From?.Id == TelegramConstants.TelegramId)
            {
                return;
            }

            var hasText = !string.IsNullOrEmpty(message.Text); // True if message has text;

#if DEBUG
            _logger.LogInformation("New message from chat id: {ChatId}", message!.Chat.Id);
            _logger.LogInformation("Message: {MessageContent}", hasText ? message.Text : "No text");
#endif
            if(message?.Text?.StartsWith('/') == true) // Если это запрос действия бота
            {

            }
            else if(message?.Text?.ToLower().StartsWith("1с") == true)
            {
                using (var httpClient = new HttpClient())
                {
                    await PostAsJsonAsync(httpClient,message,Api);
                }
            }
            else// Если это не запрос действия бота
            {
                var isRegistered = await UserIsRegistered(message!.From!.Id);
                if (!isRegistered)
                    await AnswerIsUserNotRegistered(message, cancellationToken);

                else
                {
                    var messages = GetInlineKeyboardToChooseMetdotsWithProblem();
                    await Api.SendMessageAsync(message.Chat.Id,
                        text: "Что мне сделать с вашим сообщением?",
                        replyMarkup: messages,
                        replyToMessageId: message.MessageId,
                        cancellationToken: cancellationToken);
                }
            }

            await base.OnMessageAsync(message, cancellationToken);
        }

        

    }
}
