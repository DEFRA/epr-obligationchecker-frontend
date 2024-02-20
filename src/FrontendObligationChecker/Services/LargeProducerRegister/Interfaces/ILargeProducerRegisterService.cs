namespace FrontendObligationChecker.Services.LargeProducerRegister.Interfaces;

using ByteSizeLib;
using Models;

public interface ILargeProducerRegisterService
{
    Task<(Stream Stream, string FileName)> GetReportAsync(string nationCode, string culture);

    Task<Dictionary<string, string>> GetAllReportFileSizesAsync(string culture);
}