using ApplicationCore.Repositories.Telegram;
using AutoMapper;
using System.Linq.Expressions;
using Web.Interfaces.Telegram;
using Web.ViewModels;

namespace Web.Services.Telegram
{
    public class ProblemService : IProblemService
    {
        private IProblemRepository _repository;
        private IMapper _mapper;
        private ILogger<ProblemService> _logger;

        public ProblemService(IProblemRepository repository, IMapper mapper, ILogger<ProblemService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }
        public Task<ProblemViewModel> AddAsync(ProblemViewModel entity)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync()
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(ProblemViewModel entity)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ProblemViewModel>> GetAllAsync()
        {
            var models = await _repository.GetAllAsync();
            var viewModel = _mapper.Map<IEnumerable<ProblemViewModel>>(models);
            return viewModel;
        }

        public Task<IEnumerable<ProblemViewModel>> GetAsync(Expression<Func<ProblemViewModel, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<ProblemViewModel?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(ProblemViewModel entity)
        {
            throw new NotImplementedException();
        }
    }
}
