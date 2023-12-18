using FrontendObligationChecker.Models;
using FrontendObligationChecker.Models.Config;
using FrontendObligationChecker.Services.Infrastructure.Interfaces;

using Microsoft.Extensions.Options;

namespace FrontendObligationChecker.Middleware
{
    public class AnalyticsCookieMiddleware
    {
        private readonly RequestDelegate _next;

        public AnalyticsCookieMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext httpContext,
            ICookieService cookieService,
            IOptions<AnalyticsOptions> googleAnalyticsOptions)
        {
            var cookieConsentState = cookieService.GetConsentState(httpContext.Request.Cookies, httpContext.Response.Cookies);
            httpContext.Items[ContextKeys.UseGoogleAnalyticsCookieKey] = cookieConsentState.CookiesAccepted;
            httpContext.Items[ContextKeys.TagManagerContainerIdKey] = googleAnalyticsOptions.Value.TagManagerContainerId;

            await _next(httpContext);
        }
    }
}