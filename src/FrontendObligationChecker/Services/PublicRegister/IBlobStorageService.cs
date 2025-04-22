namespace FrontendObligationChecker.Services.PublicRegister;

using Azure.Storage.Blobs.Models;
using FrontendObligationChecker.Models.BlobReader;

public interface IBlobStorageService
{
    Task<PublicRegisterBlobModel?> GetLatestFilePropertiesAsync(string containerName);
}