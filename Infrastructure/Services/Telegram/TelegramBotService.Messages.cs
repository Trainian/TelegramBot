using ApplicationCore.Entities.Telegram;
using ApplicationCore.Enums;
using Infrastructure.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableMethods.FormattingOptions;
using Telegram.BotAPI.AvailableTypes;

namespace Infrastructure.Services.Telegram
{
    public partial class TelegramBotService : AsyncTelegramBotBase<TelegramBotSettings>
    {
		/// <summary>
		/// Отправить сообщение об обновлении проблемы
		/// </summary>
		/// <param name="telegramId">Telegram ID пользователя создающего сообщение</param>
		/// <param name="problemId">ID проблемы</param>
		/// <param name="modification">Перечислитель указывающий на изминения</param>
		/// <returns></returns>
		protected async Task SendMessages(int problemId, ProblemModifications modification, Positions? position)
		{
            HashSet<TelegramUser> users = new HashSet<TelegramUser>();
            string message = "";
			var problem = await _service.GetProblemByProblemIdAsync(problemId);

            //users.Add(problem.UserCreateProblem!);
            if (problem.UserGetProblemId != null)
                users.Add(problem.UserGetProblem!);
            else
            {
                var usersList = await _service.GetListUserTelegramByPositionAsync(position ?? Positions.ТехСпециалист, position == null ? true : false);
                foreach (var user in usersList)
                    users.Add(user);
            }

            switch (modification)
			{
				case ProblemModifications.Выполнена:
					message = $"Задача: \nId:<b>{problem.Id}</b>\nСообщение: <b>{problem.Text}</b>\nбыла <b>Закрыта</b>";
					break;
				case ProblemModifications.Удалить:
					message = $"Задача: \nId:<b>{problem.Id}</b>\nСообщение: <b>{problem.Text}</b>\nбыла <b>Удалена</b>";
					break;
				case ProblemModifications.Новый_комментарий:
					var answer = problem.Answers.OrderBy(a => a.Id).Last();
					message = $"К Задаче Id:<b>{problem.Id}</b> был добавлен новый комментарий: \nСообщение: <b>{problem.Text}</b>\n:\n 🗨️<b>{answer.UserCreate!.Name}</b>\n{answer.Text}";
					break;
				case ProblemModifications.Изменить_приоритет:
					message = $"В Задача: \nId:<b>{problem.Id}</b>\nСообщение: <b>{problem.Text}</b>\nбыл изменен приоритет на: <b>{problem.Priority}</b>";
					break;
				case ProblemModifications.Новая_задача:
					message = $"Была поставлена новая задача \nId: <b>{problem.Id}</b>\nСообщение: <b>{problem.Text}</b>,\nПостановщик: <b>{problem.UserCreateProblem!.Name}</b>";
					break;
			}

            if (problem.Img != null)
                message += $"Закреплено: {_configuration["ApplicationUrl"]}{problem.Img}\n";

            foreach (var user in users)
            {
                try
                {
                    await Api.SendMessageAsync(user.TelegramId, message, parseMode: ParseMode.HTML);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex.Message);
                }
            }
		}

        //TODO: Объединить GetProblemInformation с GetProblemsMessageAsync, для формирования сообщения, switch вынести в отдельный метод

        /// <summary>
        /// Получить сообщение о задаче
        /// </summary>
        /// <param name="telegramId">Telegram ID пользователя создающего сообщение</param>
        /// <param name="problemId">ID проблемы</param>
        /// <returns>Сообщение о проблеме или её отсутсвии</returns>
        protected async Task<string> GetProblemInformation(long telegramId, string problemId)
        {
            var problem = await _service.GetProblemByProblemIdAsync(int.Parse(problemId));
            TimeSpan ts = DateTime.Now - problem.CreateDateTime;
            if (problem.Id == 0 || problem == null)
                return "Ошибка поиска проблемы";
            var message = $"Id задачи: <b>{problem.Id}</b>\n" +
                $"Задача: <b>{problem.Text}</b>\n" +
                $"Приоритет: <b>{problem.Priority}</b> \\ Создан: <b>{problem.CreateDateTime.ToShortDateString()}</b>\n" +
                $"Выполняется (дней): <b>{ts.Days}</b>\n" +
                $"Поставил: <b>{problem.UserCreateProblem!.Name}</b> \\ Выполняет: <b>{problem.UserGetProblem?.Name ?? "-"}</b>\n";

            if (problem.Img != null)
                message += $"Закреплено: {_configuration["ApplicationUrl"]}{problem.Img}\n";

            foreach (var answer in problem.Answers.OrderBy(a => a.Id))
            {
                message += $"🗨️ <i>{answer.UserCreate!.Name}</i>: \n{answer.Text}\n";
            }
            return message;
        }

        /// <summary>
        /// Получить список проблем в формате строки
        /// </summary>
        /// <param name="telegramId">Id пользователя Телеграм</param>
        /// <param name="whatTheProblem">Какие проблемы, поставленные или получаемые</param>
        /// <returns>Строка вида HTML</returns>
        protected async Task<string> GetProblemsMessageAsync(long telegramId, WhatTheProblem whatTheProblem)
        {
            var message = $"Задач что {whatTheProblem}, не найдено!";
            IEnumerable<Problem> result = new List<Problem>();

            switch (whatTheProblem)
            {
                case WhatTheProblem.Получено:
                    result = await _service.GetPerformedProblemsByTelegramIdAsync(telegramId);
                    break;
                case WhatTheProblem.Поставлено:
                    result = await _service.GetDeliveredProblemsByTelegramIdAsync(telegramId);
                    break;
                case WhatTheProblem.Поставил_Бот:
                    result = await _service.GetDeliveredProblemsByTelegramIdAsync(0);
                    break;
                case WhatTheProblem.Не_принятые:
                    result = await _service.GetAllProblemsWithoutResponsible();
                    break;
            }

            if (result.Count() != 0)
            {
                message = $"📝<b>---Кол-во Задач что {whatTheProblem} : {result.Count()}шт.---</b>📝\n\n";
                foreach (var problem in result)
                {
                    message += $"Id задачи: <b>{problem.Id}</b>\n" +
                        $"Текст: <b>{problem.Text}</b>\nПриоритет: <b>{problem.Priority}</b> \\ Создан: <b>{problem.CreateDateTime.ToShortDateString()}</b>\n" +
                        $"Поставил: <b>{problem.UserCreateProblem!.Name}</b> \\ Выполняет: <b>{problem.UserGetProblem?.Name ?? "-"}</b>\n\n";
                    foreach (var answer in problem.Answers)
                    {
                        message += $"🗨️ <i>{answer.UserCreate!.Name} ({answer.CreateDateTime.ToString("g")})</i>: \n{answer.Text}\n";
                    }
                    message += "\n➖➖➖➖➖\n\n";
                }
            }
            return message;
        }

        /// <summary>
        /// Сообщение о не Зарегестрированном пользователе
        /// </summary>
        /// <param name="message">Сообщение пользователя</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns></returns>
        protected async Task AnswerIsUserNotRegistered(Message message, CancellationToken cancellationToken)
        {
            var answer = "Пользователь не найден в системе, зарегестрируйтесь!";
            await Api.SendMessageAsync(message.Chat.Id, answer, parseMode: ParseMode.HTML, cancellationToken: cancellationToken);
        }

        //TODO: Отправка сообщения, создать общий метод (объединить с другим\и)
        public async void SendMessage(long telegramId, string message)
        {
            try
            {
                await Api.SendMessageAsync(telegramId, message, parseMode: ParseMode.HTML);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
        }
    }
}
