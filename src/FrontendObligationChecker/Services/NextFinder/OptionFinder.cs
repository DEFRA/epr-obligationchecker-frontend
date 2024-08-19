using FrontendObligationChecker.Models.ObligationChecker;
using FrontendObligationChecker.Services.NextFinder.Interfaces;

namespace FrontendObligationChecker.Services.NextFinder;
public class OptionFinder : INextFinder
{
    public OptionPath? Next(IList<Question> questions)
    {
        ArgumentNullException.ThrowIfNull(questions);
        if (questions.Count > 1) throw new InvalidOperationException("List should have only one question");
        if (!questions.Any())
            return null;

        var option = questions
            .SingleOrDefault()!
            .Options
            .Find(x => x.IsSelected.GetValueOrDefault());

        return option switch
        {
            null => null,
            _ => option.Next
        };
    }
}