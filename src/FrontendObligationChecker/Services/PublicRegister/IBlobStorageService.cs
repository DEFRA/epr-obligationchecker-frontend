namespace FrontendObligationChecker.Services.PublicRegister;

using FrontendObligationChecker.Models.BlobReader;
using FrontendObligationChecker.ViewModels.PublicRegister;

public interface IBlobStorageService
{
    Task<PublicRegisterBlobModel?> GetLatestFilePropertiesAsync(string containerName);

    Task<EnforcementActionFileViewModel> GetEnforcementActionFileByHomeNation(string homeNation);

    Task<IEnumerable<EnforcementActionFileViewModel>> GetEnforcementActionFiles();
}