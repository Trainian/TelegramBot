using Infrastructure.Data;
using Infrastructure.Data.Identity;
using Infrastructure.Data.Telegram;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure
{
    public class Dependencies
    {
        public static void ConfigureData(IConfiguration configuration, IServiceCollection services, IHostEnvironment environment)
        {
            var db = bool.Parse(configuration["DatabaseInMemory"]);
            if (db)
            {
                services.AddDbContext<IdentityContext>(c => c.UseInMemoryDatabase("Identity"));
                services.AddDbContext<TelegramContext>(c => c.UseInMemoryDatabase("Telegram"));
            }
            else
            {
                services.AddDbContext<IdentityContext>(c => c.UseSqlServer(configuration.GetConnectionString("Identity")));
                services.AddDbContext<TelegramContext>(c => c.UseSqlServer(configuration.GetConnectionString("Telegram"),
                    o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));
            }
        }
    }
}