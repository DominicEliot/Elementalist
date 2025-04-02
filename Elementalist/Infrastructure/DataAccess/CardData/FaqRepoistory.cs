using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace Elementalist.Infrastructure.DataAccess.CardData;

public class FaqRepoistory
{
    private Dictionary<string, List<CardFaq>> _faqs = [];

    public Task<Dictionary<string, List<CardFaq>>> GetFaqs()
    {
        if (_faqs.Count() > 0)
        {
            return Task.FromResult(_faqs);
        }

        var web = new HtmlWeb();
        //var html = @"https://curiosa.io/faqs";
        //var htmlDoc = await web.LoadFromWebAsync(html);
        var htmlDoc = new HtmlDocument();

        htmlDoc.Load(Path.Combine("Infrastructure", "DataAccess", "CardData", "faq.html"));

        var cardNodes = htmlDoc.DocumentNode
            .SelectNodes("/html[1]/body[1]/div[1]/main[1]/div[1]/div[1]/div[1]/div");

        foreach (var singleCardNode in cardNodes)
        {
            var cardName = Regex.Replace(singleCardNode.SelectSingleNode("h3").InnerText, @"(\s|\n)+", " ").Trim();
            List<CardFaq> cardFaqs = ParseHtmlFaqsForSingleCard(singleCardNode);

            _faqs.Add(cardName, cardFaqs);
        }

        return Task.FromResult(_faqs);
    }

    private static List<CardFaq> ParseHtmlFaqsForSingleCard(HtmlNode singleCardNode)
    {
        var cardFaqs = new List<CardFaq>();

        foreach (var faq in singleCardNode.SelectNodes("div/div"))
        {
            var question = faq.GetAttributes("class")
                .FirstOrDefault(a => a?.Value?.Contains("curiosa-faq") == true && !a.Value.Contains("curiosa-faq-a"))?
                .OwnerNode;

            var cardFaq = new CardFaq { Html = faq.InnerHtml };
            cardFaq.HasTable = faq.Descendants("table").Any();
            cardFaq.QuestionText = Regex.Replace(faq.SelectSingleNode("p[1]").InnerText, @"(\s|\n)+", " ");
            cardFaq.AnswerText = Regex.Replace(faq.SelectSingleNode("p[2]").InnerText, @"(\s|\n)+", " ");

            cardFaqs.Add(cardFaq);
        }

        return cardFaqs;
    }
}

public class CardFaq
{
    public string QuestionText { get; set; } = "";
    public string AnswerText { get; set; } = "";
    public string? Html { get; init; }
    public bool HasTable { get; set; }

    public override string ToString()
    {
        return QuestionText + "\n" + AnswerText;
    }
}
