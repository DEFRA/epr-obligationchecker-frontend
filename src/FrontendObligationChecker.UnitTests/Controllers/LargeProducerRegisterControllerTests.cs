using FluentAssertions;

using FrontendObligationChecker.Controllers;
using FrontendObligationChecker.Models.LargeProducerRegister;
using FrontendObligationChecker.ViewModels;

using Microsoft.AspNetCore.Mvc;

namespace FrontendObligationChecker.UnitTests.Controllers;

[TestClass]
public class LargeProducerRegisterControllerTests
{
    private LargeProducerRegisterController _controller;

    [TestInitialize]
    public void TestInitialize()
    {
        _controller = new LargeProducerRegisterController();
    }

    [TestMethod]
    [DataRow(PagePath.LargeProducerRegister)]
    public async Task Get_ReturnsLargeProducerRegisterView_WhenCalled(string path)
    {
        // Arrange

        // Act
        var result = await _controller.Get() as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result.ViewName.Should().Be("LargeProducerRegister");
        result.Model.Should().BeEquivalentTo(new LargeProducerRegisterViewModel());
    }
}