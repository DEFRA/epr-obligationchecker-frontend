namespace FrontendObligationChecker.ConfigurationExtensions;

using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using Azure.Storage.Blobs;
using FrontendObligationChecker.Services.PublicRegister;
using FrontendObligationChecker.Services.Session;
using FrontendObligationChecker.Services.Session.Interfaces;
using Microsoft.Extensions.Options;
using Models.Config;
using Readers;
using Services.Caching;
using Services.Infrastructure;
using Services.Infrastructure.Interfaces;
using Services.LargeProducerRegister;
using Services.LargeProducerRegister.Interfaces;
using Services.PageService;
using Services.PageService.Interfaces;
using Services.Wrappers;
using Services.Wrappers.Interfaces;

[ExcludeFromCodeCoverage]
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
        services.AddScoped<IBlobReader, BlobReader>();
        services.AddScoped<ILargeProducerRegisterService, LargeProducerRegisterService>();
        services.AddScoped<ICacheService, CacheService>();
        services.AddSingleton(x =>
        {
            var storageAccountOptions = x.GetService<IOptions<StorageAccountOptions>>().Value;
            var blobContainerClient = new BlobContainerClient(
                storageAccountOptions.ConnectionString,
                storageAccountOptions.BlobContainerName);
            return blobContainerClient;
        });

        services.AddSingleton(x =>
        {
            var storageAccountOptions = x.GetService<IOptions<StorageAccountOptions>>().Value;
            return new BlobServiceClient(storageAccountOptions.ConnectionString);
        });

        services.AddScoped<IBlobStorageService, BlobStorageService>();
        services.AddAutoMapper(typeof(Program).Assembly);
        services.AddHealthChecks();
        return services;
    }
}