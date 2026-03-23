namespace OutsideInTests.Infrastructure;

using PageModels;

public abstract class IntegrationTestBase : IAsyncLifetime
{
    private ObligationCheckerWebApplicationFactory Factory { get; set; } = null!;
    protected HttpClient Client { get; private set; } = null!;
    protected StubBlobStorageService BlobStorage => Factory.BlobStorage;
    protected StubLargeProducerRegisterService LargeProducerRegister => Factory.LargeProducerRegister;

    public virtual Task InitializeAsync()
    {
        Factory = new ObligationCheckerWebApplicationFactory();
        Client = Factory.CreateClient();
        BlobStorage.Reset();
        LargeProducerRegister.Reset();
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        Client.Dispose();
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
