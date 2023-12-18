using Newtonsoft.Json;

namespace FrontendObligationChecker.Services.Wrappers.Interfaces;
public interface IJsonConvertWrapper
{
    T DeserializeObject<T>(string value, JsonSerializerSettings settings);

    string SerializeObject(object value, Formatting formatting, JsonSerializerSettings settings);
}