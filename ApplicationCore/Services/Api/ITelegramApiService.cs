using ApplicationCore.Entities.Telegram;
using Telegram.BotAPI.AvailableTypes;

namespace ApplicationCore.Services.Api
{
    public interface ITelegramApiService
    {
        Task<string> CreateTelegramUser(User user);
        Task<TelegramUser?> GetUserTelegramByTelegramId(long telegramId);
        Task<List<TelegramUser>> GetListUserTelegramByPositionAsync(PositionEnum position);
        Task<IEnumerable<Problem>> GetPerformedProblemsByTelegramIdAsync(long telegramId);
        Task<Problem> GetPerformedProblemByTelegramIdAsync(long telegramId, int problemId);
        Task<IEnumerable<Problem>> GetDeliveredProblemsByTelegramIdAsync(long telegramId);
        Task<Problem> GetDeliveredProblemByTelegramIdAsync(long telegramId, int problemId);
        Task<IEnumerable<Problem>> GetAllProblemsByTelegramIdAsync(long telegramId);
        Task<Problem> GetProblemByProblemId(long telegramId, int problemId);

        /// <summary>
        /// Создание нового экзмепляра Задачи(проблемы)
        /// </summary>
        /// <param name="text">Текст задачи</param>
        /// <param name="user">Пользователь Telegram</param>
        /// <param name="imgPath">Ссылка на изображение, если есть</param>
        /// <param name="userGet">Пользователь, которому ставится задача, если есть</param>
        /// <returns></returns>
        Task<Problem> AddProblemAsync(string text, long telegramId, string? imgPath = null, string? userGet = null);
        Task<string> AddProblemComment(int problemId, string text, long telegramId, string? imgPath = null);
        Task<string> UpdateProblemAsync(Problem problem);
        Task<string> DeleteProblemByIdAsync(Problem problem);
        Task<int> CountProblemsAsync(long telegramId);
        Task<string> ChangePositionByTelegramUserIdAsync(long telegramUserId, PositionEnum position);
    }
}
