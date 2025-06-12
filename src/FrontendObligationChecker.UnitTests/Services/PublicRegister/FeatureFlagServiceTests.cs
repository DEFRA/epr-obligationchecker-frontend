namespace FrontendObligationChecker.UnitTests.Services.PublicRegister
{
    using FluentAssertions;
    using FrontendObligationChecker.Constants;
    using FrontendObligationChecker.Services.PublicRegister;
    using Microsoft.FeatureManagement;
    using Moq;

    [TestClass]
    public class FeatureFlagServiceTests
    {
        private Mock<IFeatureManager> _mockFeatureManager;
        private FeatureFlagService _featureFlagService;

        [TestInitialize]
        public void Setup()
        {
            _mockFeatureManager = new Mock<IFeatureManager>();
            _featureFlagService = new FeatureFlagService(_mockFeatureManager.Object);
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task IsComplianceSchemesRegisterEnabledAsync_ShouldReturnExpectedValue(bool expected)
        {
            // Arrange
            _mockFeatureManager
                .Setup(m => m.IsEnabledAsync(FeatureFlags.ComplianceSchemesRegisterEnabled))
                .ReturnsAsync(expected);

            // Act
            var result = await _featureFlagService.IsComplianceSchemesRegisterEnabledAsync();

            // Assert
            result.Should().Be(expected);
            _mockFeatureManager.Verify(m => m.IsEnabledAsync(FeatureFlags.ComplianceSchemesRegisterEnabled), Times.Once);
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task IsEnforcementActionsSectionEnabledAsync_ShouldReturnExpectedValue(bool expected)
        {
            // Arrange
            _mockFeatureManager
                .Setup(m => m.IsEnabledAsync(FeatureFlags.EnforcementActionsSectionEnabled))
                .ReturnsAsync(expected);

            // Act
            var result = await _featureFlagService.IsEnforcementActionsSectionEnabledAsync();

            // Assert
            result.Should().Be(expected);
            _mockFeatureManager.Verify(m => m.IsEnabledAsync(FeatureFlags.EnforcementActionsSectionEnabled), Times.Once);
        }
    }
}