﻿using Infrastructure.Settings;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.InlineMode;
using Telegram.BotAPI.UpdatingMessages;
using Microsoft.Extensions.Logging;
using System.Net;
using ApplicationCore.Entities.Telegram;
using Telegram.BotAPI.AvailableMethods.FormattingOptions;
using ApplicationCore.Enums;

namespace Infrastructure.Services.Telegram
{
    public partial class TelegramBotService : AsyncTelegramBotBase<TelegramBotSettings>
    {
        protected override async Task OnCallbackQueryAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            var args = callbackQuery.Data.Split(' ');
            InlineKeyboardMarkup? markup;
            string result;

            switch (args[0])
            {
                #region Работа с новыми задачами

                case "NewTask":
                    markup = GetInlineKeyboardToChooseResponsible();
                    await Api.EditMessageTextAsync<Message>(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId,
                        "Куда ставим задачу?", replyMarkup: markup);
                    break;

                case "GetResponsiblePositions":
                    markup = GetInlineKeyboardToSetResponsibleByPosition();
                    await Api.EditMessageTextAsync<Message>(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId,
                        "Выберите отдел:", replyMarkup: markup);
                    break;
                    
                case "GetResponsibleUsers":
                    markup = await GetInlineKeyboardToSetResponsibleByTelegramUserAsync();
                    await Api.EditMessageTextAsync<Message>(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId,
                        "Выберите ответсвенного:", replyMarkup: markup);
                    break;

                case "SetPiority":
                    markup = GetInlineKeyboardToChoosePriority(Enum.Parse<ResponibleTypes>(args[1]), args[2]);
                    await Api.EditMessageTextAsync<Message>(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId,
                        "Выберите приоритет:", replyMarkup: markup);
                    break;
                    
                case "CreateTask":
                    result = await CreateNewTask(callbackQuery, Enum.Parse<ResponibleTypes>(args[1]), args[2], Enum.Parse<Prioritys>(args[3]));
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
                    var inlineKeyboard = await GetInlineKeyboardWithProblemsAsync(callbackQuery.From!.Id, "AddComment");
                    await Api.EditMessageTextAsync<Message>(callbackQuery.Message.Chat.Id, callbackQuery.Message.MessageId,
                        "Выберете задачу к которой добавить комментарий", replyMarkup: inlineKeyboard);
                    break;

                case "AddComment":
                    result = await CreateNewAnswer(callbackQuery, args[1]);
                    await ClearInlineKeyboard(callbackQuery);
                    await Api.SendMessageAsync(callbackQuery.Message!.Chat.Id, result);
                    await SendMessages(Int32.Parse(args[1]), ProblemModifications.Новый_комментарий, null);
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
                    await SendMessages(Int32.Parse(args[1]), ProblemModifications.Изменить_приоритет, null);
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
    }
}