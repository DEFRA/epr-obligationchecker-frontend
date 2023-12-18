using FrontendObligationChecker.Extensions;
using FrontendObligationChecker.Services.Session.Interfaces;

namespace FrontendObligationChecker.Services.Session;
public class DistributedSession<T> : IDistributedSession<T>
{
    private readonly IHttpContextAccessor _contextAccessor;

    public DistributedSession(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
    }

    public async Task<T> GetAsync(string key)
    {
        return (await _contextAccessor.HttpContext!.Session.GetAsync<T>(key))!;
    }

    public async Task SetAsync(string key, T data)
    {
        await _contextAccessor.HttpContext!.Session.SetAsync<T>(key, data);
    }

    public void Remove(string key)
    {
        _contextAccessor.HttpContext!.Session.Remove(key);
    }
}