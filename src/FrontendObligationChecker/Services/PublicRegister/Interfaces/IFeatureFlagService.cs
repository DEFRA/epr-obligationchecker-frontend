namespace FrontendObligationChecker.Services.PublicRegister.Interfaces
{
    public interface IFeatureFlagService
    {
        Task<bool> IsComplianceSchemesRegisterEnabledAsync();

        Task<bool> IsEnforcementActiionsSectionEnabledAsync();
    }
}