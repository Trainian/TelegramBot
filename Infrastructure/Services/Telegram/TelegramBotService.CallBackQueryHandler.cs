using Infrastructure.Settings;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.InlineMode;
using Telegram.BotAPI.UpdatingMessages;
using Microsoft.Extensions.Logging;
using System.Net;
using ApplicationCore.Entities.Telegram;
using Telegram.BotAPI.AvailableMethods.FormattingOptions;

namespace Infrastructure.Services.Telegram
{
    public partial class TelegramBotService : AsyncTelegramBotBase<TelegramBotSettings>
    {
        protected override async Task OnCallbackQueryAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            var args = callbackQuery.Data.Split(' ');
            string result;

            switch (args[0])
            {
                #region Работа с новыми задачами

                case "NewTask":
                    List<TelegramUser> usersOnPosition = await _service.GetListUserTelegramByPositionAsync(PositionEnum.ТехСпециалист);
                    if(usersOnPosition.Count() > 0)
                    {
                        var newTaskMarkup = CreateInlineKeyboardButtonsToSetResponsible(usersOnPosition);
                        await Api.EditMessageTextAsync<Message>(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId, 
                            "Кому поставить задачу?", replyMarkup: newTaskMarkup);
                    }
                    else
                    {
                        result = await CreateNewProblem(callbackQuery);
                        await ClearInlineKeyboard(callbackQuery);
                        Api.SendMessage(callbackQuery.Message!.Chat.Id, result);
                    }
                break;

                case "SetResponsible":
                    result = await CreateNewProblem(callbackQuery, args[1]);
                    await ClearInlineKeyboard(callbackQuery);
                    await Api.SendMessageAsync(callbackQuery.Message!.Chat.Id, result);
                    break;

                #endregion

                #region Отобразить задачу

                case "ShowTasks":
                    result = await GetProblemInformation(callbackQuery.From!.Id, args[1]);
                    await ClearInlineKeyboard(callbackQuery);
                    await Api.SendMessageAsync(callbackQuery.Message!.Chat.Id, result, parseMode: ParseMode.HTML);
                    break;

                #endregion

                #region Работа с комментариями

                case "AddCommentChoiseProblem":
                    var inlineKeyboard = await GetInlineKeyboardMyProblems(callbackQuery.From!.Id, "AddComment");
                    await Api.EditMessageTextAsync<Message>(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId,
                        "Выберете задачу(проблему) к которой добавить комментарий", replyMarkup: inlineKeyboard);
                    break;

                case "AddComment":
                    result = await CreateNewAnswer(callbackQuery, args[1]);
                    await ClearInlineKeyboard(callbackQuery);
                    await Api.SendMessageAsync(callbackQuery.Message!.Chat.Id, result);
                    await SendMessagesAboutUpdateProblem(callbackQuery.From.Id, args[1], ProblemModificationEnum.Новый_комментарий);
                    break;

                #endregion

                #region Работа с модификацией проблемы (задачи)

                case "GetModifiedProblem":
                    result = await GetProblemInformation(callbackQuery.From!.Id, args[1]);
                    var getModifiedmarkup = GetInlineKeyboardToModifiedProblem(args[1]);
                    await Api.EditMessageTextAsync<Message>(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId,
                        result, parseMode: ParseMode.HTML, replyMarkup: getModifiedmarkup);
                    break;

                case "SetModifiedProblem":
                    await ProblemUpdate(callbackQuery, args[1], args[2]);
                    break;

                case "SetPriority":
                    result = await ChangePriorityProblem(callbackQuery.From!.Id, args[1], args[2]);
                    await ClearInlineKeyboard(callbackQuery);
                    Api.SendMessage(callbackQuery.Message!.Chat.Id, result);
                    await SendMessagesAboutUpdateProblem(callbackQuery.From.Id, args[1], ProblemModificationEnum.Изменить_приоритет);
                    break;

                #endregion

                case "Nothing":
                    await ClearInlineKeyboard(callbackQuery);
                    await Api.SendMessageAsync(callbackQuery.Message!.Chat.Id, "Успешно пропущено");
                    break;

                default:
                    break;
            }
            await base.OnCallbackQueryAsync(callbackQuery, cancellationToken);
        }

        /// <summary>
        /// Создать список кнопок, для выбора пользователя\ответственного
        /// </summary>
        /// <param name="users">Список пользователей</param>
        /// <returns>Список для выбора</returns>
        private InlineKeyboardMarkup CreateInlineKeyboardButtonsToSetResponsible (List<TelegramUser> users)
        {
            InlineKeyboardButton[][] userButtons = new InlineKeyboardButton[users.Count() + 1][];
            for (int i = 0; i < users.Count(); i++)
            {
                userButtons[i] = new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.SetCallbackData($"{users[i].Name} | {users[i].Position}", $"SetResponsible {users[i].Id}")
                };
            }
            userButtons[users.Count()] = new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.SetCallbackData("Отменить задачу", "Nothing")
                };
            return new InlineKeyboardMarkup(userButtons);
        }

        /// <summary>
        /// Очистить выбор для пользователя
        /// </summary>
        /// <param name="callbackQuery">Ответ пользователя</param>
        /// <returns></returns>
        private async Task ClearInlineKeyboard(CallbackQuery callbackQuery)
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
        /// Создать новую задачу(проблему)
        /// </summary>
        /// <param name="callbackQuery">Ответ пользователя</param>
        /// <param name="userGet">Укажите пользователя, которму необходимо поставить задачу,
        /// при его отсутствии пропустить.</param>
        /// <returns></returns>
        private async Task<string> CreateNewProblem (CallbackQuery callbackQuery, string? userGet = null)
        {
            Problem problem = new Problem();
            // Обработка тестового сообщения
            if (callbackQuery.Message!.ReplyToMessage?.Text != null)
            {
                var text = callbackQuery.Message.ReplyToMessage.Text;

                problem = await _service.AddProblemAsync(text!, callbackQuery.From!.Id, userGet: userGet);
                await SendMessagesAboutUpdateProblem(callbackQuery.From!.Id, problem.Id.ToString(), ProblemModificationEnum.Новая_задача);
                return "Успешное создание Задачи(проблеы)";
            }

            // Обработка сообщения с изображением
            else if (callbackQuery.Message!.ReplyToMessage?.Photo != null)
            {
                var photoPath = Api.GetFile(
                    callbackQuery.Message.ReplyToMessage.Photo[callbackQuery.Message.ReplyToMessage.Photo.Length - 1]
                    .FileId).FilePath;
                var text = callbackQuery.Message.ReplyToMessage.Caption ?? "Без сообщения";

                problem = await _service.AddProblemAsync(text, callbackQuery.From!.Id, photoPath, userGet: userGet);
                await SendMessagesAboutUpdateProblem(callbackQuery.From!.Id, problem.Id.ToString(), ProblemModificationEnum.Новая_задача);
                return "Успешное создание Задачи(проблеы)";
            }
            else
                return "Ошибка при создании задачи";
        }

        private InlineKeyboardMarkup GetInlineKeyboardToModifiedProblem (string problemId)
        {
            InlineKeyboardButton[][] keyboard = new InlineKeyboardButton[4][];
            keyboard[0] = new InlineKeyboardButton[]
            {
                InlineKeyboardButton.SetCallbackData("Задача выполнена", $"SetModifiedProblem {problemId} {ProblemModificationEnum.Выполнена}")
            };
            keyboard[1] = new InlineKeyboardButton[]
            {
                InlineKeyboardButton.SetCallbackData("Изменить приоритет", $"SetModifiedProblem {problemId} {ProblemModificationEnum.Изменить_приоритет}")
            };
            keyboard[2] = new InlineKeyboardButton[]
            {
                InlineKeyboardButton.SetCallbackData("Удалить", $"SetModifiedProblem {problemId} {ProblemModificationEnum.Удалить}")
            };
            keyboard[3] = new InlineKeyboardButton[]
            {
                InlineKeyboardButton.SetCallbackData("Вернуться", $"Nothing")
            };
            return new InlineKeyboardMarkup(keyboard);
        }

        private async Task<string> GetProblemInformation (long telegramId, string problemId)
        {
            var problem = await _service.GetProblemByProblemId(telegramId ,int.Parse(problemId));
            TimeSpan ts = DateTime.Now - problem.CreateDateTime;
            if (problem.Id == 0)
                return "Ошибка поиска проблемы";
            var message = $"Id задачи(ошибки): <b>{problem.Id}</b>\n" +
                $"Задача: <b>{problem.Text}</b>\n" +
                $"Приоритет: <b>{problem.Priority}</b> \\ Создан: <b>{problem.CreateDateTime.ToShortDateString()}</b>\n" +
                $"Выполняется (дней): <b>{ts.Days}</b>\n" +
                $"Поставил: <b>{problem.UserCreateProblem!.Name}</b> \\ Выполняет: <b>{problem.UserGetProblem?.Name ?? "-"}</b>\n\n";
            foreach (var answer in problem.Answers.OrderBy(a => a.Id))
            {
                message += $"🗨️ <i>{answer.UserCreate!.Name}</i>: \n{answer.Text}\n";
            }
            return message;
        }

        private async Task ProblemUpdate (CallbackQuery callbackQuery, string problemId, string modification)
        {
            string result = "";
            var markup = new InlineKeyboardMarkup();
            var telegramId = callbackQuery.From!.Id;
            var modificationEnum = Enum.Parse(typeof(ProblemModificationEnum), modification);
            var problem = await _service.GetProblemByProblemId(telegramId, int.Parse(problemId));
            if (problem.Id == 0)
                return;
            switch (modificationEnum)
            {
                case ProblemModificationEnum.Выполнена:
                    problem.IsComplited = true;
                    await ClearInlineKeyboard(callbackQuery);
                    await SendMessagesAboutUpdateProblem(callbackQuery.From.Id, problemId, ProblemModificationEnum.Выполнена);
                    result = await _service.UpdateProblemAsync(problem);
                    await Api.SendMessageAsync(callbackQuery.Message!.Chat.Id, result);
                    break;

                case ProblemModificationEnum.Удалить:
                    await ClearInlineKeyboard(callbackQuery);
                    await SendMessagesAboutUpdateProblem(callbackQuery.From.Id, problemId, ProblemModificationEnum.Удалить);
                    result = await _service.DeleteProblemByIdAsync(problem);
                    await Api.SendMessageAsync(callbackQuery.Message!.Chat.Id, result);
                    break;

                case ProblemModificationEnum.Изменить_приоритет:
                    var keyboard = new InlineKeyboardButton[Enum.GetNames<PriorityEnum>().Length + 1][];
                    for (int i = 0; i < Enum.GetNames<PriorityEnum>().Length; i++)
                    {
                        var priority = (PriorityEnum)(i + 1);
                        keyboard[i] = new InlineKeyboardButton[]
                        {
                            InlineKeyboardButton.SetCallbackData($"{priority}", $"SetPriority {problemId} {priority}")
                        };
                    }
                    keyboard[Enum.GetNames<PriorityEnum>().Length] = new InlineKeyboardButton[]
                    {
                            InlineKeyboardButton.SetCallbackData($"Вернуться", $"GetModifiedProblem {problemId}")
                    };
                    markup.InlineKeyboard = keyboard;
                    result = await GetProblemInformation(telegramId, problemId);
                    result += "<b>Выберете приоритет:</b>";
                    await Api.EditMessageTextAsync<Message>(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId,
                        result, parseMode: ParseMode.HTML, replyMarkup: markup);
                    break;

                default:
                    result = "Ошибка команды!";
                    break;
            }
            return;
        }

        private async Task<string> ChangePriorityProblem (long telegramId, string problemId, string priority)
        {
            var problem = await _service.GetProblemByProblemId(telegramId, int.Parse(problemId));
            if (problem == null)
                return "Ошибка при изминении приоритета";
            problem.Priority = Enum.Parse<PriorityEnum>(priority);
            await _service.UpdateProblemAsync(problem);
            return "Успешное обновление приоритета!";
        }

        private async Task<string> CreateNewAnswer(CallbackQuery callbackQuery, string problemId)
        {
            // Обработка тестового сообщения
            if (callbackQuery.Message!.ReplyToMessage?.Text != null)
            {
                var text = callbackQuery.Message.ReplyToMessage.Text;
                return await _service.AddProblemComment(int.Parse(problemId), text, callbackQuery.From!.Id);
            }

            // Обработка сообщения с изображением
            else if (callbackQuery.Message!.ReplyToMessage?.Photo != null)
            {
                var photoPath = Api.GetFile(
                    callbackQuery.Message.ReplyToMessage.Photo[callbackQuery.Message.ReplyToMessage.Photo.Length - 1]
                    .FileId).FilePath;
                var text = callbackQuery.Message.ReplyToMessage.Caption ?? "Без сообщения";

                return await _service.AddProblemComment(int.Parse(problemId), text, callbackQuery.From!.Id, photoPath);
            }
            else
                return "Ошибка при создании комментария";
        }

        private async Task SendMessagesAboutUpdateProblem (long telegramId, string problemId, ProblemModificationEnum modification)
        {
            string message = "";
            var problem = await _service.GetProblemByProblemId(telegramId, int.Parse(problemId));
            HashSet<TelegramUser> users = new HashSet<TelegramUser>();
            users.Add(problem.UserCreateProblem!);
            if(problem.UserGetProblem != null)
                users.Add(problem.UserGetProblem);

            switch(modification)
            {
                case ProblemModificationEnum.Выполнена:
                    message = $"Задача c <b>Id:{problem.Id}</b> была <b>Закрыта</b>";
                    break;
                case ProblemModificationEnum.Удалить:
                    message = $"Задача c <b>Id:{problem.Id}</b> была <b>Удалена</b>";
                    break;
                case ProblemModificationEnum.Новый_комментарий:
                    var answer = problem.Answers.OrderBy(a => a.Id).Last();
                    message = $"В задаче c <b>Id:{problem.Id}</b> был добавлен новый комментарий:\n 🗨️<b>{answer.UserCreate!.Name}</b>\n{answer.Text}";
                    break;
                case ProblemModificationEnum.Изменить_приоритет:
                    message = $"В задаче c <b>Id:{problem.Id}</b> был изменен приоритет на: <b>{problem.Priority}</b>";
                    break;
                case ProblemModificationEnum.Новая_задача:
                    message = $"Была поставлена новая задача <b>Id: {problem.Id}</b> Задача: <b>{problem.Text}</b>, от <b>{problem.UserCreateProblem!.Name}</b>";
                    break;
            }
            foreach (var user in users)
                await Api.SendMessageAsync(user.TelegramId, message, parseMode: ParseMode.HTML);
        }

        private enum ProblemModificationEnum
        {
            Выполнена = 1,
            Удалить = 2,
            Изменить_приоритет = 3,
            Новый_комментарий = 4,
            Новая_задача = 5
        }
    }
}
