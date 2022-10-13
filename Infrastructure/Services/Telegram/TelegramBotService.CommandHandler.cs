using Infrastructure.Settings;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Microsoft.Extensions.Logging;
using Telegram.BotAPI.AvailableMethods.FormattingOptions;
using ApplicationCore.Entities.Telegram;

namespace Infrastructure.Services.Telegram
{
    public partial class TelegramBotService : AsyncTelegramBotBase<TelegramBotSettings>
    {
        protected override async Task OnCommandAsync(Message message, string commandName, string commandParameters, CancellationToken cancellationToken)
        {
            var args = commandParameters.Split(' ');
            var isRegistered = await UserIsRegistered(message.From!.Id);

            string answer;
#if DEBUG
            _logger.LogInformation("Params: {0}", args.Length);
#endif

            switch (commandName)
            {
                case "help":
                    answer = string.Format("📝<b>---Как добавить новую задачу---</b>📝\n\n" +
                        "1️⃣ Для добавления новой задачи, достаточно отправить сообщение или изображение с подписью к нему.\n\n" +
                        "2️⃣ После чего, бот у вас уточнит создавать задачу, ответив на это сообщение вы добавите новую задачу.");
                    await Api.SendMessageAsync(message.Chat.Id, answer, parseMode: ParseMode.HTML, cancellationToken: cancellationToken);
                    break;

                case "register":
                    answer = await _service.CreateTelegramUser(message.From!);
                    await Api.SendMessageAsync(message.Chat.Id, answer, parseMode: ParseMode.HTML, cancellationToken: cancellationToken);
                    break;

                case "tasks":
                    if (!isRegistered)
                    {
                        await AnswerIsUserNotRegistered(message, cancellationToken);
                        break;
                    }
                    answer = string.Format("📝<b>---Выберете задачу, для прочтения---</b>📝");
                    var markup = await GetInlineKeyboardMyProblems(message.From.Id, "ShowTasks");
                    await Api.SendMessageAsync(message.Chat.Id, answer, parseMode: ParseMode.HTML, replyMarkup: markup, cancellationToken: cancellationToken);
                    break;

                case "performedproblems":
                    if (!isRegistered)
                    {
                        await AnswerIsUserNotRegistered(message, cancellationToken);
                        break;
                    }
                    answer = await GetProblemsMessage(message.From!.Id, WhatTheProblem.Получено);
                    await Api.SendMessageAsync(message.Chat.Id, answer, parseMode: ParseMode.HTML, cancellationToken: cancellationToken);
                    break;

                case "deliveredproblems":
                    if (!isRegistered)
                    {
                        await AnswerIsUserNotRegistered(message, cancellationToken);
                        break;
                    }
                    answer = await GetProblemsMessage(message.From!.Id, WhatTheProblem.Поставлено);
                    await Api.SendMessageAsync(message.Chat.Id, answer, parseMode: ParseMode.HTML, cancellationToken: cancellationToken);
                    break;

                case "problemedit":
                    if (!isRegistered)
                    {
                        await AnswerIsUserNotRegistered(message, cancellationToken);
                        break;
                    }
                    var inlineKeyboard = await GetInlineKeyboardMyProblems(message.From!.Id, "GetModifiedProblem");
                    if (inlineKeyboard.InlineKeyboard == null)
                        answer = "Нет задача(проблем), что можно было бы изменить";
                    else
                        answer = "Выберете Задачу(Проблему), что хотите изменить:";
                    await Api.SendMessageAsync(message.Chat.Id, answer, parseMode: ParseMode.HTML, cancellationToken: cancellationToken, replyMarkup: inlineKeyboard);
                    break;

                case "adminCommands":
                    #region Команды
                    answer = string.Format("📝<b>---Возможности команд Администратора---</b>📝\n\n" +
                        "1️⃣ \'/changeUserPosition\' - Изменяет позицию пользователя (в разработке)\n\n" +
                        "📝<b>---Возможности команд всех Пользователей---</b>📝\n\n" +
                        "Предоставьте пользователю команду, для изминения его позиции в команде\n" +
                        "1️⃣ \'/addTechSpecialist\' - Изменяет позицию пользователя на \'Тех.Специалист\'\n'" +
                        "2️⃣ \'/addAdministrator\' - Изменяет позицию пользователя на \'Администратор\'\n" +
                        "3️⃣ \'/addSuperAdmin\' - Изменяет позицию пользователя на \'СуперАдминистратор\'\n");
                    await Api.SendMessageAsync(message.Chat.Id, answer, parseMode: ParseMode.HTML, cancellationToken: cancellationToken);
                    break;
                #endregion

                case "addTechSpecialist":
                    if (!isRegistered)
                    {
                        await AnswerIsUserNotRegistered(message, cancellationToken);
                        break;
                    }
                    answer = await ChangeTelegramUserPosition(message.From!.Id, PositionEnum.ТехСпециалист);
                    await Api.SendMessageAsync(message.Chat.Id, answer, parseMode: ParseMode.HTML, cancellationToken: cancellationToken);
                    break;

                case "addAdministrator":
                    if (!isRegistered)
                    {
                        await AnswerIsUserNotRegistered(message, cancellationToken);
                        break;
                    }
                    answer = await ChangeTelegramUserPosition(message.From!.Id, PositionEnum.Администратор);
                    await Api.SendMessageAsync(message.Chat.Id, answer, parseMode: ParseMode.HTML, cancellationToken: cancellationToken);
                    break;

                case "addSuperAdmin":
                    if (!isRegistered)
                    {
                        await AnswerIsUserNotRegistered(message, cancellationToken);
                        break;
                    }
                    answer = await ChangeTelegramUserPosition(message.From!.Id, PositionEnum.СуперАдминистратор);
                    await Api.SendMessageAsync(message.Chat.Id, answer, parseMode: ParseMode.HTML, cancellationToken: cancellationToken);
                    break;

                case "changeUserPosition":
                    if (!isRegistered)
                    {
                        await AnswerIsUserNotRegistered(message, cancellationToken);
                        break;
                    }
                    answer = string.Format("Находится в разработке");
                    await Api.SendMessageAsync(message.Chat.Id, answer, parseMode: ParseMode.HTML, cancellationToken: cancellationToken);
                    break;

                default:
                    if (message.Chat.Type == ChatType.Private)
                    {
                        await Api.SendMessageAsync(message.Chat.Id, "Нераспознанная команда.", cancellationToken: cancellationToken);
                    }
                    break;
            }
        }

        private async Task<bool> UserIsRegistered (long telegramId)
        {
            var telegramUser = await _service.GetUserTelegramByTelegramId(telegramId);
            return telegramUser != null ? true : false;
        }
        private async Task AnswerIsUserNotRegistered (Message message, CancellationToken cancellationToken)
        {
            var answer = "Пользователь не найден в системе, зарегестрируйтесь!";
            await Api.SendMessageAsync(message.Chat.Id, answer, parseMode: ParseMode.HTML, cancellationToken: cancellationToken);
        }
        /// <summary>
        /// Создать список доступных задач(проблем)
        /// </summary>
        /// <param name="telegramId">Telegram Id пользователя</param>
        /// <param name="callbackData">Какая команда будт выбрана в методе: "OnCommand",
        /// необходима для опеределения от какой команды получено</param>
        /// <returns>Список кнопок с проблемами для выбора</returns>
        private async Task<InlineKeyboardMarkup> GetInlineKeyboardMyProblems (long telegramId, string callbackData)
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

        private async Task<string> ChangeTelegramUserPosition(long telegramId, PositionEnum position)
        {
            return await _service.ChangePositionByTelegramUserIdAsync(telegramId, position);
        }

        /// <summary>
        /// Получить список проблем в формате строки
        /// </summary>
        /// <param name="telegramId">Id пользователя Телеграм</param>
        /// <param name="whatTheProblem">Какие проблемы, поставленные или получаемые</param>
        /// <returns>Строка вида HTML</returns>
        private async Task<string> GetProblemsMessage (long telegramId, WhatTheProblem whatTheProblem)
        {
            var message = $"Задач(Проблем) что {whatTheProblem}, не найдено!";
            IEnumerable<Problem> result = new List<Problem>();

            switch(whatTheProblem)
            {
                case WhatTheProblem.Получено:
                    result = await _service.GetPerformedProblemsByTelegramIdAsync(telegramId);
                    break;
                case WhatTheProblem.Поставлено:
                    result = await _service.GetDeliveredProblemsByTelegramIdAsync(telegramId);
                    break;
            }

            if(result.Count() != 0)
            {
                message = $"📝<b>---Кол-во Задач(Проблем) что {whatTheProblem} : {result.Count()}шт.---</b>📝\n\n";
                foreach (var problem in result)
                {
                    message += $"Id задачи(ошибки): <b>{problem.Id}</b>\n" +
                        $"Текст: <b>{problem.Text}</b>\nПриоритет: <b>{problem.Priority}</b> \\ Создан: <b>{problem.CreateDateTime.ToShortDateString()}</b>\n" +
                        $"Поставил: <b>{problem.UserCreateProblem!.Name}</b> \\ Выполняет: <b>{problem.UserGetProblem?.Name ?? "-"}</b>\n\n";
                    foreach (var answer in problem.Answers)
                    {
                        message += $"🗨️ <i>{answer.UserCreate!.Name}</i>: \n{answer.Text}\n";
                    }
                    message += "\n➖➖➖➖➖\n\n";
                }
            }
            return message;
        }

        private enum WhatTheProblem
        {
            Поставлено = 1,
            Получено = 2,
            Все = 3
        }
    }
}
