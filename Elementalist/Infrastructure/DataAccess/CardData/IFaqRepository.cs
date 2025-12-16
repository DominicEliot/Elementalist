using System.Globalization;
using System.Text.RegularExpressions;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using ElementalistBot.Models;
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
        if (_faqs.Count() > 0)
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

public class HtmlFaqRepository : IFaqRepository
{
    private Dictionary<string, List<CardFaq>> _faqs = [];

    public async Task<Dictionary<string, List<CardFaq>>> GetFaqs()
    {
        if (_faqs.Count() > 0)
        {
            return _faqs; //Todo: this will cause the FAQs to become out of date if the bot is running for a long time, maybe we should reload the FAQs after a few days?
        }

        var web = new HtmlWeb();
        //var html = @"https://curiosa.io/faqs";
        //var htmlDoc = await web.LoadFromWebAsync(html);

        //Todo: maybe this should be a separate interface Implementation? 
        var htmlDoc = new HtmlDocument();
        htmlDoc.Load(Path.Combine("Infrastructure", "DataAccess", "CardData", "faq.html"));

        var cardNodes = htmlDoc.DocumentNode
            .SelectNodes("/html[1]/body[1]/div[1]/main[1]/div[1]/div[1]/div[1]/div[3]/div");

        foreach (var singleCardNode in cardNodes)
        {
            var cardName = Regex.Replace(singleCardNode.SelectSingleNode("h3").InnerText, @"(\s|\n)+", " ").Trim();
            List<CardFaq> cardFaqs = ParseHtmlFaqsForSingleCard(singleCardNode);

            _faqs.Add(cardName, cardFaqs);
        }

        return _faqs;
    }

    private static List<CardFaq> ParseHtmlFaqsForSingleCard(HtmlNode singleCardNode)
    {
        var cardFaqs = new List<CardFaq>();

        foreach (var faq in singleCardNode.SelectNodes("div/div"))
        {
            var question = faq.GetAttributes("class")
                .FirstOrDefault(a => a?.Value?.Contains("curiosa-faq") == true && !a.Value.Contains("curiosa-faq-a"))?
                .OwnerNode;

            var cardFaq = new CardFaq
            {
                HasTable = faq.Descendants("table").Any(),
                QuestionText = Regex.Replace(faq.SelectSingleNode("p[1]").InnerText, @"(\s|\n)+", " "),
                AnswerText = Regex.Replace(faq.SelectSingleNode("p[2]").InnerText, @"(\s|\n)+", " ")
            };

            cardFaqs.Add(cardFaq);
        }

        return cardFaqs;
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
