using System.Linq.Expressions;
using Web.Interfaces.Telegram;
using Web.ViewModels;

namespace Web.Services.Telegram
{
    public class AnswerService : IAnswerService
    {
        public Task<AnswerViewModel> AddAsync(AnswerViewModel entity)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync()
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(AnswerViewModel entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AnswerViewModel>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AnswerViewModel>> GetAsync(Expression<Func<AnswerViewModel, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<AnswerViewModel?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(AnswerViewModel entity)
        {
            throw new NotImplementedException();
        }
    }
}
