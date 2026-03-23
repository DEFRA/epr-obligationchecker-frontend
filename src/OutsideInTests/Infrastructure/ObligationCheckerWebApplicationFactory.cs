namespace OutsideInTests.Infrastructure;

using FrontendObligationChecker;
using FrontendObligationChecker.Readers;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public class ObligationCheckerWebApplicationFactory : WebApplicationFactory<Program>
{
    public StubBlobReader BlobReader { get; } = new();

    /// <summary>
    /// Additional config overrides merged last (highest priority).
    /// Set before calling CreateClient().
    /// </summary>
    public Dictionary<string, string?> ConfigOverrides { get; set; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Infrastructure defaults only - test-relevant config goes in ConfigOverrides
        var config = new Dictionary<string, string?>
        {
            ["FeatureManagement:PublicRegisterEnabled"] = "true",
            ["FeatureManagement:LargeProducerRegisterEnabled"] = "true",

            // Azurite emulator connection string — not a real secret, just satisfies DI wiring.
            ["StorageAccount:ConnectionString"] = "UseDevelopmentStorage=true",
            ["StorageAccount:BlobContainerName"] = "test-container",
        };

        foreach (var kvp in ConfigOverrides)
        {
            config[kvp.Key] = kvp.Value;
        }

        builder.ConfigureAppConfiguration((_, cfg) =>
        {
            cfg.AddInMemoryCollection(config);
        });

        Environment.SetEnvironmentVariable("ByPassSessionValidation", "true");

        builder.ConfigureTestServices(services =>
        {
            // Replace blob reader with controllable stub — both BlobStorageService and
            // LargeProducerRegisterService use IBlobReader, so their real logic is exercised.
            services.AddSingleton<IBlobReader>(BlobReader);

            services.AddAntiforgery(options =>
            {
                options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.None;
            });

            services.Configure<Microsoft.AspNetCore.Builder.SessionOptions>(options =>
            {
                options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.None;
            });
        });
    }
}
