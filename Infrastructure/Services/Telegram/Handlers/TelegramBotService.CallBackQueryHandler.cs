using Infrastructure.Settings;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Payments;
using Telegram.BotAPI.TelegramPassport;
using Telegram.BotAPI.InlineMode;
using Telegram.BotAPI.UpdatingMessages;
using Microsoft.Extensions.Logging;
using System.Net;
using ApplicationCore.Entities.Telegram;
using Telegram.BotAPI.AvailableMethods.FormattingOptions;
using ApplicationCore.Enums;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Infrastructure.Services.Telegram
{
    public partial class TelegramBotService : AsyncTelegramBotBase<TelegramBotSettings>
    {
        protected override async Task OnCallbackQueryAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            var args = callbackQuery!.Data!.Split(' ');
            TelegramUser? user;
            InlineKeyboardMarkup? markup = new InlineKeyboardMarkup();
            string result;

            switch (args[0])
            {
                //TODO: Установить ответсвенного к задаче

                //TODO: Изменить позицию пользователя

                #region Работа с новыми задачами

                case "NewTask":
                    markup = GetIKChooseResponsible();
                    await Api.EditMessageTextAsync<Message>(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId,
                        "Куда ставим задачу?", replyMarkup: markup);
                    break;

                case "GetResponsiblePositions":
                    markup = GetIKSetResponsibleByPosition();
                    await Api.EditMessageTextAsync<Message>(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId,
                        "Выберите отдел:", replyMarkup: markup);
                    break;
                    
                case "GetResponsibleUsers":
                    markup = await GetIKSetResponsibleByTelegramUserAsync();
                    await Api.EditMessageTextAsync<Message>(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId,
                        "Выберите ответсвенного:", replyMarkup: markup);
                    break;

                case "SetPiority":
                    markup = GetIKChoosePriority(Enum.Parse<ResponibleTypes>(args[1]), args[2]);
                    await Api.EditMessageTextAsync<Message>(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId,
                        "Выберите приоритет:", replyMarkup: markup);
                    break;
                    
                case "CreateTask":
                    result = await CreateNewTask(callbackQuery, Enum.Parse<ResponibleTypes>(args[1]), args[2], Enum.Parse<Prioritys>(args[3]));
                    await ClearIK(callbackQuery);
                    await Api.SendMessageAsync(callbackQuery.Message!.Chat.Id, result);
                    break;

                #endregion

                #region Работа с текущими задачами

                case "ShowTasks":
                    result = await GetProblemInformation(callbackQuery.From!.Id, args[1]);
                    await ClearIK(callbackQuery);
                    await Api.SendMessageAsync(callbackQuery.Message!.Chat.Id, result, parseMode: ParseMode.HTML);
                    break;

                case "Deliveredproblems":
                    result = await GetProblemsMessageAsync(callbackQuery.From!.Id, WhatTheProblem.Поставлено);
                    await Api.SendMessageAsync(callbackQuery.Message!.Chat.Id, result, parseMode: ParseMode.HTML, cancellationToken: cancellationToken);
                    break;

                case "Performedproblems":
                    result = await GetProblemsMessageAsync(callbackQuery.From!.Id, WhatTheProblem.Получено);
                    await Api.SendMessageAsync(callbackQuery.Message!.Chat.Id, result, parseMode: ParseMode.HTML, cancellationToken: cancellationToken);
                    break;

                case "1cbotproblems":
                    result = await GetProblemsMessageAsync(callbackQuery.From!.Id, WhatTheProblem.Поставил_Бот);
                    await Api.SendMessageAsync(callbackQuery.Message!.Chat.Id, result, parseMode: ParseMode.HTML, cancellationToken: cancellationToken);
                    break;

                case "Notresponsible":
                    result = await GetProblemsMessageAsync(callbackQuery.From!.Id, WhatTheProblem.Не_принятые);
                    await Api.SendMessageAsync(callbackQuery.Message!.Chat.Id, result, parseMode: ParseMode.HTML, cancellationToken: cancellationToken);
                    break;

                case "Problemedit":
                    markup = await GetIKWithProblemsAsync(callbackQuery.From!.Id, "GetModifiedProblem");
                    if (markup.InlineKeyboard == null)
                        result = "Нет задач, что можно было бы изменить";
                    else
                        result = "Выберете Задачу, что хотите изменить:";
                    await Api.SendMessageAsync(callbackQuery.Message!.Chat.Id, result, parseMode: ParseMode.HTML, cancellationToken: cancellationToken, replyMarkup: markup);
                    break;

                #endregion

                #region Работа с комментариями

                case "AddCommentChoiseProblem":
                    markup = await GetIKWithProblemsAsync(callbackQuery.From!.Id, "AddComment");
                    await Api.EditMessageTextAsync<Message>(callbackQuery.Message!.Chat.Id, callbackQuery.Message.MessageId,
                        "Выберете задачу к которой добавить комментарий", replyMarkup: markup);
                    break;

                case "AddComment":
                    result = await CreateNewAnswer(callbackQuery, args[1]);
                    await ClearIK(callbackQuery);
                    await Api.SendMessageAsync(callbackQuery.Message!.Chat.Id, result);
                    await SendMessages(Int32.Parse(args[1]), ProblemModifications.Новый_комментарий, null);
                    break;

                #endregion

                #region Работа с модификацией проблемы (задачи)

                case "GetModifiedProblem":
                    result = await GetProblemInformation(callbackQuery.From!.Id, args[1]);
                    var getModifiedmarkup = GetIKModifiedProblem(args[1]);
                    await Api.EditMessageTextAsync<Message>(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId,
                        result, parseMode: ParseMode.HTML, replyMarkup: getModifiedmarkup);
                    break;

                case "SetModifiedProblem":
                    await ProblemUpdate(callbackQuery, args[1], args[2]);
                    break;

                case "SetPriority":
                    result = await ChangePriorityProblem(callbackQuery.From!.Id, args[1], args[2]);
                    await ClearIK(callbackQuery);
                    Api.SendMessage(callbackQuery.Message!.Chat.Id, result);
                    await SendMessages(Int32.Parse(args[1]), ProblemModifications.Изменить_приоритет, null);
                    break;

                #endregion

                #region Работа с новыми пользователями
                case "Register":
                    result = await _service.CreateTelegramUser(callbackQuery.From!);
                    Api.EditMessageText<Message>(callbackQuery.Message!.Chat!.Id, callbackQuery.Message.MessageId, result);
                    break;
                #endregion

                #region Настройки

                case "Settings":
                    markup = GetIKSettings();
                    result = string.Format("📝<b>---Выберите настройку---</b>📝");
                    await Api.SendMessageAsync(callbackQuery.Message!.Chat.Id, result, parseMode: ParseMode.HTML, replyMarkup: markup, cancellationToken: cancellationToken);
                    break;

                #endregion

                #region Работа с уведомлениями

                case "Notification":
                    user = await _service.GetUserTelegramByTelegramId(callbackQuery.From!.Id);
                    var choose = args.Count() > 1 ? args[1] : "DayOfWeek";

                    switch(choose)
                    {
                        case "DayOfWeek":
                            markup = GetIKChooseNotification(choose, user!.NotificationDays ?? "");
                            break;
                        case "HourOnDay":
                            markup = GetIKChooseNotification(choose, user!.NotificationHours ?? "");
                            break;
                    }
                    
                    Api.EditMessageText<Message>(callbackQuery.Message!.Chat.Id, callbackQuery.Message.MessageId, "Настройка уведомлений", replyMarkup: markup);
                    break;

                case "NotificationChange":
                    user = await _service.GetUserTelegramByTelegramId(callbackQuery.From!.Id);

                    switch (args[1])
                    {
                        case "DayOfWeek":
                            var day = Enum.Parse<DayOfWeekRus>(args[2]);
                            await _service.ChangeUserDayNotification(callbackQuery.From!.Id, day);
                            markup = GetIKChooseNotification("DayOfWeek", user!.NotificationDays ?? "");
                            break;
                        case "HourOnDay":
                            var hour = args[2];
                            await _service.ChangeUserTimeNotification(callbackQuery.From!.Id, hour);
                            markup = GetIKChooseNotification("HourOnDay", user!.NotificationHours ?? "");
                            break;
                    }

                    Api.EditMessageText<Message>(callbackQuery.Message!.Chat.Id, callbackQuery.Message.MessageId, "Настройка уведомлений, изменена", replyMarkup: markup);
                    break;

                #endregion

                #region Простые методы завершения

                case "Nothing":
                    var answer = "Успешно пропущено";
                    await ClearIK(callbackQuery);
                    if (args.Count() > 1)
                        answer = args[1];
                    await Api.SendMessageAsync(callbackQuery.Message!.Chat.Id, answer);
                    break;

                default:
                    break;

                #endregion
            }
            await base.OnCallbackQueryAsync(callbackQuery, cancellationToken);
        }
    }
}
