namespace FrontendObligationChecker.Services.PublicRegister;

using FrontendObligationChecker.Models.BlobReader;
using FrontendObligationChecker.ViewModels.PublicRegister;

public interface IBlobStorageService
{
    Task<PublicRegisterBlobModel?> GetLatestFilePropertiesAsync(string containerName);

    Task<EnforcementActionFileViewModel> GetEnforcementActionFileByAgency(string agency);

    Task<IEnumerable<EnforcementActionFileViewModel>> GetEnforcementActionFiles();

    Task<Stream?> GetLatestFileAsync(string containerName);
}