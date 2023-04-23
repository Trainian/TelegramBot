using ApplicationCore.Enums;
using Infrastructure.Settings;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods.FormattingOptions;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.UpdatingMessages;
using ApplicationCore.Entities.Telegram;
using Types = Telegram.BotAPI.AvailableTypes;
using Microsoft.Extensions.Logging;
using Infrastructure.Static;
using System.Threading;
using Infrastructure.Extensions;
using System.Net;

namespace Infrastructure.Services.Telegram
{
    public partial class TelegramBotService : AsyncTelegramBotBase<TelegramBotSettings>
    {
        /// <summary>
        /// Создать новую задачу
        /// </summary>
        /// <param name="callbackQuery">Ответ от клиента</param>
        /// <param name="responibleType">Куда ставить задачу</param>
        /// <param name="priority">Приоритет задачи</param>
        /// <param name="whoGet">Получатель (Пользователь\Отдел)</param>
        /// <returns></returns>
        protected async Task<string> CreateNewTask(CallbackQuery callbackQuery, ResponibleTypes responibleType, string whoGet, Prioritys priority)
        {
            var args = callbackQuery.Data.Split(' ');
            Positions position;

            if (callbackQuery.Message!.ReplyToMessage?.Text != null || callbackQuery.Message!.ReplyToMessage?.Photo != null || callbackQuery.Message!.ReplyToMessage?.Document != null)
            {
                Problem problem = new Problem();
                string? imgPath = null;
                Types.File? document = null;

                if (callbackQuery.Message!.ReplyToMessage?.Photo != null)
                {
                    imgPath = Api.GetFile(
                        callbackQuery.Message.ReplyToMessage.Photo[callbackQuery.Message.ReplyToMessage.Photo.Length - 1]
                        .FileId).FilePath;
                } else if(callbackQuery.Message!.ReplyToMessage?.Document != null)
                {                    
                    document = Api.GetFile(
                        callbackQuery.Message.ReplyToMessage.Document.FileId);
                }

                var text = callbackQuery.Message!.ReplyToMessage?.Text ?? callbackQuery.Message!.ReplyToMessage?.Caption ?? "Без сообщения";

                Enum.TryParse<Positions>(whoGet, true, out position);

                problem = await _service.AddProblemAsync(text, callbackQuery.From!.Id, responibleType, whoGet, priority, imgPath, document);
                await SendMessages(problem.Id, ProblemModifications.Новая_задача, position);
                return $"Успешное создание Задачи с Id: {problem.Id}";
            }
            else
                return "Ошибка при создании задачи, не обнаружено информации";
        }

        /// <summary>
        /// Изменить задачу
        /// </summary>
        /// <param name="callbackQuery">Сообщение в котором происходит выбор</param>
        /// <param name="problemId">Id проблемы</param>
        /// <param name="modification">Строка модификации из перечислителя ProblemModifications</param>
        /// <returns></returns>
        protected async Task ProblemUpdate(CallbackQuery callbackQuery, string problemId, string modification)
        {
            string result = "";
            var markup = new InlineKeyboardMarkup();
            var telegramId = callbackQuery.From!.Id;
            var modificationEnum = Enum.Parse(typeof(ProblemModifications), modification);
            var problem = await _service.GetProblemByProblemIdAsync(int.Parse(problemId));
            if (problem.Id == 0 || problem == null)
                return;
            switch (modificationEnum)
            {
                case ProblemModifications.Выполнена:
                    problem.IsComplited = true;
                    await ClearIK(callbackQuery);
                    await SendMessages(Int32.Parse(problemId), ProblemModifications.Выполнена, null);
                    result = await _service.UpdateProblemAsync(problem);
                    await Api.SendMessageAsync(callbackQuery.Message!.Chat.Id, result);
                    break;

                case ProblemModifications.Удалить:
                    await ClearIK(callbackQuery);
                    await SendMessages(Int32.Parse(problemId), ProblemModifications.Удалить, null);
                    result = await _service.DeleteProblemByIdAsync(problem);
                    await Api.SendMessageAsync(callbackQuery.Message!.Chat.Id, result);
                    break;

                case ProblemModifications.Изменить_приоритет:
                    var keyboard = new InlineKeyboardButton[Enum.GetNames<Prioritys>().Length + 1][];
                    for (int i = 0; i < Enum.GetNames<Prioritys>().Length; i++)
                    {
                        var priority = (Prioritys)(i + 1);
                        keyboard[i] = new InlineKeyboardButton[]
                        {
                            InlineKeyboardButton.SetCallbackData($"{priority}", $"SetPriority {problemId} {priority}")
                        };
                    }
                    keyboard[Enum.GetNames<Prioritys>().Length] = new InlineKeyboardButton[]
                    {
                            InlineKeyboardButton.SetCallbackData($"Вернуться", $"GetModifiedProblem {problemId}")
                    };
                    markup.InlineKeyboard = keyboard;
                    result = await GetProblemInformation(telegramId, problemId);
                    result += "<b>Выберите приоритет:</b>";
                    await Api.EditMessageTextAsync<Message>(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId,
                        result, parseMode: ParseMode.HTML, replyMarkup: markup);
                    break;

                case ProblemModifications.Новый_ответственный:
                    break;

                default:
                    result = "Ошибка команды!";
                    break;
            }
            return;
        }

        /// <summary>
        /// Изменить приоритет задачи
        /// </summary>
        /// <param name="telegramId">Telegram ID пользователя изменяющего приоритет</param>
        /// <param name="problemId">ID задачи</param>
        /// <param name="priority">Новый приоритет</param>
        /// <returns></returns>
        protected async Task<string> ChangePriorityProblem(long telegramId, string problemId, string priority)
        {
            var problem = await _service.GetProblemByProblemIdAsync(int.Parse(problemId));
            if (problem == null)
                return "Ошибка при изменении приоритета";
            problem.Priority = Enum.Parse<Prioritys>(priority);
            await _service.UpdateProblemAsync(problem);
            return "Успешное обновление приоритета!";
        }

        /// <summary>
        /// Создать ответ на задачу
        /// </summary>
        /// <param name="callbackQuery">Ответ от клиента</param>
        /// <param name="problemId">ID задачи</param>
        /// <returns>Сообщение об успехе или ошибке</returns>
        protected async Task<string> CreateNewAnswer(CallbackQuery callbackQuery, string problemId)
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

        /// <summary>
        /// Изменить позицию в котором находится пользователь
        /// </summary>
        /// <param name="telegramId">Telegram Id пользователя, которому меняется приоритет</param>
        /// <param name="position">Новая позиция пользователя</param>
        /// <returns>Сообщение об успехе или ошибке</returns>
        protected async Task<string> ChangeTelegramUserPosition(long telegramId, Positions position)
        {
            return await _service.ChangePositionByTelegramUserIdAsync(telegramId, position);
        }

        /// <summary>
        /// Проверить наличие включенных уведомлений и разослать при необходимости
        /// </summary>
        /// <param name="o">null</param>
        public async void CheckNotifications(object o)
        {
            var lastUpdate = Options.LastNotification.ToString("dd.MM.yy hh");
            var dtNow = DateTime.Now.ToMosccow().ToString("dd.MM.yy hh");

            if(lastUpdate != dtNow)
            {
                var users = await _service.GetListUsersToNeedNotification();
                foreach (var user in users)
                {
                    var answer = await GetProblemsMessageAsync(user.TelegramId, WhatTheProblem.Получено);
                    await Api.SendMessageAsync(user.TelegramId, answer, parseMode: ParseMode.HTML);
                }
                _logger.LogInformation($"({DateTime.Now}) Сработал метод уведомления, с кол-во пользователей: {users.Count()}, прошлый запуск: {Options.LastNotification.ToShortTimeString()}");
            }
            Options.LastNotification = DateTime.Now.ToMosccow();
        }




        /// <summary>
        /// Запрос к сервису, что бы не терялось соединение
        /// </summary>
        /// <param name="o">null</param>
        public async void ReconnectApi(object o)
        {
            var Url = new UriBuilder((String.Concat(_configuration["ApplicationUrl"], "/api/telegram"))).Uri;
            var WebHookToken = _configuration["Telegram:WebhookToken"];
            var client = new HttpClient() { BaseAddress = Url };
            client.DefaultRequestHeaders.Add("X-Telegram-Bot-Api-Secret-Token", WebHookToken);
            var response = await client.GetAsync(Url);
            _logger.LogInformation("Статус запроса к API: " + response.StatusCode.ToString());
        }
    }
}
