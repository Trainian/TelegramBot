using ApplicationCore.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.BotAPI.AvailableTypes;

namespace Infrastructure.Static
{
    public static class TelegramBotCommands
    {
        private static BotCommand Help => new BotCommand("help", "Как работать с ботом ?");
        private static BotCommand Tasks => new BotCommand("tasks", "Просмотреть задачу");
        private static BotCommand ProblemEdit => new BotCommand("problemedit", "Редактировать задачу");
        private static BotCommand DeliveredProblems => new BotCommand("deliveredproblems", "Поставленные мной задачи");
        private static BotCommand PerformedProblems => new BotCommand("performedproblems", "Поставленные мне задачи");
        private static BotCommand Bot1CProblems => new BotCommand("1cbotproblems", "Поставленные ботом 1С задачи");
        private static BotCommand NotResponsible => new BotCommand("notresponsible", "Не принятые задачи");
        private static BotCommand Settings => new BotCommand("settings", "Настройки");
        private static BotCommand Register => new BotCommand("register", "Зарегестрироваться на сервере");

        public static BotCommand[] GetCommands(Positions position)
        {
            var commands = new List<BotCommand>();
            switch (position)
            {
                case Positions.Пользователь:
                    commands.Add(Help);
                    commands.Add(DeliveredProblems);
                    commands.Add(Settings);
                    commands.Add(Register);
                    break;
                case Positions.ТехСпециалист:
                    commands.Add(Help);
                    commands.Add(DeliveredProblems);
                    commands.Add(PerformedProblems);
                    commands.Add(Bot1CProblems);
                    commands.Add(NotResponsible);
                    commands.Add(ProblemEdit);
                    commands.Add(Settings);
                    break;
                case Positions.Администратор:
                    commands.Add(Help);
                    commands.Add(DeliveredProblems);
                    commands.Add(PerformedProblems);
                    commands.Add(Bot1CProblems);
                    commands.Add(NotResponsible);
                    commands.Add(ProblemEdit);
                    commands.Add(Settings);
                    break;
                case Positions.СуперАдмин:
                    commands.Add(Help);
                    commands.Add(DeliveredProblems);
                    commands.Add(PerformedProblems);
                    commands.Add(Bot1CProblems);
                    commands.Add(NotResponsible);
                    commands.Add(ProblemEdit);
                    commands.Add(Settings);
                    break;
            }
            return commands.ToArray();
        }
    }
}
