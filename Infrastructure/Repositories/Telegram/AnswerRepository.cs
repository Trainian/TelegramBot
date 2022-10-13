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

namespace Infrastructure.Repositories.Telegram
{
    public class AnswerRepository : Repository<Answer>, IAnswerRepository
    {
        public AnswerRepository(TelegramContext context, ILogger<Answer> logger) : base(context, logger)
        {
        }

        public override async Task<IReadOnlyList<Answer>> GetAllAsync()
        {
            return await _context.Set<Answer>()
                .Include(a => a.UserCreate)
                .Include(a => a.Problem)
                .ToListAsync();
        }
        public override async Task<IReadOnlyList<Answer>> GetAsync(Expression<Func<Answer, bool>> predicate)
        {
            return await _context.Set<Answer>()
                .Include(a => a.UserCreate)
                .Include(a => a.Problem)
                .Where(predicate)
                .ToListAsync();
        }
        public override async Task<Answer?> GetByIdAsync(int id)
        {
            return await _context.Set<Answer>()
                .Include(a => a.UserCreate)
                .Include(a => a.Problem)
                .FirstAsync(a => a.Id == id);
        }
    }
}
