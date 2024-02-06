namespace FrontendObligationChecker.Services.LargeProducerRegister.Interfaces;

public interface ILargeProducerRegisterService
{
    Task<(Stream Stream, string FileName)> GetReportAsync(string nationCode);
}