using FrontendObligationChecker.Services.Session.Interfaces;

namespace FrontendObligationChecker.UnitTests.Helpers;

public class TestDistributedSession<T> : IDistributedSession<T>
{
    private readonly Dictionary<string, T> _session = new();

    public async Task<T> GetAsync(string key)
    {
        T result = default;

        if (_session.TryGetValue(key, out T? value))
        {
            result = value;
        }

        return await Task.FromResult(result);
    }

    public async Task SetAsync(string key, T data)
    {
        _session[key] = data;
    }

    public void Remove(string key)
    {
        _session.Remove(key);
    }
}