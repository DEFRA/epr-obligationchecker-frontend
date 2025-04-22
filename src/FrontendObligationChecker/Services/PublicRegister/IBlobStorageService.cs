namespace FrontendObligationChecker.Services.PublicRegister;

using FrontendObligationChecker.Models.BlobReader;

public interface IBlobStorageService
{
    Task<PublicRegisterBlobModel?> GetLatestFilePropertiesAsync(string containerName);
    Task<PublicRegisterBlobModel?> GetLatestFileAsync(string containerName);
}