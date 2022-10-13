using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.Interfaces.Telegram;
using Web.ViewModels;

namespace Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IProblemService _problemService;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(IProblemService problemService, ILogger<IndexModel> logger)
        {
            _problemService = problemService;
            _logger = logger;
        }

        public IEnumerable<ProblemViewModel> Problems { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Problems = await _problemService.GetAllAsync();
            return Page();
        }
    }
}