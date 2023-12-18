using FrontendObligationChecker.Models.ObligationChecker;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace FrontendObligationChecker.UnitTests.Helpers;

public class PageForm
{
    private readonly List<KeyValuePair<string, StringValues>> _answers;

    public FormUrlEncodedContent FormUrlEncodedContent => new(_answers.Select(o => new KeyValuePair<string, string>(o.Key, o.Value.ToString())));

    public FormCollection FormCollection => new(_answers.ToDictionary(o => o.Key, o=> o.Value));

    public PageForm(TypeOfOrganisation answer)
    {
        string typeOfOrganisation = answer switch
        {
            TypeOfOrganisation.ParentCompany => "parent",
            TypeOfOrganisation.Subsidiary => "subsidiary",
            TypeOfOrganisation.IndividualCompany => "individual",
            TypeOfOrganisation.NotSet => string.Empty,
            _ => throw new ArgumentOutOfRangeException(nameof(answer), answer, null)
        };

        _answers = new List<KeyValuePair<string, StringValues>>
        {
            new(QuestionKey.TypeOfOrganisation, typeOfOrganisation)
        };
    }

    public PageForm(AnnualTurnover answer)
    {
        string annualTurnover = answer switch
        {
            AnnualTurnover.UnderOneMillion => "1",
            AnnualTurnover.OneMillionToTwoMillion => "2",
            AnnualTurnover.OverTwoMillion => "3",
            AnnualTurnover.NotSet => string.Empty,
            _ => throw new ArgumentOutOfRangeException(nameof(answer), answer, null)
        };

        _answers = new List<KeyValuePair<string, StringValues>> { new(QuestionKey.AnnualTurnover, annualTurnover) };
    }

    public PageForm(MaterialsForDrinksContainers answer)
    {
        var materialValues = new List<string>();

        foreach (var material in Enum.GetValues(typeof(MaterialsForDrinksContainers)).Cast<MaterialsForDrinksContainers>())
        {
            if (material == MaterialsForDrinksContainers.NotSet)
            {
                continue;
            }

            if (!answer.HasFlag(material))
            {
                continue;
            }

            string materialValue = material switch
            {
                MaterialsForDrinksContainers.None => "0",
                MaterialsForDrinksContainers.PlasticBottles => "1",
                MaterialsForDrinksContainers.GlassBottles => "2",
                MaterialsForDrinksContainers.SteelCans => "3",
                MaterialsForDrinksContainers.AluminiumCans => "4",
                _ => throw new ArgumentException($"Unsupported type of MaterialsForDrinksContainers: '{material}'")
            };

            materialValues.Add(materialValue);
        }

        _answers = new List<KeyValuePair<string, StringValues>>
        {
            new(QuestionKey.MaterialsForDrinksContainers, materialValues.ToArray())
        };
    }

    public PageForm(AmountYouSupply answer)
    {
        string amountYouHandle = answer switch
        {
            AmountYouSupply.HandleUnder25Tonnes => "1",
            AmountYouSupply.Handle25To50Tonnes => "2",
            AmountYouSupply.Handle50TonnesOrMore => "3",
            _ => throw new ArgumentOutOfRangeException(nameof(answer), answer, null)
        };

        _answers = new List<KeyValuePair<string, StringValues>> { new(QuestionKey.AmountYouSupply, amountYouHandle) };
    }

    public PageForm(string questionKey, YesNo yesNo)
    {
        _answers = new List<KeyValuePair<string, StringValues>> { new(questionKey, yesNo == YesNo.Yes ? "1" : "2"), };
    }
}