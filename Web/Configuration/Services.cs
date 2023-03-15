using ApplicationCore.Repositories.Base;
using ApplicationCore.Repositories.Telegram;
using Infrastructure.Repositories.Base;
using Infrastructure.Repositories.Telegram;
using Infrastructure.Services.Telegram;
using Infrastructure.Settings;
using Infrastructure.Services.Api;
using ApplicationCore.Services.Api;
using Web.AutoMapper;
using Web.Interfaces.Telegram;
using Web.Services.Telegram;
using Web.Services.Timers;
using Web.Interfaces.Timers;

namespace Web.Configuration
{
    public static class Services
    {
        public static IServiceCollection SetServices(this IServiceCollection services)
        {
            services.AddSingleton(typeof(TelegramBotSettings));
            services.AddScoped(typeof(TelegramBotService));
            services.AddScoped(typeof(IRepository<>),typeof(Repository<>));
            services.AddScoped(typeof(IAnswerRepository), typeof(AnswerRepository));
            services.AddScoped(typeof(IProblemRepository), typeof(ProblemRepository));
            services.AddScoped(typeof(IUserRepository), typeof(UserRepository));
            services.AddScoped(typeof(ITelegramApiService), typeof(TelegramApiService));
            services.AddScoped(typeof(IProblemService), typeof(ProblemService));
            services.AddScoped(typeof(IAnswerService), typeof(AnswerService));
            services.AddScoped(typeof(IUserService), typeof(UserService));
            services.AddScoped(typeof(IBot1CService), typeof(Bot1CService));
            services.AddAutoMapper(typeof(MapperSettings));

            services.AddHostedService<TimerService>();
            services.AddScoped<IScopedProcessingTimerService, ScopedProcessingTimerService>();
            return services;
        }
    }
}
