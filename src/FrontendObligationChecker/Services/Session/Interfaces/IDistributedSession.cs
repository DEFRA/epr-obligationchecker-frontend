namespace FrontendObligationChecker.Services.Session.Interfaces;

public interface IDistributedSession<T>
{
    Task<T> GetAsync(string key);

    Task SetAsync(string key, T data);

    void Remove(string key);
}