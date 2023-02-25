using Infrastructure.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;

namespace Infrastructure.Services.Telegram
{
    public partial class TelegramBotService : AsyncTelegramBotBase<TelegramBotSettings>
    {
        /// <summary>
        /// Тестовый метод отправки запроса к API 1C
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="message"></param>
        /// <param name="api"></param>
        /// <returns></returns>
        protected static async Task PostAsJsonAsync(HttpClient httpClient, Message message, BotClient api)
        {
            using HttpResponseMessage response = await httpClient.PostAsJsonAsync(
                "http://topsite1.ru",
                new { test = message.Text });

            var result = response.EnsureSuccessStatusCode();

            if (result.IsSuccessStatusCode)
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
        }
    }
}
