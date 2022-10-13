using ApplicationCore.Entities.Telegram;
using AutoMapper;
using Web.ViewModels;

namespace Web.AutoMapper
{
    public class MapperSettings : Profile
    {
        public MapperSettings()
        {
            CreateMap<Answer, AnswerViewModel>().ReverseMap();
            CreateMap<Problem, ProblemViewModel>().ReverseMap();
            CreateMap<TelegramUser, UserViewModel>().ReverseMap();
        }
    }
}
