namespace FrontendObligationChecker.Services.Caching;

using System.Reflection;

public interface ICacheService
{
    bool GetReportFileSizeCache(string culture, out Dictionary<string, string> reportFileSizeMapping);

    void SetReportFileSizeCache(string culture, Dictionary<string, string> reportFileSizeMapping);
}