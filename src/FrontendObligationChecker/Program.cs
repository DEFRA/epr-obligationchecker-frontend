using FrontendObligationChecker;
using FrontendObligationChecker.ConfigurationExtensions;
using FrontendObligationChecker.HealthChecks;
using FrontendObligationChecker.Middleware;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Azure;
using Microsoft.FeatureManagement;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFeatureManagement();

builder.Services.ConfigureOptions(builder.Configuration);

string pathBase = builder.Configuration.GetValue<string>("PATH_BASE");

builder.Services.AddMemoryCache();

builder.Services
    .AddControllersWithViews()
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

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedHost | ForwardedHeaders.XForwardedProto;
    options.ForwardedHostHeaderName = builder.Configuration.GetValue<string>("ForwardedHeaders:ForwardedHostHeaderName");
    options.OriginalHostHeaderName = builder.Configuration.GetValue<string>("ForwardedHeaders:OriginalHostHeaderName");
    options.AllowedHosts = builder.Configuration.GetValue<string>("ForwardedHeaders:AllowedHosts").Split(";");
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
}

app.UseSession();

app.UseForwardedHeaders();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseRequestLocalization();
app.UseMiddleware<AnalyticsCookieMiddleware>();
app.MapHealthChecks(builder.Configuration.GetValue<string>("HEALTH_CHECK_LIVENESS_PATH"), HealthCheckOptionBuilder.Build());
app.MapControllers();

app.Run();

public partial class Program
{
}