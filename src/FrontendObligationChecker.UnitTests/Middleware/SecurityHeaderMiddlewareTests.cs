namespace FrontendObligationChecker.UnitTests.Middleware;

using FrontendObligationChecker.Middleware;
using Microsoft.AspNetCore.Http;
using Moq;

[TestClass]
public class SecurityHeaderMiddlewareTests
{
    [TestMethod]
    public async Task Invoke_AddsSecurityHeaders()
    {
        // Arrange
        var mockRequestDelegate = new Mock<RequestDelegate>();
        var middleware = new SecurityHeaderMiddleware(mockRequestDelegate.Object);

        var context = new DefaultHttpContext();
        var responseHeaders = context.Response.Headers;

        // Act
        await middleware.Invoke(context);

        // Assert
        Assert.IsTrue(responseHeaders.ContainsKey("Content-Security-Policy"));
        Assert.IsTrue(responseHeaders.ContainsKey("Cross-Origin-Embedder-Policy"));
        Assert.IsTrue(responseHeaders.ContainsKey("Cross-Origin-Opener-Policy"));
        Assert.IsTrue(responseHeaders.ContainsKey("Cross-Origin-Resource-Policy"));
        Assert.IsTrue(responseHeaders.ContainsKey("Permissions-Policy"));
        Assert.IsTrue(responseHeaders.ContainsKey("Referrer-Policy"));
        Assert.IsTrue(responseHeaders.ContainsKey("X-Content-Type-Options"));
        Assert.IsTrue(responseHeaders.ContainsKey("X-Frame-Options"));
        Assert.IsTrue(responseHeaders.ContainsKey("X-Permitted-Cross-Domain-Policies"));
        Assert.IsTrue(responseHeaders.ContainsKey("X-Robots-Tag"));

        // Verify the content of the headers, especially those that are dynamic
        Assert.Contains("script-src 'self' 'nonce-", responseHeaders.ContentSecurityPolicy.ToString());
        Assert.AreEqual("require-corp", responseHeaders["Cross-Origin-Embedder-Policy"].ToString());
        Assert.AreEqual("same-origin", responseHeaders["Cross-Origin-Opener-Policy"].ToString());
        Assert.AreEqual("same-origin", responseHeaders["Cross-Origin-Resource-Policy"].ToString());
        Assert.AreEqual("strict-origin-when-cross-origin", responseHeaders["Referrer-Policy"].ToString());
        Assert.AreEqual("nosniff", responseHeaders.XContentTypeOptions.ToString());
        Assert.AreEqual("deny", responseHeaders.XFrameOptions.ToString());
        Assert.AreEqual("none", responseHeaders["X-Permitted-Cross-Domain-Policies"].ToString());
        Assert.AreEqual("noindex, nofollow", responseHeaders["X-Robots-Tag"].ToString());
    }
}