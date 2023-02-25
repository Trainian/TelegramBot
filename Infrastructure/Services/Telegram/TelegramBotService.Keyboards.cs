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
        /// <summary>
        /// Очистить список кнопок
        /// </summary>
        /// <param name="callbackQuery">Сообщение для очистки</param>
        /// <returns></returns>
        protected async Task ClearInlineKeyboard(CallbackQuery callbackQuery)
        {
            // Очистка действий на сообщение пользователя
            await Api.EditMessageReplyMarkupAsync<Message>(new EditMessageReplyMarkup
            {
                ChatId = callbackQuery.Message.Chat.Id,
                MessageId = callbackQuery.Message.MessageId,
                ReplyMarkup = new InlineKeyboardMarkup()
            });
        }

        /// <summary>
        /// Получить кнопки для модификации задачи
        /// </summary>
        /// <param name="problemId">ID проблемы</param>
        /// <returns>Клавиатура для выбора</returns>
        protected InlineKeyboardMarkup GetInlineKeyboardToModifiedProblem(string problemId)
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
        protected InlineKeyboardMarkup GetInlineKeyboardToChooseMetdotsWithProblem ()
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
        protected async Task<InlineKeyboardMarkup> GetInlineKeyboardToSetResponsibleByTelegramUserAsync()
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
        protected InlineKeyboardMarkup GetInlineKeyboardToSetResponsibleByPosition()
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
        protected InlineKeyboardMarkup GetInlineKeyboardToChooseResponsible()
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
        protected InlineKeyboardMarkup GetInlineKeyboardToChoosePriority(ResponibleTypes responibleType, string whoGet)
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
        protected async Task<InlineKeyboardMarkup> GetInlineKeyboardWithProblemsAsync(long telegramId, string callbackData)
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
    }
}
