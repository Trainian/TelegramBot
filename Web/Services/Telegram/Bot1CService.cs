using ApplicationCore.Entities.Telegram;
using ApplicationCore.Enums;
using ApplicationCore.Models;
using ApplicationCore.Repositories.Telegram;
using ApplicationCore.Services.Api;
using Ardalis.Result;
using Infrastructure.Services.Telegram;
using NuGet.Common;
using System.Text;
using Web.Interfaces.Telegram;

namespace Web.Services.Telegram
{
    public class Bot1CService : IBot1CService
    {
        private readonly ITelegramApiService _apiService;
        private readonly IUserRepository _userRepository;
        private readonly TelegramBotService _botService;

        public Bot1CService(ITelegramApiService apiService, IUserRepository userRepository, TelegramBotService botService)
        {
            _apiService = apiService;
            _userRepository = userRepository;
            _botService = botService;
        }
        public async Task<Problem?> AddProblemAsync(Element1CToGetError error)
        {
            var stringError = $"\nID Лицензии: {error.WarningInfo.LicenseId},\n"+
                $"Организация: {error.WarningInfo.Organization},\n"+
                $"Тема: {error.WarningInfo.Topic},\n"+
                $"Содержание: {error.WarningInfo.Content}.";

            Prioritys priority = Prioritys.Низкий;

            WarningLevels warningLvl = WarningLevels.Сообщение;
            Enum.TryParse<WarningLevels>(error.WarningLevel, true, out warningLvl);
            switch(warningLvl)
            {
                case WarningLevels.Сообщение:
                    priority = Prioritys.Низкий;
                    break;

                case WarningLevels.Предупреждение:
                    priority = Prioritys.Средний;
                    break;

                case WarningLevels.Ошибка:
                    priority = Prioritys.Высокий;
                    break;
            }

            var executor = await _userRepository.GetByNameAsync(error.WarningInfo.Executor!);

            var problem = await _apiService.AddProblemAsync(stringError, 000000000, ResponibleTypes.Пользователь ,executor?.Id.ToString() ?? "000000000", priority: priority);
            return problem;
        }

        public async Task SendMessagesByPositionAsync(Positions position, Problem problem, bool upperPosition = false)
        {
            var message = GetMessage(problem);
            IReadOnlyList<TelegramUser> users;
            if(upperPosition)
                users = await _userRepository.GetAsync(u => u.Position >= position);
            else
                users = await _userRepository.GetAsync(u => u.Position == position);
            foreach(var user in users)
            {
                _botService.SendMessage(user!.TelegramId, message);
            }
        }

        private string GetMessage(Problem problem)
        {
            var message = $"📝<b>---Сообщение о проблеме из 1С---</b>📝\n\n"+
                $"Id задачи(ошибки): <b>{problem.Id}</b>\n"+
                $"Текст: <b>{problem.Text}</b>\nПриоритет: <b>{problem.Priority}</b> \\ Создан: <b>{problem.CreateDateTime.ToShortDateString()}</b>\n" +
                $"Поставил: <b>{problem.UserCreateProblem!.Name}</b> \\ Выполняет: <b>{problem.UserGetProblem?.Name ?? "-"}</b>";
            return message;
        }
    }
}
