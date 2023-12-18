namespace FrontendObligationChecker.Models.ObligationChecker;

public class Question
{
    public string Key { get; set; } = default!;

    public QuestionType QuestionType { get; set; } = QuestionType.Radios;

    public string Title { get; set; } = default!;

    public string Description { get; set; } = default!;

    public string? AlternateDescription { get; set; }

    public string Detail { get; set; } = default!;

    public DetailPosition DetailPosition { get; set; }

    public string AlternateTitle { get; set; } = default!;

    public string AmountHandlePageText { get; set; } = default!;

    public string Summary { get; set; } = default!;

    public string Answer { get; set; } = default!;

    public bool HasError { get; set; }

    public string ErrorMessage { get; set; } = default!;

    public List<Option> Options { get; set; } = new();

    public Option? SelectedOption => Options.FirstOrDefault(option => option.IsSelected == true);

    public string GetDescription() => !string.IsNullOrEmpty(AlternateDescription) ? AlternateDescription : Description;

    public bool HasDetail => !string.IsNullOrWhiteSpace(Summary);

    public void SetAnswer(string questionAnswer)
    {
        if (!string.IsNullOrWhiteSpace(questionAnswer))
        {
            Options.ForEach(option => option.IsSelected = null);
            var answerList = questionAnswer.Split(",");
            var optionList = Options.Where(option => answerList.Contains(option.Value)).ToList();
            if (optionList.Any())
            {
                foreach (var option in optionList)
                {
                    option.IsSelected = true;
                }

                Answer = questionAnswer;
                return;
            }
        }

        HasError = true;
    }
}