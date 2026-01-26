using FrontendObligationChecker.ConfigurationExtensions;
using FrontendObligationChecker.FeatureManagement;
using FrontendObligationChecker.HealthChecks;
using FrontendObligationChecker.Middleware;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using Serilog;

namespace FrontendObligationChecker;

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Logging.ClearProviders();
        builder.Host.UseSerilog((context, _, config) =>
        {
            config.ReadFrom.Configuration(context.Configuration);
            config.Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName);
        });

        builder.Services.AddFeatureManagement().UseDisabledFeaturesHandler(new RedirectDisabledFeatureHandler());

        builder.Services.ConfigureOptions(builder.Configuration);

        string pathBase = builder.Configuration.GetValue<string>("PATH_BASE");

        builder.Services.AddAntiforgery(opts =>
        {
            opts.Cookie.Name = builder.Configuration.GetValue<string>("COOKIE_OPTIONS:AntiForgeryCookieName");
            opts.Cookie.Path = pathBase;
            opts.Cookie.HttpOnly = true;
            opts.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            opts.Cookie.SameSite = SameSiteMode.Strict;
        });

        builder.Services.AddMemoryCache();

        builder.Services
            .AddControllersWithViews(options => options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()))
            .AddViewLocalization()
            .AddDataAnnotationsLocalization();

        builder.Services
            .ConfigureLocalization()
            .ConfigureSession(builder.Configuration);

        builder.Services.AddApplicationInsightsTelemetry();

        builder.Services.AddHsts(options =>
        {
            options.IncludeSubDomains = true;
            options.MaxAge = TimeSpan.FromDays(365);
        });

        builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);

        builder.Services.RegisterServices();

        if (builder.Configuration.GetValue<string>("ByPassSessionValidation") != null)
        {
            GlobalData.ByPassSessionValidation = bool.Parse(builder.Configuration.GetValue<string>("ByPassSessionValidation"));
        }

        var app = builder.Build();

        app.UsePathBase(pathBase);

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/error");
        }

        app.UseMiddleware<SecurityHeaderMiddleware>();
        app.UseSession();

        // This must be put after security headers middleware to prevent executing it twice when error page is rendered
        app.UseStatusCodePagesWithReExecute("/error", "?statusCode={0}");

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseSerilogRequestLogging(); // after `UseStaticFiles()` to prevent logging of requests to css/js/png etc.
        app.UseRouting();
        app.UseAuthorization();
        app.UseRequestLocalization();
        app.UseMiddleware<AnalyticsCookieMiddleware>();
        app.MapHealthChecks(builder.Configuration.GetValue<string>("HEALTH_CHECK_LIVENESS_PATH"), HealthCheckOptionBuilder.Build());
        app.MapControllers();
        app.Run();
    }
}