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
    public class ProblemRepository : Repository<Problem>, IProblemRepository
    {
        public ProblemRepository(TelegramContext context, ILogger<Problem> logger) : base(context, logger)
        {
        }

        public override async Task<IReadOnlyList<Problem>> GetAllAsync()
        {
            return await _context.Set<Problem>()
                .Include(p => p.UserCreateProblem)
                .Include(p => p.UserGetProblem)
                .Include(p => p.Answers)
                .ToListAsync();
        }
        public override async Task<IReadOnlyList<Problem>> GetAsync(Expression<Func<Problem, bool>> predicate)
        {
            return await _context.Set<Problem>()
                .Include(p => p.UserCreateProblem)
                .Include(p => p.UserGetProblem)
                .Include(p => p.Answers)
                .Where(predicate)
                .ToListAsync();
        }
        public override async Task<Problem?> GetByIdAsync(int id)
        {
            return await _context.Set<Problem>()
                .Include(p => p.UserCreateProblem)
                .Include(p => p.UserGetProblem)
                .Include(p => p.Answers)
                .FirstAsync(p => p.Id == id);
        }

        public async Task<int> GetCountByUserTelegramIdAsync(int userTelegram)
        {
            return await _context.Set<Problem>()
                .Include(p => p.UserCreateProblem)
                .Include(p => p.UserGetProblem)
                .Include(p => p.Answers)
                .Where(p => p.UserCreateProblem!.TelegramId == userTelegram)
                .CountAsync();
        }

        public async Task<Problem?> ProblemCreatedInWorkAsync(int userTelegram)
        {
            return await _context.Set<Problem>()
                .Where(p => p.UserCreateProblemId == userTelegram && p.UserGetProblemId == null)
                .FirstOrDefaultAsync();
        }
    }
}
