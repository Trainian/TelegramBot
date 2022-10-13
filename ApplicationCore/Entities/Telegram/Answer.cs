using ApplicationCore.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Entities.Telegram
{
    public class Answer : BaseEntity
    {
        public Answer(){}
        public Answer(int userCreateId, int problemId, string text, string? img = null)
        {
            UserCreateId = userCreateId;
            ProblemId = problemId;
            Text = text;
            Img = img;
            CreateDateTime = DateTime.Now;
        }
        [Required]
        public string Text { get; set; } = "Answer";

        [Column("datetime2")]
        public DateTime CreateDateTime { get; set; }

        public string? Img { get; set; }

        [Required]
        public int ProblemId { get; set; }
        [Required]
        public int UserCreateId { get; set; }

        public Problem? Problem { get; set; }
        public TelegramUser? UserCreate { get; set; }

    }
}
