using ApplicationCore.Entities.Base;
using ApplicationCore.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.BotAPI.AvailableTypes;

namespace ApplicationCore.Entities.Telegram
{
    public class TelegramUser : BaseEntity
    {
        public TelegramUser(){}
        public TelegramUser(User user)
        {
            TelegramId = user.Id;
            Name = user.FirstName;
            TelegramName = user.Username;
        }
        public TelegramUser(long telegramId, string name, string telegramName, Positions position = Positions.Пользователь)
        {
            TelegramId = telegramId;
            Name = name;
            TelegramName = telegramName;
            Position = position;
        }
        [Required]
        public string Name { get; set; }
        [Required]
        public string TelegramName { get; set; }

        [Required]
        public Positions Position { get; set; }

        [Required]
        public long TelegramId { get; set; }

        public string? NotificationDays { get; set; }

        public string? NotificationHours { get; set; }



        public ICollection<Problem> Problems { get; set; } = new List<Problem>();
        public ICollection<Problem> GetProblems { get; set; } = new List<Problem>();
        public ICollection<Answer> Answers { get; set; } = new List<Answer>();
    }
}
