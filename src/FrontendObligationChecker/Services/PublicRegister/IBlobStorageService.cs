namespace FrontendObligationChecker.Services.PublicRegister;

using FrontendObligationChecker.Models.BlobReader;

public interface IBlobStorageService
{
    Task<PublicRegisterBlobModel?> GetLatestProducersFilePropertiesAsync();
}