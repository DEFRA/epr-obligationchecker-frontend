using FrontendObligationChecker.Models.ObligationChecker;
using FrontendObligationChecker.Services.NextFinder.Interfaces;

namespace FrontendObligationChecker.Services.NextFinder;
public class AggregateFinder : INextFinder
{
    public OptionPath? Next(IList<Question> questions)
    {
        ArgumentNullException.ThrowIfNull(questions);

        var isEligible = questions
            .Any(question =>
                question.Options.Exists(option => option.IsSelected.GetValueOrDefault() && option.Next == OptionPath.Primary));
        return isEligible
            ? OptionPath.Secondary
            : OptionPath.Primary;
    }
}