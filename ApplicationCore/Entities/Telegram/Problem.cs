using ApplicationCore.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Entities.Telegram
{
    public class Problem : BaseEntity
    {
        public Problem(){}
        public Problem(string text, int userId, string? img = null, int userGetId = 0)
        {
            Text = text;
            Img = img;
            UserCreateProblemId = userId;
            UserGetProblemId = userGetId != 0 ? userGetId : null;
            CreateDateTime = DateTime.Now;
        }

        [Required]
        public string Text { get; set; } = "Some problem";
        public string? Img { get; set; }

        [Required]
        public PriorityEnum Priority { get; set; } = PriorityEnum.Низкий;
        public ICollection<Answer> Answers { get; set; } = new List<Answer>();

        [Column("datetime2")]
        public DateTime CreateDateTime { get; set; }
        public bool IsComplited { get; set; } = false;

        [Required]
        public int UserCreateProblemId { get; set; }

        public int? UserGetProblemId { get; set; }


        public TelegramUser? UserCreateProblem { get; set; }
        public TelegramUser? UserGetProblem { get; set; }
    }
}