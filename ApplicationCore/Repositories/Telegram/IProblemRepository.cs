using ApplicationCore.Entities.Telegram;
using ApplicationCore.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Repositories.Telegram
{
    public interface IProblemRepository : IRepository<Problem>
    {
        Task<int> GetCountByUserTelegramIdAsync(int userTelegram);
        Task<Problem?> ProblemCreatedInWorkAsync(int userTelegram);
    }
}
