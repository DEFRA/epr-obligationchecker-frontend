namespace OutsideInTests.Infrastructure;

using FrontendObligationChecker.Services.LargeProducerRegister.Interfaces;
using FrontendObligationChecker.ViewModels.LargeProducer;

/// <summary>
/// Controllable stub replacing real blob-backed large producer register service.
/// Tests configure responses via the public properties before making requests.
/// </summary>
public class StubLargeProducerRegisterService : ILargeProducerRegisterService
{
    public List<LargeProducerFileInfoViewModel> FileInfoList { get; set; } = [];

    public LargeProducerFileViewModel? FileToReturn { get; set; }

    public Dictionary<string, string> FileSizes { get; set; } = new();

    public Task<IEnumerable<LargeProducerFileInfoViewModel>> GetLatestAllNationsFileInfoAsync(string culture)
    {
        return Task.FromResult<IEnumerable<LargeProducerFileInfoViewModel>>(FileInfoList);
    }

    public Task<LargeProducerFileViewModel?> GetLatestAllNationsFileAsync(int reportingYear, string culture)
    {
        return Task.FromResult(FileToReturn);
    }

    public Task<(Stream Stream, string FileName)> GetReportAsync(string nationCode, string culture)
    {
        return Task.FromResult<(Stream, string)>((new MemoryStream(), "stub.csv"));
    }

    public Task<Dictionary<string, string>> GetAllReportFileSizesAsync(string culture)
    {
        return Task.FromResult(FileSizes);
    }

    public void Reset()
    {
        FileInfoList = [];
        FileToReturn = null;
        FileSizes = new();
    }
}
