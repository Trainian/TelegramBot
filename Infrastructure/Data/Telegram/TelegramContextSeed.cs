using ApplicationCore.Entities.Telegram;
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
            //if (webContext.Users.Count() == 0)
            //{
            //    var testUser = new TelegramUser(1, "Test", "TelegramTest", PositionEnum.ТехСпециалист);
            //    var trainianUser = new TelegramUser(366459395, "Trainian", "Trainian_Z_V", PositionEnum.ТехСпециалист);

            //    webContext.Set<TelegramUser>().Add(testUser);
            //    webContext.Set<TelegramUser>().Add(trainianUser);

            //    await webContext.SaveChangesAsync();
            //}
        }
    }
}
