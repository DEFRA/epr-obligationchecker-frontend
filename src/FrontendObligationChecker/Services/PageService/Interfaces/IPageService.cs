using FrontendObligationChecker.Models.ObligationChecker;

namespace FrontendObligationChecker.Services.PageService.Interfaces;

public interface IPageService
{
    Task<Page?> GetPageAsync(string path);

    Task<Page?> SetAnswersAndGetPageAsync(string path, IFormCollection formCollection);
}