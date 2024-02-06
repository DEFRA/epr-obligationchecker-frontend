namespace FrontendObligationChecker.ViewModels;

using Models.ObligationChecker;

public class PageModel : BaseViewModel
{
    public PageModel(Page page)
    {
        Page = page;
        BackLinkToDisplay = page is { HasBackLink: true, IsBackButtonHidden: false } ? page.BackLinkPath : string.Empty;
    }

    public Page Page { get; }
}