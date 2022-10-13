using ApplicationCore.Entities.Telegram;
using ApplicationCore.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.BotAPI.AvailableTypes;

namespace ApplicationCore.Repositories.Telegram
{
    public interface IUserRepository : IRepository<TelegramUser>
    {
        Task<TelegramUser> CreateTelegramUserAsync(User user);
        Task<TelegramUser?> GetByTelegramIdAsync(long telegramId);
    }
}
