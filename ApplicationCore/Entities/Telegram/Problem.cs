using ApplicationCore.Entities.Base;
using ApplicationCore.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Entities.Telegram
{
    public class Problem : BaseEntity
    {
        public Problem(){}
        public Problem(string text, int userId, Prioritys priority, string? img = null)
        {
            Text = text;
            UserCreateProblemId = userId;
            Priority = priority;
            Img = img;
            CreateDateTime = DateTime.Now;
        }

        [Required]
        public string Text { get; set; } = "Some problem";
        public string? Img { get; set; }

        [Required]
        public Prioritys Priority { get; set; } = Prioritys.Низкий;
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