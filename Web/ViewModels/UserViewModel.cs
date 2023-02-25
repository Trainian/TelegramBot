using ApplicationCore.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
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
        public Positions Position { get; set; }

        [JsonIgnore]
        public ICollection<ProblemViewModel> Problems { get; set; } = new List<ProblemViewModel>();
        [JsonIgnore]
        public ICollection<ProblemViewModel> GetProblems { get; set; } = new List<ProblemViewModel>();
        [JsonIgnore]
        public ICollection<AnswerViewModel> Answers { get; set; } = new List<AnswerViewModel>();
    }
}
