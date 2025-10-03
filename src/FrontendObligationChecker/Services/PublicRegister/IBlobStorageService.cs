namespace FrontendObligationChecker.Services.PublicRegister;

using FrontendObligationChecker.Models.BlobReader;
using FrontendObligationChecker.ViewModels.PublicRegister;

public interface IBlobStorageService
{
    Task<PublicRegisterBlobModel> GetLatestFilePropertiesAsync(string containerName);

    Task<Dictionary<string, PublicRegisterBlobModel>> GetLatestFilePropertiesAsync(string containerName, List<string> folderPrefixes);

    Task<EnforcementActionFileViewModel> GetEnforcementActionFileByAgency(string agency);

    Task<IEnumerable<EnforcementActionFileViewModel>> GetEnforcementActionFiles();

    Task<PublicRegisterFileModel> GetLatestFileAsync(string containerName);
}