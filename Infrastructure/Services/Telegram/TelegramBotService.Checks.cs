using Infrastructure.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods.FormattingOptions;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;

namespace Infrastructure.Services.Telegram
{
    public partial class TelegramBotService : AsyncTelegramBotBase<TelegramBotSettings>
    {
        /// <summary>
        /// Проверка что пользователь Зарегестрирован
        /// </summary>
        /// <param name="telegramId">Telegram ID</param>
        /// <returns>Существует ли пользователь</returns>
        protected async Task<bool> UserIsRegistered(long telegramId)
        {
            var telegramUser = await _service.GetUserTelegramByTelegramId(telegramId);
            return telegramUser != null ? true : false;
        }


    }
}
