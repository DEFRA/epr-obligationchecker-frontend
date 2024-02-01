namespace FrontendObligationChecker.Controllers;

using Microsoft.AspNetCore.Mvc;
using Models.LargeProducerRegister;
using ViewModels;

[Route(PagePath.LargeProducerRegister)]
public class LargeProducerRegisterController : Controller
{
    public async Task<IActionResult> Get()
    {
        return View("LargeProducerRegister", new LargeProducerRegisterViewModel());
    }
}