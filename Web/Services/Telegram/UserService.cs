using ApplicationCore.Repositories.Telegram;
using AutoMapper;
using System.Linq.Expressions;
using Web.Interfaces.Telegram;
using Web.ViewModels;

namespace Web.Services.Telegram
{
    public class UserService : IUserService
    {
        private IUserRepository _userRepository;
        private IMapper _mapper;
        private ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, IMapper mapper, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }
        public Task<UserViewModel> AddAsync(UserViewModel entity)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync()
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(UserViewModel entity)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<UserViewModel>> GetAllAsync()
        {
            var telegramUsersDb = await _userRepository.GetAllAsync();
            var usersViewModel = _mapper.Map<IEnumerable<UserViewModel>>(telegramUsersDb);
            if (usersViewModel == null)
                usersViewModel = new List<UserViewModel>();
            return usersViewModel;
        }

        public Task<IEnumerable<UserViewModel>> GetAsync(Expression<Func<UserViewModel, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<UserViewModel?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(UserViewModel entity)
        {
            throw new NotImplementedException();
        }
    }
}
