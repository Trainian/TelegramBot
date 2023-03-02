using ApplicationCore.Entities.Telegram;
using ApplicationCore.Enums;
using ApplicationCore.Models;
using Telegram.BotAPI.AvailableTypes;
using Types = Telegram.BotAPI.AvailableTypes;

namespace ApplicationCore.Services.Api
{
    public interface ITelegramApiService
    {
        Task<string> CreateTelegramUser(User user);
        Task<TelegramUser?> GetUserTelegramByTelegramId(long telegramId);
        Task<List<TelegramUser>> GetListUserTelegramByPositionAsync(Positions position = Positions.ТехСпециалист, bool andHigher = false);
        Task<IEnumerable<Problem>> GetPerformedProblemsByTelegramIdAsync(long telegramId);
        Task<Problem> GetPerformedProblemByTelegramIdAsync(long telegramId, int problemId);
        Task<IEnumerable<Problem>> GetDeliveredProblemsByTelegramIdAsync(long telegramId);
        Task<Problem> GetDeliveredProblemByTelegramIdAsync(long telegramId, int problemId);
        Task<IEnumerable<Problem>> GetAllProblemsByTelegramIdAsync(long telegramId);
        Task<IEnumerable<Problem>> GetAllProblemsWithoutResponsible();
        Task<Problem> GetProblemByProblemIdAsync(int problemId);
        Task<Problem> AddProblemAsync(string text, long telegramId, ResponibleTypes responibleType, string whoGet, Prioritys priority, string? imgPath = null, Types.File? document = null);
        Task<string> AddProblemComment(int problemId, string text, long telegramId, string? imgPath = null);
        Task<string> UpdateProblemAsync(Problem problem);
        Task<string> DeleteProblemByIdAsync(Problem problem);
        Task<int> CountProblemsAsync(long telegramId);
        Task<string> ChangePositionByTelegramUserIdAsync(long telegramUserId, Positions position);
        Task ChangeUserDayNotification(long telegramId, DayOfWeekRus day);
        Task ChangeUserTimeNotification(long telegramId, string time);
        Task<IEnumerable<TelegramUser>> GetListUsersToNeedNotification();
    }
}
