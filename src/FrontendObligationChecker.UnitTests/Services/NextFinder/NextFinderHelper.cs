using FrontendObligationChecker.Models.ObligationChecker;

namespace FrontendObligationChecker.UnitTests.Services.NextFinder;
public static class NextFinderHelper
{
    public static Question GetQuestionPrimaryOptionSelected()
    {
        var options = new List<Option> { GetPrimaryPathOption(true), GetSecondaryPathOption() };
        return new Question { Options = options };
    }

    public static Question GetQuestionSecondaryOptionSelected()
    {
        var options = new List<Option> { GetPrimaryPathOption(), GetSecondaryPathOption(true) };
        return new Question { Options = options };
    }

    public static List<Question> GetSingleQuestionList()
    {
        return new List<Question> { GetQuestionPrimaryOptionSelected() };
    }

    public static List<Question> GetMultiQuestionList_OneTrue()
    {
        return new List<Question> { GetQuestionPrimaryOptionSelected(), GetQuestionSecondaryOptionSelected() };
    }

    public static List<Question> GetMultiQuestionList_AllFalse()
    {
        return new List<Question> { GetQuestionSecondaryOptionSelected(), GetQuestionSecondaryOptionSelected() };
    }

    private static Option GetPrimaryPathOption(bool isSelected = false)
    {
        return new Option()
        {
            Next = OptionPath.Primary,
            IsSelected = isSelected
        };
    }

    private static Option GetSecondaryPathOption(bool isSelected = false)
    {
        return new Option()
        {
            Next = OptionPath.Secondary,
            IsSelected = isSelected
        };
    }
}