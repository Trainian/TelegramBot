using ApplicationCore.Entities.Telegram;
using ApplicationCore.Enums;
using ApplicationCore.Models;

namespace Web.Interfaces.Telegram
{
    public interface IBot1CService
    {
        Task<Problem?> AddProblemAsync(Element1CToGetError error);
        Task SendMessagesByPositionAsync(Positions position, Problem problem, bool upperPosition = false);
    }
}
