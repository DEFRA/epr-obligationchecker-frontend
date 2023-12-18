using System.Text.Json;

namespace FrontendObligationChecker.Extensions;
public static class SessionExtensions
{
    public static async Task SetAsync<T>(this ISession session, string key, T value)
    {
        await session.LoadAsync();
        session.SetString(key, JsonSerializer.Serialize(value));
    }

    public static async Task<T?> GetAsync<T>(this ISession session, string key)
    {
        await session.LoadAsync();
        var value = session.GetString(key);
        return value == null ? default : JsonSerializer.Deserialize<T>(value);
    }
}