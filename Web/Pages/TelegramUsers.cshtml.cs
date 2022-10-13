using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.Interfaces.Telegram;
using Web.ViewModels;

namespace Web.Pages
{
    public class TelegramUsersModel : PageModel
    {
        private IUserService _userService;
        private ILogger<TelegramUsersModel> _logger;

        public TelegramUsersModel(IUserService userService, ILogger<TelegramUsersModel> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        public IEnumerable<UserViewModel> Users { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Users = await _userService.GetAllAsync();
            return Page();
        }
    }
}
