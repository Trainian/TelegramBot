using ApplicationCore.Entities.Telegram;
using ApplicationCore.Enums;
using Infrastructure.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.UpdatingMessages;

namespace Infrastructure.Services.Telegram
{
    public partial class TelegramBotService : AsyncTelegramBotBase<TelegramBotSettings>
    {
        private const string HeavyCheckMark = "✔️";
        private const string RadioButton = "🔘";
        private const string WhiteCircle = "⚪️";

        /// <summary>
        /// Очистить список кнопок
        /// </summary>
        /// <param name="callbackQuery">Сообщение для очистки</param>
        /// <returns></returns>
        protected async Task ClearIK(CallbackQuery callbackQuery)
        {
            // Очистка действий на сообщение пользователя
            await Api.EditMessageReplyMarkupAsync<Message>(new EditMessageReplyMarkup
            {
                ChatId = callbackQuery.Message!.Chat.Id,
                MessageId = callbackQuery.Message.MessageId,
                ReplyMarkup = new InlineKeyboardMarkup()
            });
        }

        /// <summary>
        /// Меню
        /// </summary>
        /// <param name="position">Позиция пользователя</param>
        /// <returns>Клавиатура для выбора</returns>
        protected InlineKeyboardMarkup GetIKStart(Positions? position)
        {
            InlineKeyboardButton[][] keyboard = new InlineKeyboardButton[2][] 
            {
                new InlineKeyboardButton[]{ InlineKeyboardButton.SetCallbackData("Ошибка", $"Nothing") },
                new InlineKeyboardButton[]{ InlineKeyboardButton.SetCallbackData("Вернуться", $"Nothing") }
            };

            switch(position)
            {
                case Positions.Пользователь:
                    keyboard = new InlineKeyboardButton[3][];
                    keyboard[0] = new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.SetCallbackData("Поставленные мной задачи", $"deliveredproblems")
                    };
                    keyboard[1] = new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.SetCallbackData("Вернуться", $"Nothing")
                    };
                    break;
                case Positions.ТехСпециалист:
                    keyboard = new InlineKeyboardButton[7][];
                    keyboard[0] = new InlineKeyboardButton[] 
                    {
                        InlineKeyboardButton.SetCallbackData("Поставленные мной задачи", $"Deliveredproblems")
                    };
                    keyboard[1] = new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.SetCallbackData("Поставленные мне задачи", $"Performedproblems")
                    };
                    keyboard[2] = new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.SetCallbackData("Поставленные ботом 1С задачи", $"1cbotproblems")
                    };
                    keyboard[3] = new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.SetCallbackData("Не принятые задачи", $"Notresponsible")
                    };
                    keyboard[4] = new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.SetCallbackData("Редактировать задачу", $"Problemedit")
                    };
                    keyboard[5] = new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.SetCallbackData("Настройки", $"Settings")
                    };
                    keyboard[6] = new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.SetCallbackData("Вернуться", $"Nothing")
                    };
                    break;
                case Positions.Администратор:
                    keyboard = new InlineKeyboardButton[7][];
                    keyboard[0] = new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.SetCallbackData("Поставленные мной задачи", $"Deliveredproblems")
                    };
                    keyboard[1] = new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.SetCallbackData("Поставленные мне задачи", $"Performedproblems")
                    };
                    keyboard[2] = new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.SetCallbackData("Поставленные ботом 1С задачи", $"1cbotproblems")
                    };
                    keyboard[3] = new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.SetCallbackData("Не принятые задачи", $"Notresponsible")
                    };
                    keyboard[4] = new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.SetCallbackData("Редактировать задачу", $"Problemedit")
                    };
                    keyboard[5] = new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.SetCallbackData("Настройки", $"Settings")
                    };
                    keyboard[6] = new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.SetCallbackData("Вернуться", $"Nothing")
                    };
                    break;
                case Positions.СуперАдмин:
                    keyboard = new InlineKeyboardButton[7][];
                    keyboard[0] = new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.SetCallbackData("Поставленные мной задачи", $"Deliveredproblems")
                    };
                    keyboard[1] = new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.SetCallbackData("Поставленные мне задачи", $"Performedproblems")
                    };
                    keyboard[2] = new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.SetCallbackData("Поставленные ботом 1С задачи", $"1cbotproblems")
                    };
                    keyboard[3] = new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.SetCallbackData("Не принятые задачи", $"Notresponsible")
                    };
                    keyboard[4] = new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.SetCallbackData("Редактировать задачу", $"Problemedit")
                    };
                    keyboard[5] = new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.SetCallbackData("Настройки", $"Settings")
                    };
                    keyboard[6] = new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.SetCallbackData("Вернуться", $"Nothing")
                    };
                    break;
                default:
                    keyboard = new InlineKeyboardButton[2][];
                    keyboard[0] = new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.SetCallbackData($"Зарегестрироваться", $"Register")
                    };
                    keyboard[1] = new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.SetCallbackData($"Отменить", $"Nothing")
                    };
                    break;
            }
            return new InlineKeyboardMarkup(keyboard);
        }

        /// <summary>
        /// Получить кнопки для модификации задачи
        /// </summary>
        /// <param name="problemId">ID проблемы</param>
        /// <returns>Клавиатура для выбора</returns>
        protected InlineKeyboardMarkup GetIKModifiedProblem(string problemId)
        {
            InlineKeyboardButton[][] keyboard = new InlineKeyboardButton[4][];
            keyboard[0] = new InlineKeyboardButton[]
            {
                InlineKeyboardButton.SetCallbackData("Задача выполнена", $"SetModifiedProblem {problemId} {ProblemModifications.Выполнена}")
            };
            keyboard[1] = new InlineKeyboardButton[]
            {
                InlineKeyboardButton.SetCallbackData("Изменить приоритет", $"SetModifiedProblem {problemId} {ProblemModifications.Изменить_приоритет}")
            };
            keyboard[2] = new InlineKeyboardButton[]
            {
                InlineKeyboardButton.SetCallbackData("Удалить", $"SetModifiedProblem {problemId} {ProblemModifications.Удалить}")
            };
            keyboard[3] = new InlineKeyboardButton[]
            {
                InlineKeyboardButton.SetCallbackData("Вернуться", $"Nothing")
            };
            return new InlineKeyboardMarkup(keyboard);
        }

        /// <summary>
        /// Получить кнопки для выбора действий с задачей
        /// </summary>
        /// <returns>Клавиатура для выбора</returns>
        protected InlineKeyboardMarkup GetIKChooseMetodsWithProblem ()
        {
            return new InlineKeyboardMarkup(new InlineKeyboardButton[][]
                    {
                        new InlineKeyboardButton[]
                        {
                            InlineKeyboardButton.SetCallbackData("Создать новую задачу", "NewTask")
                        },
                        new InlineKeyboardButton[]
                        {
                            InlineKeyboardButton.SetCallbackData("Добавить комментарий к задаче", "AddCommentChoiseProblem")
                        },
                        new InlineKeyboardButton[]
                        {
                            InlineKeyboardButton.SetCallbackData("Ничего", "Nothing")
                        }
                    });
        }

        /// <summary>
        /// Получить кнопоки для выбора ответственного пользователя
        /// </summary>
        /// <returns>Клавиатура для выбора</returns>
        protected async Task<InlineKeyboardMarkup> GetIKSetResponsibleByTelegramUserAsync()
        {
            var users = await _service.GetListUserTelegramByPositionAsync(andHigher: true);
            InlineKeyboardButton[][] buttons = new InlineKeyboardButton[users.Count() + 1][];
            for (int i = 0; i < users.Count(); i++)
            {
                buttons[i] = new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.SetCallbackData($"{users[i].Name} | {users[i].Position}", $"SetPiority {ResponibleTypes.Пользователь} {users[i].Id}")
                };
            }
            buttons[users.Count()] = new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.SetCallbackData("Вернуться", "NewTask")
                };
            return new InlineKeyboardMarkup(buttons);
        }

        /// <summary>
        /// Получить кнопоки для выбора отдела
        /// </summary>
        /// <returns>Клавиатура для выбора</returns>
        protected InlineKeyboardMarkup GetIKSetResponsibleByPosition()
        {
            var enums = Enum.GetValues<Positions>();
            InlineKeyboardButton[][] buttons = new InlineKeyboardButton[enums.Count()][];
            for (int i = 1; i < enums.Count(); i++)
            {
                buttons[i-1] = new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.SetCallbackData($"{enums[i].ToString()}", $"SetPiority {ResponibleTypes.Отдел} {enums[i]}")
                };
            }
            buttons[enums.Count()-1] = new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.SetCallbackData("Вернуться", "NewTask")
                };
            return new InlineKeyboardMarkup(buttons);
        }

        /// <summary>
        /// Получить кнопки для выбора Пользоватли или Позиции
        /// </summary>
        /// <returns>Клавиатура для выбора</returns>
        protected InlineKeyboardMarkup GetIKChooseResponsible()
        {
            InlineKeyboardButton[][] buttons = new InlineKeyboardButton[3][];
            buttons[0] = new InlineKeyboardButton[]
            {
                InlineKeyboardButton.SetCallbackData($"Отдел", "GetResponsiblePositions")
            };
            buttons[1] = new InlineKeyboardButton[]
            {
                InlineKeyboardButton.SetCallbackData($"Пользователь", "GetResponsibleUsers")
            };
            buttons[2] = new InlineKeyboardButton[]
            {
                InlineKeyboardButton.SetCallbackData("Отменить задачу", "Nothing")
            };
            return new InlineKeyboardMarkup(buttons);
        }

        /// <summary>
        /// Получить кнопки для выбора Приоритета
        /// </summary>
        /// <returns>Клавиатура для выбора</returns>
        protected InlineKeyboardMarkup GetIKChoosePriority(ResponibleTypes responibleType, string whoGet)
        {
            InlineKeyboardButton[][] buttons = new InlineKeyboardButton[3][];
            buttons[0] = new InlineKeyboardButton[]
            {
                InlineKeyboardButton.SetCallbackData($"Низкий", $"CreateTask {responibleType} {whoGet} {Prioritys.Низкий}")
            };
            buttons[1] = new InlineKeyboardButton[]
            {
                InlineKeyboardButton.SetCallbackData($"Средний", $"CreateTask {responibleType} {whoGet} {Prioritys.Средний}")
            };
            buttons[2] = new InlineKeyboardButton[]
            {
                InlineKeyboardButton.SetCallbackData("Высокий", $"CreateTask {responibleType} {whoGet} {Prioritys.Высокий}")
            };
            return new InlineKeyboardMarkup(buttons);
        }

        /// <summary>
        /// Получить кнопки списка доступных задач по пользователю
        /// </summary>
        /// <param name="telegramId">Telegram Id пользователя</param>
        /// <param name="callbackData">Какая команда будет выбрана в методе: "OnCommand",
        /// необходима для опеределения от какой команды получено</param>
        /// <returns>Список кнопок с проблемами для выбора</returns>
        protected async Task<InlineKeyboardMarkup> GetIKWithProblemsAsync(long telegramId, string callbackData)
    {
        var keyboardMarkup = new InlineKeyboardMarkup();
        var problems = (await _service.GetAllProblemsByTelegramIdAsync(telegramId)).ToList();
        TimeSpan ts = new TimeSpan();

        var keyboardButtons = new InlineKeyboardButton[problems.Count() + 1][];
        for (int i = 0; i < problems.Count(); i++)
        {
            ts = DateTime.Now - problems[i].CreateDateTime;
            keyboardButtons[i] = new InlineKeyboardButton[]
            {
                InlineKeyboardButton.SetCallbackData($"Id: {problems[i].Id}, " +
                $"Задача: {problems[i].Text}, " +
                $"Приоритет: {problems[i].Priority}, " +
                $"Задача на выполнении (дней): {ts.Days}, " +
                $"Поставил: {problems[i].UserCreateProblem?.Name ?? "-"}, Выполняет: {problems[i].UserGetProblem?.Name ?? "-"}"
                ,$"{callbackData} {problems[i].Id}")
            };
        }
        keyboardButtons[problems.Count()] = new InlineKeyboardButton[]
        {
            InlineKeyboardButton.SetCallbackData("Закрыть", "Nothing")
        };
        keyboardMarkup.InlineKeyboard = keyboardButtons;
        return keyboardMarkup;
    }

        /// <summary>
        /// Получить кнопки для изменений даты или времени уведомлений
        /// </summary>
        /// <param name="choose">Строка выбора DayOfWeek или HourOnDay</param>
        /// <param name="notifications">Текущие выбранные значения пользователя</param>
        /// <returns>Список кнопок с выбором даты или времени</returns>
        protected InlineKeyboardMarkup GetIKChooseNotification(string choose, string notifications)
        {
            InlineKeyboardButton[][] buttons;

            if (choose == "DayOfWeek")
            {
                var day = 0;
                var ND = new List<DayOfWeekRus>();

                if (notifications.Length > 0)
                {
                    ND = notifications.Split(';').Select(s => (DayOfWeekRus)Enum.Parse(typeof(DayOfWeekRus), s)).ToList();
                }

                var daysString = GetNotificationsOnDayString(ND).ToList();
                buttons = new InlineKeyboardButton[4][];
                for(int i = 0; i < 3; i++)
                {
                    buttons[i] = new InlineKeyboardButton[2];
                    for (int j = 0; j < 2; j++)
                    {
                        buttons[i][j] = InlineKeyboardButton.SetCallbackData(daysString[day],
                            $"NotificationChange DayOfWeek {(DayOfWeekRus)day}");
                        day++;
                    }
                }
                buttons[3] = new InlineKeyboardButton[]
                {
                InlineKeyboardButton.SetCallbackData(daysString[day],
                    $"NotificationChange DayOfWeek {(DayOfWeekRus)day}"),
                InlineKeyboardButton.SetCallbackData($"Подтвердить   {HeavyCheckMark}", "Notification HourOnDay")
                };
            }
            else if (choose == "HourOnDay")
            {
                var hour = 0;
                var NH = new List<int>();

                if (notifications.Length > 0)
                {
                    NH = notifications.Split(';').Select(s => Int32.Parse(s)).ToList();
                }

                var hoursString = GetNotificationsOnHourString(NH).ToList();
                buttons = new InlineKeyboardButton[7][];
                for (int i = 0; i < 6; i++)
                {
                    buttons[i] = new InlineKeyboardButton[4];
                    for (int j = 0; j < 4; j++)
                    {
                        buttons[i][j] = InlineKeyboardButton.SetCallbackData(hoursString[hour],
                            $"NotificationChange HourOnDay {hour}");
                        hour++;
                    }
                }
                buttons[6] = new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.SetCallbackData("Вернуться","Notification DayOfWeek"),
                    InlineKeyboardButton.SetCallbackData($"Завершить {HeavyCheckMark}","Nothing Установлено!")
                };
            }
            else
            {
                buttons = new InlineKeyboardButton[1][];
                buttons[0] = new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.SetCallbackData("Не предвиденная ошибка","Nothing")
                };
            }
            return new InlineKeyboardMarkup(buttons);
        }

        /// <summary>
        /// Выбор настроек
        /// </summary>
        /// <returns>Список кнопок с выбором настроек</returns>
        protected InlineKeyboardMarkup GetIKSettings()
        {
            InlineKeyboardButton[][] buttons = new InlineKeyboardButton[2][];
            buttons[0] = new InlineKeyboardButton[]
            {
                InlineKeyboardButton.SetCallbackData($"Настроить уведомления", $"Notification")
            };
            buttons[1] = new InlineKeyboardButton[]
            {
                InlineKeyboardButton.SetCallbackData($"Вернуться", $"Nothing")
            };
            return new InlineKeyboardMarkup(buttons);
        }

        protected InlineKeyboardMarkup GetIKRegistration()
        {
            InlineKeyboardButton[][] buttons = new InlineKeyboardButton[2][];
            buttons[0] = new InlineKeyboardButton[]
            {
                InlineKeyboardButton.SetCallbackData($"Зарегестрироваться", $"Register")
            };
            buttons[1] = new InlineKeyboardButton[]
            {
                InlineKeyboardButton.SetCallbackData($"Отменить", $"Nothing")
            };
            return new InlineKeyboardMarkup(buttons);
        }





        /// <summary>
        /// Получить строки с значками текущих выбранных значений дня
        /// </summary>
        /// <param name="notificationDays">Список выбранных значений дня</param>
        /// <returns>Список строк с соответсвующими знаками выбора</returns>
        private IEnumerable<string> GetNotificationsOnDayString(List<DayOfWeekRus> notificationDays)
        {
            List<string> days = new List<string>(new string[7]);
            List<bool> list = new List<bool>(new bool[7]);
            foreach(var day in notificationDays)
            {
                list[(int)day] = true;
            }
            for(int i = 0; i < 7; i++)
            {
                days[i] = $"{(DayOfWeekRus)i}   " + (list[i] == true ? RadioButton : WhiteCircle);
            }
            return days;
        }

        /// <summary>
        /// Получить строки с значками текущих выбранных значений времени
        /// </summary>
        /// <param name="notificationHours">Список выбранных значений времени</param>
        /// <returns>Список строк с соответсвующими знаками выбора</returns>
        private IEnumerable<string> GetNotificationsOnHourString(List<int> notificationHours)
        {
            List<string> hours = new List<string>(new string[24]);
            List<bool> list = new List<bool>(new bool[24]);
            foreach (var hour in notificationHours)
            {
                list[hour] = true;
            }
            for(int i = 0; i < 24; i++)
            {
                hours[i] = $"{i}:00 " + (list[i] == true ? RadioButton : WhiteCircle);
            }
            return hours;
        }

    }
}
