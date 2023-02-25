using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Web.ViewModels.Base;

namespace Web.ViewModels
{
    public class AnswerViewModel : BaseViewModel
    {
        [Required]
        public string Text { get; set; } = "Answer";

        public DateTime CreateDateTime { get; set; }

        [Required]
        public int ProblemId { get; set; }
        [Required]
        public int UserCreateId { get; set; }

        [JsonIgnore]
        public ProblemViewModel? Problem { get; set; }
        
        public UserViewModel? UserCreate { get; set; }
    }
}
