namespace FrontendObligationChecker.Services.LargeProducerRegister.Interfaces;

using FrontendObligationChecker.ViewModels.LargeProducer;

public interface ILargeProducerRegisterService
{
    Task<(Stream Stream, string FileName)> GetReportAsync(string nationCode, string culture);

    Task<Dictionary<string, string>> GetAllReportFileSizesAsync(string culture);

    Task<IEnumerable<LargeProducerFileInfoViewModel>> GetLatestAllNationsFileInfoAsync(string culture);

    Task<LargeProducerFileViewModel> GetLatestAllNationsFileAsync(int reportingYear, string culture);
}