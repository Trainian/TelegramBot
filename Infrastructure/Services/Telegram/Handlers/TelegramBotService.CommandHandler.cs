using Infrastructure.Settings;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Microsoft.Extensions.Logging;
using Telegram.BotAPI.AvailableMethods.FormattingOptions;
using ApplicationCore.Entities.Telegram;
using ApplicationCore.Enums;

namespace Infrastructure.Services.Telegram
{
    public partial class TelegramBotService : AsyncTelegramBotBase<TelegramBotSettings>
    {
        protected override async Task OnCommandAsync(Message message, string commandName, string commandParameters, CancellationToken cancellationToken)
        {
            var args = commandParameters.Split(' ');
            var isRegistered = await UserIsRegistered(message.From!.Id);
            var markup = new InlineKeyboardMarkup();

            string answer;
#if DEBUG
            _logger.LogInformation("Params: {0}", args.Length);
#endif

            switch (commandName)
            {
                case "help":
                    answer = string.Format("📝<b>---Как добавить новую задачу---</b>📝\n\n" +
                        "1️⃣ Для добавления новой задачи, достаточно отправить сообщение или изображение с подписью к нему.\n\n" +
                        "2️⃣ После чего, бот у вас уточнит что сделатṁ, ответив на это сообщение вы добавите новую задачу.");
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
                    markup = await GetInlineKeyboardWithProblemsAsync(message.From.Id, "ShowTasks");
                    await Api.SendMessageAsync(message.Chat.Id, answer, parseMode: ParseMode.HTML, replyMarkup: markup, cancellationToken: cancellationToken);
                    break;

                case "performedproblems":
                    if (!isRegistered)
                    {
                        await AnswerIsUserNotRegistered(message, cancellationToken);
                        break;
                    }
                    answer = await GetProblemsMessageAsync(message.From!.Id, WhatTheProblem.Получено);
                    await Api.SendMessageAsync(message.Chat.Id, answer, parseMode: ParseMode.HTML, cancellationToken: cancellationToken);
                    break;

                case "deliveredproblems":
                    if (!isRegistered)
                    {
                        await AnswerIsUserNotRegistered(message, cancellationToken);
                        break;
                    }
                    answer = await GetProblemsMessageAsync(message.From!.Id, WhatTheProblem.Поставлено);
                    await Api.SendMessageAsync(message.Chat.Id, answer, parseMode: ParseMode.HTML, cancellationToken: cancellationToken);
                    break;

                case "problemedit":
                    if (!isRegistered)
                    {
                        await AnswerIsUserNotRegistered(message, cancellationToken);
                        break;
                    }
                    var inlineKeyboard = await GetInlineKeyboardWithProblemsAsync(message.From!.Id, "GetModifiedProblem");
                    if (inlineKeyboard.InlineKeyboard == null)
                        answer = "Нет задач, что можно было бы изменить";
                    else
                        answer = "Выберете Задачу, что хотите изменить:";
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
                    answer = await ChangeTelegramUserPosition(message.From!.Id, Positions.ТехСпециалист);
                    await Api.SendMessageAsync(message.Chat.Id, answer, parseMode: ParseMode.HTML, cancellationToken: cancellationToken);
                    break;

                case "addAdministrator":
                    if (!isRegistered)
                    {
                        await AnswerIsUserNotRegistered(message, cancellationToken);
                        break;
                    }
                    answer = await ChangeTelegramUserPosition(message.From!.Id, Positions.Администратор);
                    await Api.SendMessageAsync(message.Chat.Id, answer, parseMode: ParseMode.HTML, cancellationToken: cancellationToken);
                    break;

                case "addSuperAdmin":
                    if (!isRegistered)
                    {
                        await AnswerIsUserNotRegistered(message, cancellationToken);
                        break;
                    }
                    answer = await ChangeTelegramUserPosition(message.From!.Id, Positions.СуперАдмин);
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

                case "1cbotproblems":
                    if (!isRegistered)
                    {
                        await AnswerIsUserNotRegistered(message, cancellationToken);
                        break;
                    }
                    answer = await GetProblemsMessageAsync(message.From!.Id, WhatTheProblem.Поставил_Бот);
                    await Api.SendMessageAsync(message.Chat.Id, answer, parseMode: ParseMode.HTML, cancellationToken: cancellationToken);
                    break;

                case "notresponsible":
                    if (!isRegistered)
                    {
                        await AnswerIsUserNotRegistered(message, cancellationToken);
                        break;
                    }
                    answer = await GetProblemsMessageAsync(message.From!.Id, WhatTheProblem.Не_принятые);
                    await Api.SendMessageAsync(message.Chat.Id, answer, parseMode: ParseMode.HTML, cancellationToken: cancellationToken);
                    break;

                case "settings":
                    if (!isRegistered)
                    {
                        await AnswerIsUserNotRegistered(message, cancellationToken);
                        break;
                    }
                    markup = GetInlineKeyboardSettings();
                    answer = string.Format("📝<b>---Выберите настройку---</b>📝");
                    await Api.SendMessageAsync(message.Chat.Id, answer, parseMode: ParseMode.HTML, replyMarkup: markup, cancellationToken: cancellationToken);
                    break;

                default:
                    if (message.Chat.Type == ChatType.Private)
                    {
                        await Api.SendMessageAsync(message.Chat.Id, "Нераспознанная команда.", cancellationToken: cancellationToken);
                    }
                    break;
            }
        }
    }
}
