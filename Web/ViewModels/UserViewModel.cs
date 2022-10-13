using ApplicationCore.Entities.Telegram;
using System.ComponentModel.DataAnnotations;
using Web.ViewModels.Base;

namespace Web.ViewModels
{
    public class UserViewModel : BaseViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string TelegramName { get; set; }

        [Required]
        public int TelegramId { get; set; }

        [Required]
        public PositionEnum Position { get; set; }
        public ICollection<ProblemViewModel> Problems { get; set; } = new List<ProblemViewModel>();
        public ICollection<ProblemViewModel> GetProblems { get; set; } = new List<ProblemViewModel>();
        public ICollection<AnswerViewModel> Answers { get; set; } = new List<AnswerViewModel>();
    }
}
