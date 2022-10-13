using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Identity
{
    public static class IdentityContextSeed
    {
        public static async Task SeedAsync(IdentityContext identityWebContext, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            
        }
    }
}
