using ApplicationCore.Models;
using ApplicationCore.Services.Api;
using Ardalis.ListStartupServices;
using Ardalis.Result;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Infrastructure;
using Infrastructure.Data.Identity;
using Infrastructure.Data.Telegram;
using Infrastructure.Services.Telegram;
using Infrastructure.Settings;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Serilog;
using System.Text.Json.Serialization;
using Web.Configuration;
using Web.Interfaces.Telegram;
using Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Host.UseSerilog((ctx, lx) => {
    lx.ReadFrom.Configuration(builder.Configuration);
    lx.WriteTo.Console();
    });

//builder.Services.AddMemoryCache();
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

//Option is  Disable Required not Nullable classes
builder.Services.AddControllersWithViews(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true).AddNewtonsoftJson();
builder.Services.AddRazorPages();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "1С to API", Version = "v1" });
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

builder.Services.Configure<JsonSerializerSettings>(options =>
{
    options.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseShowAllServicesMiddleware();
    app.UseSwagger();
    app.UseSwaggerUI(options => {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    });
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

app.MapRazorPages();
app.MapDefaultControllerRoute();

app.MapPost("api/1c/problem", async ([FromHeader(Name = "1CToken")] string token, IBot1CService botService, Element1CToGetError elementError) =>
{
    if (token == app.Configuration["Telegram:1CToken"])
    {
        try
        {
            var problem = await botService.AddProblemAsync(elementError);

            if (problem == null)
                return Results.Problem("Ошибка добавления модели...");

            await botService.SendMessagesByPositionAsync(ApplicationCore.Enums.Positions.ТехСпециалист, problem, true);
            return Results.Ok();

        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    }

    return Results.Unauthorized();
});

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
