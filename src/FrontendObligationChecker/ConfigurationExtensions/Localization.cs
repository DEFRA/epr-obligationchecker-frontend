using Microsoft.AspNetCore.Localization;

namespace FrontendObligationChecker.ConfigurationExtensions;
public static class Localization
{
    private const string English = "en";

    public static IServiceCollection ConfigureLocalization(this IServiceCollection services)
    {
        services.AddLocalization(options => options.ResourcesPath = "Resources")
            .Configure<RequestLocalizationOptions>(options =>
            {
                var cultureList = new[] { English };
                options.SetDefaultCulture(English);
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