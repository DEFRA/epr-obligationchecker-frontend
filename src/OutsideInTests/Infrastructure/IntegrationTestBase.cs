namespace OutsideInTests.Infrastructure;

using PageModels;

public abstract class IntegrationTestBase : IAsyncLifetime
{
    private ObligationCheckerWebApplicationFactory Factory { get; set; } = null!;
    private HttpClient? _client;

    /// <summary>
    /// Lazy - first access starts the server, so set ConfigOverrides before this.
    /// </summary>
    protected HttpClient Client => _client ??= Factory.CreateClient();

    protected StubBlobStorageService BlobStorage => Factory.BlobStorage;
    protected StubLargeProducerRegisterService LargeProducerRegister => Factory.LargeProducerRegister;

    /// <summary>
    /// Set config overrides before the first HTTP request.
    /// Keys use colon-separated config paths, e.g. "PublicRegister:FakeDateTimeUtcNow".
    /// </summary>
    protected Dictionary<string, string?> ConfigOverrides => Factory.ConfigOverrides;

    public virtual Task InitializeAsync()
    {
        Factory = new ObligationCheckerWebApplicationFactory();
        BlobStorage.Reset();
        LargeProducerRegister.Reset();
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _client?.Dispose();
        await Factory.DisposeAsync();
    }

    protected async Task<TPageModel> GetAsPageModel<TPageModel>(string? requestUri)
        where TPageModel : PageModelBase, IPageModelFactory<TPageModel>
    {
        var response = await Client.GetAsync(requestUri);
        response.EnsureSuccessStatusCode();
        var htmlContent = await response.Content.ReadAsStringAsync();
        return TPageModel.FromContent(htmlContent);
    }
}
