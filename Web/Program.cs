using Ardalis.ListStartupServices;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Infrastructure;
using Infrastructure.Data.Identity;
using Infrastructure.Data.Telegram;
using Infrastructure.Services.Telegram;
using Infrastructure.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using Serilog;
using Web.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Host.UseSerilog((ctx, lx) => {
    lx.ReadFrom.Configuration(builder.Configuration);
    lx.WriteTo.Console();
    });

builder.Services.AddMemoryCache();
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

Dependencies.ConfigureData(builder.Configuration, builder.Services, builder.Environment);
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedEmail = false;
        options.Password.RequireDigit = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<IdentityContext>()
    .AddDefaultUI();

//builder.Services.ConfigureApplicationCookie(options =>
//{
    //options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    //options.Cookie.Name = "YourAppCookieName";
    //options.Cookie.HttpOnly = true;
    //options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    //options.LoginPath = "/Identity/Account/Login";
    //// ReturnUrlParameter requires 
    ////using Microsoft.AspNetCore.Authentication.Cookies;
    //options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
    //options.SlidingExpiration = true;
//});

//Option is  Disable Required not Nullable classes
builder.Services.AddControllersWithViews(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true).AddNewtonsoftJson();
builder.Services.AddRazorPages();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    c.EnableAnnotations();
});

builder.Services.SetServices();

// add list services for diagnostic purposes - see https://github.com/ardalis/AspNetCoreStartupServices
builder.Services.Configure<ServiceConfig>(config =>
{
    config.Services = new List<ServiceDescriptor>(builder.Services);

    // optional - default path to view services is /listallservices - recommended to choose your own path
    config.Path = "/listservices";
});


//builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
//{
//    containerBuilder.RegisterModule(new DefaultCoreModule());
//    containerBuilder.RegisterModule(new DefaultInfrastructureModule(builder.Environment.EnvironmentName == "Development"));
//});

//builder.Logging.AddAzureWebAppDiagnostics(); add this if deploying to Azure

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseShowAllServicesMiddleware();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthorization();
app.UseCookiePolicy();

app.UseSwagger();
app.UseSwaggerUI(options => {
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
});

app.MapRazorPages();
app.MapDefaultControllerRoute();

// Seed Database
using (var scope = app.Services.CreateScope())
{
    var scopeProvider = scope.ServiceProvider;

    try
    {
        var webContext = scopeProvider.GetRequiredService<TelegramContext>();
        await TelegramContextSeed.SeedAsync(webContext);

        var identityWebContext = scopeProvider.GetRequiredService<IdentityContext>();
        var userManager = scopeProvider.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = scopeProvider.GetRequiredService<RoleManager<IdentityRole>>();
        await IdentityContextSeed.SeedAsync(identityWebContext, userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Произошла ошибка, заполнения БД. {exceptionMessage}", ex.Message);
    }
}

app.Run();
