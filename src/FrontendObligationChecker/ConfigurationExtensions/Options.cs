namespace FrontendObligationChecker.ConfigurationExtensions;

using System.Diagnostics.CodeAnalysis;
using Models.Config;

[ExcludeFromCodeCoverage]
public static class Options
{
    public static IServiceCollection ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<EprCookieOptions>(configuration.GetSection(EprCookieOptions.ConfigSection));
        services.Configure<AnalyticsOptions>(configuration.GetSection(AnalyticsOptions.ConfigSection));
        services.Configure<PhaseBannerOptions>(configuration.GetSection(PhaseBannerOptions.ConfigSection));
        services.Configure<ExternalUrlsOptions>(configuration.GetSection(ExternalUrlsOptions.ConfigSection));
        services.Configure<EmailAddressOptions>(configuration.GetSection(EmailAddressOptions.ConfigSection));
        services.Configure<SiteDateOptions>(configuration.GetSection(SiteDateOptions.ConfigSection));
        services.Configure<StorageAccountOptions>(configuration.GetSection(StorageAccountOptions.ConfigSection));
        services.Configure<LargeProducerReportFileNamesOptions>(configuration.GetSection(LargeProducerReportFileNamesOptions.ConfigSection));
        services.Configure<CachingOptions>(configuration.GetSection(CachingOptions.ConfigSection));
        services.Configure<PublicRegisterOptions>(configuration.GetSection(PublicRegisterOptions.ConfigSection));

        return services;
    }
}