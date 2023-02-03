using Infrastructure.Settings;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Threading;

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
                    var messages = new InlineKeyboardMarkup(new InlineKeyboardButton[][]
                    {
                        new InlineKeyboardButton[]
                        {
                            InlineKeyboardButton.SetCallbackData("Создать новую задачу", "NewTask")
                        },
                        new InlineKeyboardButton[]
                        {
                            InlineKeyboardButton.SetCallbackData("Добавить комментарий к задаче", "AddCommentChoiseProblem")
                        },
                        new InlineKeyboardButton[]
                        {
                            InlineKeyboardButton.SetCallbackData("Ничего", "Nothing")
                        }
                    });
                    await Api.SendMessageAsync(message.Chat.Id,
                        text: "Что мне сделать с вашим сообщением?",
                        replyMarkup: messages,
                        replyToMessageId: message.MessageId,
                        cancellationToken: cancellationToken);
                }
            }

            await base.OnMessageAsync(message, cancellationToken);
        }

        static async Task PostAsJsonAsync(HttpClient httpClient, Message message, BotClient api)
        {
            using HttpResponseMessage response = await httpClient.PostAsJsonAsync(
                "http://topsite1.ru",
                new { test = message.Text});

            var result = response.EnsureSuccessStatusCode();

            if(result.IsSuccessStatusCode)
            {
                await api.SendMessageAsync(message.Chat.Id,
                    text: "Успешная отправка на ТОПСАЙТ",
                    replyToMessageId: message.MessageId);
                var answer = await response.Content.ReadAsStringAsync();
                await api.SendMessageAsync(message.Chat.Id,
                    text: $"Ответ: {answer}",
                    replyToMessageId: message.MessageId);
            }
            else
            {
                await api.SendMessageAsync(message.Chat.Id,
                    text: "Ошибка отправки запроса на ТОПСАЙТ",
                    replyToMessageId: message.MessageId);
            }

            //var todo = await response.Content.ReadFromJsonAsync<Todo>();
            //WriteLine($"{todo}\n");
            // Expected output:
            //   POST https://jsonplaceholder.typicode.com/todos HTTP/1.1
            //   Todo { UserId = 9, Id = 201, Title = Show extensions, Completed = False }
        }

    }
}
