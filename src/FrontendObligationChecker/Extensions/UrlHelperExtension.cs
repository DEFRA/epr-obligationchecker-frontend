namespace FrontendObligationChecker.Extensions;

using Microsoft.AspNetCore.Mvc;

public static class UrlHelperExtension
{
    public static string HomePath(this IUrlHelper url)
    {
        return url.Action("Get", "LargeProducerRegister");
    }
}