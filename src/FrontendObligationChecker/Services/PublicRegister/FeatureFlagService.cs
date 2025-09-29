namespace FrontendObligationChecker.Services.PublicRegister
{
    using System.Threading.Tasks;
    using FrontendObligationChecker.Constants;
    using FrontendObligationChecker.Services.PublicRegister.Interfaces;
    using Microsoft.FeatureManagement;

    public class FeatureFlagService(IFeatureManager featureManager) : IFeatureFlagService
    {
        private readonly IFeatureManager _featureManager = featureManager;

        public Task<bool> IsComplianceSchemesRegisterEnabledAsync() =>
            _featureManager.IsEnabledAsync(FeatureFlags.ComplianceSchemesRegisterEnabled);

        public Task<bool> IsEnforcementActionsSectionEnabledAsync() =>
            _featureManager.IsEnabledAsync(FeatureFlags.EnforcementActionsSectionEnabled);

        public Task<bool> IsPublicRegisterNextYearEnabledAsync() =>
            _featureManager.IsEnabledAsync(FeatureFlags.PublicRegisterNextYearEnabled);
    }
}