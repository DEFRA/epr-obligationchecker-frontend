namespace OutsideInTests.Infrastructure;

using FrontendObligationChecker.Models.BlobReader;
using FrontendObligationChecker.Services.PublicRegister;
using FrontendObligationChecker.ViewModels.PublicRegister;

/// <summary>
/// Controllable stub replacing real Azure Blob Storage access.
/// Tests configure responses via the public properties before making requests.
/// Analogous to WireMock stubs in the regulator service tests.
/// </summary>
public class StubBlobStorageService : IBlobStorageService
{
    public Dictionary<string, PublicRegisterBlobModel> ProducerBlobModels { get; set; } = new();

    public PublicRegisterBlobModel ComplianceBlobModel { get; set; } = new();

    public PublicRegisterFileModel FileToReturn { get; set; } = new();

    public List<EnforcementActionFileViewModel> EnforcementFiles { get; set; } = [];

    public EnforcementActionFileViewModel EnforcementFileByAgency { get; set; } = new();

    public Task<PublicRegisterBlobModel> GetLatestFilePropertiesAsync(string containerName)
    {
        return Task.FromResult(ComplianceBlobModel);
    }

    public Task<Dictionary<string, PublicRegisterBlobModel>> GetLatestFilePropertiesAsync(
        string containerName, List<string> folderPrefixes)
    {
        // Filter to only requested prefixes, matching real blob storage behaviour
        var filtered = ProducerBlobModels
            .Where(kvp => folderPrefixes.Contains(kvp.Key))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        return Task.FromResult(filtered);
    }

    public Task<PublicRegisterFileModel> GetLatestFileAsync(string containerName, string blobName)
    {
        return Task.FromResult(FileToReturn);
    }

    public Task<EnforcementActionFileViewModel> GetEnforcementActionFileByAgency(string agency)
    {
        return Task.FromResult(EnforcementFileByAgency);
    }

    public Task<IEnumerable<EnforcementActionFileViewModel>> GetEnforcementActionFiles()
    {
        return Task.FromResult<IEnumerable<EnforcementActionFileViewModel>>(EnforcementFiles);
    }

    public void Reset()
    {
        ProducerBlobModels = new();
        ComplianceBlobModel = new();
        FileToReturn = new();
        EnforcementFiles = [];
        EnforcementFileByAgency = new();
    }
}
