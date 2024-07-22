namespace FrontendObligationChecker.ConfigurationExtensions;

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.DataProtection;
using StackExchange.Redis;

[ExcludeFromCodeCoverage]
public static class Session
{
    public static IServiceCollection ConfigureSession(this IServiceCollection services, IConfiguration configuration)
    {
        bool useLocalSession = configuration.GetValue<bool>("UseLocalSession");

        if (!useLocalSession)
        {
            var redisConnection = configuration.GetConnectionString("REDIS_CONNECTION");

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnection;
                options.InstanceName = configuration.GetValue<string>("REDIS_INSTANCE_NAME");
            });

            services
                .AddDataProtection()
                .PersistKeysToStackExchangeRedis(ConnectionMultiplexer.Connect(redisConnection), "DataProtection-Keys")
                .SetApplicationName("Obligation-checker")
                .SetDefaultKeyLifetime(TimeSpan.FromDays(90));
        }


        services.AddSession(options =>
        {
            options.Cookie.Name = configuration.GetValue<string>("COOKIE_OPTIONS:SessionCookieName");
            options.IdleTimeout = TimeSpan.FromMinutes(configuration.GetValue<int>("SESSION_IDLE_TIMEOUT_MINUTES"));
            options.Cookie.HttpOnly = true;
            options.Cookie.Path = configuration.GetValue<string>("PATH_BASE");
        });

        return services;
    }
}