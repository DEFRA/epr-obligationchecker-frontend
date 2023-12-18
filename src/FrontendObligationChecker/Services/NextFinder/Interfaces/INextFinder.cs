using FrontendObligationChecker.Models.ObligationChecker;

namespace FrontendObligationChecker.Services.NextFinder.Interfaces;
public interface INextFinder
{
    OptionPath? Next(IList<Question> questions);
}