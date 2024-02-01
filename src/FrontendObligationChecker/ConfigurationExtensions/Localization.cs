namespace FrontendObligationChecker.ConfigurationExtensions;

using Constants;
using Microsoft.AspNetCore.Localization;

public static class Localization
{
    public static IServiceCollection ConfigureLocalization(this IServiceCollection services)
    {
        services.AddLocalization(options => options.ResourcesPath = "Resources")
            .Configure<RequestLocalizationOptions>(options =>
            {
                var cultureList = new[] { Language.English, Language.Welsh };
                options.SetDefaultCulture(Language.English);
                options.AddSupportedCultures(cultureList);
                options.AddSupportedUICultures(cultureList);
                options.RequestCultureProviders = new IRequestCultureProvider[]
                {
                    new QueryStringRequestCultureProvider()
                };
            });
        return services;
    }
}