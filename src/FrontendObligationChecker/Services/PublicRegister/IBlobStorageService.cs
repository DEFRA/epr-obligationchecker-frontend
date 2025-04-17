namespace FrontendObligationChecker.Services.PublicRegister;

using FrontendObligationChecker.ViewModels.PublicRegister;

public interface IBlobStorageService
{
    Task<GuidanceViewModel> GetGuidanceViewModelAsync();
}