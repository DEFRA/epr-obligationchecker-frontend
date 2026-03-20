namespace OutsideInTests.Infrastructure;

using FrontendObligationChecker;
using FrontendObligationChecker.Services.LargeProducerRegister.Interfaces;
using FrontendObligationChecker.Services.PublicRegister;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public class ObligationCheckerWebApplicationFactory : WebApplicationFactory<Program>
{
    public StubBlobStorageService BlobStorage { get; } = new();

    public StubLargeProducerRegisterService LargeProducerRegister { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                // Enable feature flags for the endpoints under test
                ["FeatureManagement:PublicRegisterEnabled"] = "true",
                ["FeatureManagement:LargeProducerRegisterEnabled"] = "true",
                ["FeatureManagement:PublicRegisterNextYearEnabled"] = "true",

                // Fake the clock to a date after Nov 1 so next year file logic activates
                // (production config PublicRegisterNextYearStartMonthAndDay is "11-01")
                ["PublicRegister:FakeDateTimeUtcNow"] = "2025-12-08",

                // Azurite emulator connection string — not a real secret, just satisfies DI wiring.
                ["StorageAccount:ConnectionString"] = "UseDevelopmentStorage=true",
                ["StorageAccount:BlobContainerName"] = "test-container",
            });
        });

        Environment.SetEnvironmentVariable("ByPassSessionValidation", "true");

        builder.ConfigureTestServices(services =>
        {
            // Replace blob storage services with controllable stubs
            services.AddSingleton<IBlobStorageService>(BlobStorage);
            services.AddSingleton<ILargeProducerRegisterService>(LargeProducerRegister);

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
