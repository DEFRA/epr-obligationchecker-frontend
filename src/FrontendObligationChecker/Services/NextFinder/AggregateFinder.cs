using FrontendObligationChecker.Models.ObligationChecker;
using FrontendObligationChecker.Services.NextFinder.Interfaces;

namespace FrontendObligationChecker.Services.NextFinder;
public class AggregateFinder : INextFinder
{
    public OptionPath? Next(IList<Question> questions)
    {
        if (questions == null) throw new ArgumentNullException(nameof(questions));

        var isEligible = questions
            .Any(question =>
                question.Options.Any(option => option.IsSelected.GetValueOrDefault() && option.Next == OptionPath.Primary));
        return isEligible
            ? OptionPath.Secondary
            : OptionPath.Primary;
    }
}