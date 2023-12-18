using System.IO.Abstractions;

using FrontendObligationChecker.Services.Infrastructure;
using FrontendObligationChecker.Services.Infrastructure.Interfaces;
using FrontendObligationChecker.Services.PageService;
using FrontendObligationChecker.Services.PageService.Interfaces;
using FrontendObligationChecker.Services.Session;
using FrontendObligationChecker.Services.Session.Interfaces;
using FrontendObligationChecker.Services.Wrappers;
using FrontendObligationChecker.Services.Wrappers.Interfaces;

namespace FrontendObligationChecker.ConfigurationExtensions;
public static class DependencyInjection
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddSingleton<IFileSystem, FileSystem>();
        services.AddSingleton<IDateTimeWrapper, DateTimeWrapper>();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSingleton<ICookieService, CookieService>();
        services.AddScoped(typeof(IDistributedSession<>), typeof(DistributedSession<>));
        services.AddScoped<IJourneySession, JourneySession>();
        services.AddScoped<IPageService, PageService>();
        services.AddAutoMapper(typeof(Program).Assembly);
        services.AddHealthChecks();
        return services;
    }
}