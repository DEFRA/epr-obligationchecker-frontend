namespace FrontendObligationChecker.FeatureManagement
{
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.FeatureManagement.Mvc;

    public class RedirectDisabledFeatureHandler : IDisabledFeaturesHandler
    {
        public Task HandleDisabledFeatures(IEnumerable<string> features, ActionExecutingContext context)
        {
            context.Result = new RedirectToActionResult("Error", "Error", HttpStatusCode.NotFound);
            return Task.CompletedTask;
        }
    }
}