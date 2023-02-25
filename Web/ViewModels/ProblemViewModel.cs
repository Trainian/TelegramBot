using ApplicationCore.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Web.ViewModels.Base;

namespace Web.ViewModels
{
    public class ProblemViewModel : BaseViewModel
    {
        [Required]
        public string Text { get; set; } = "Text Problem";
        public string? Img { get; set; }

        [Required]
        public Prioritys Priority { get; set; } = Prioritys.Низкий;
        public ICollection<AnswerViewModel> Answers { get; set; } = new List<AnswerViewModel>();
        public DateTime CreateDateTime { get; set; }
        public bool IsComplited { get; set; }

        [Required]
        public int UserCreateProblemId { get; set; }

        public int? UserGetProblemId { get; set; }


        public UserViewModel? UserCreateProblem { get; set; }

        public UserViewModel? UserGetProblem { get; set; }
    }
}
