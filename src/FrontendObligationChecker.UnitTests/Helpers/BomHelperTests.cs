namespace FrontendObligationChecker.UnitTests.Helpers
{
    using FluentAssertions;
    using FrontendObligationChecker.Helpers;

    [TestClass]
    public class BomHelperTests
    {
        [TestMethod]
        public void PrependBOMBytes_Returns_When_MemoryStreamIsNull()
        {
            MemoryStream memoryStream = null;

            BomHelper.PrependBOMBytes(memoryStream);

            memoryStream.Should().BeNull();
        }
    }
}