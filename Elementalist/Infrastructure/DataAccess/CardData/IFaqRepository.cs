using System.Globalization;
using System.Text.RegularExpressions;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Elementalist.Models;
using HtmlAgilityPack;

namespace Elementalist.Infrastructure.DataAccess.CardData;
public interface IFaqRepository
{
    Task<Dictionary<string, List<CardFaq>>> GetFaqs();
}

public class CsvFaqRepository : IFaqRepository
{
    private Dictionary<string, List<CardFaq>> _faqs = [];

    public Task<Dictionary<string, List<CardFaq>>> GetFaqs()
    {
        if (_faqs.Count > 0)
        {
            return Task.FromResult(_faqs);
        }

        using var reader = new StreamReader(Path.Combine(nameof(Infrastructure), nameof(DataAccess), nameof(CardData), "faq.csv"));
        var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var csvFaqs = csv.GetRecords<FaqCsvEntry>().ToList();

        var compiledList = new Dictionary<string, List<CardFaq>>();

        for (var i = 0; i < csvFaqs.Count; i++)
        {
            var csvFaq = csvFaqs[i];
            if (string.IsNullOrEmpty(csvFaq.card_name))
            {
                continue;
            }

            var singleCardFaqs = new List<CardFaq>
            {
                new() { QuestionText = csvFaq.question, AnswerText = csvFaq.answer }
            };

            var j = 1;
            while (csvFaqs.Count > i + j + 1 && string.IsNullOrEmpty(csvFaqs[i + j].card_name)) //in the csv multiple entries for the same card appear in order but with empty titles
            {
                singleCardFaqs.Add(new() { QuestionText = csvFaqs[i + j].question, AnswerText = csvFaqs[i + j].answer });
                j++;
            }

            compiledList.Add(csvFaq.card_name, singleCardFaqs);
        }
        _faqs = compiledList;

        return Task.FromResult(_faqs);
    }

    private class FaqCsvEntry
    {
        [Name("card name")]
        public required string card_name { get; init; }
        public required string question { get; init; }
        public required string answer { get; init; }
    }
}

public class CardFaq
{
    public string QuestionText { get; set; } = "";
    public string AnswerText { get; set; } = "";
    public bool HasTable { get; set; }

    public override string ToString()
    {
        return QuestionText + "\n" + AnswerText;
    }
}
