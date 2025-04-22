namespace FrontendObligationChecker.Services.PublicRegister;

using FrontendObligationChecker.Models.BlobReader;

public interface IBlobStorageService
{
    Task<PublicRegisterBlobModel?> GetLatestFilePropertiesAsync(string containerName);
    Task<Stream?> GetLatestFileAsync(string containerName);
}