using ApplicationCore.Entities.Telegram;
using ApplicationCore.Repositories.Telegram;
using Infrastructure.Data.Telegram;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Telegram.BotAPI.AvailableTypes;

namespace Infrastructure.Repositories.Telegram
{
    public class UserRepository : Repository<TelegramUser>, IUserRepository
    {
        public UserRepository(TelegramContext context, ILogger<TelegramUser> logger) : base(context, logger)
        {
        }

        public async Task<TelegramUser> CreateTelegramUserAsync(User user)
        {
            var userDb = new TelegramUser(user);
            return await this.AddAsync(userDb);
        }

        public override async Task<IReadOnlyList<TelegramUser>> GetAllAsync()
        {
            return await _context.Set<TelegramUser>()
                .Include(ut => ut.Problems)
                .ToListAsync();
        }
        public override async Task<IReadOnlyList<TelegramUser>> GetAsync(Expression<Func<TelegramUser, bool>> predicate)
        {
            return await _context.Set<TelegramUser>()
                .Include(ut => ut.Problems)
                .Where(predicate)
                .ToListAsync();
        }
        public override async Task<TelegramUser?> GetByIdAsync(int id)
        {
            return await _context.Set<TelegramUser>()
                .Include(ut => ut.Problems)
                .FirstAsync(ut => ut.Id == id);
        }

        public async Task<TelegramUser?> GetByTelegramIdAsync(long telegramId)
        {
            return await _context.Set<TelegramUser>()
                .FirstOrDefaultAsync(u => u.TelegramId == telegramId);
        }
    }
}
