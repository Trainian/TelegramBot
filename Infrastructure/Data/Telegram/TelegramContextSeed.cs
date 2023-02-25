using ApplicationCore.Entities.Telegram;
using ApplicationCore.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Telegram
{
    public static class TelegramContextSeed
    {
        public static async Task SeedAsync(TelegramContext webContext)
        {
            if (webContext.Users.Count() == 0)
            {
                var Bot = new TelegramUser(1, "1CBot", "1CBot", Positions.СуперАдмин);

                webContext.Set<TelegramUser>().Add(Bot);

                await webContext.SaveChangesAsync();
            }
        }
    }
}
