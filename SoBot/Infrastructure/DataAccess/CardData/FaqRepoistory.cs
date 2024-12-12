using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace SorceryBot.Infrastructure.DataAccess.CardData;
public class FaqRepoistory
{
    public FaqRepoistory()
    {
        
    }

    public async Task LoadAsync()
    {
        var html = @"https://curiosa.io/faqs";
        var web = new HtmlWeb();
        //var htmlDoc = await web.LoadFromWebAsync(html);
        var htmlDoc = new HtmlDocument();

        htmlDoc.Load(Path.Combine("Infrastructure", "DataAccess", "CardData", "faq.html"));

        var nodes = htmlDoc.DocumentNode
            .SelectNodes("/html[1]/body[1]/div[1]/main[1]/div[1]/div[1]/div[1]/div");

        foreach (var node in nodes)
        {
            var cardName = node.SelectSingleNode("h3").InnerText.Trim();
            var faqs = new List<string>();

            foreach(var faq in node.SelectNodes("div"))
            {
                faqs.Add(faq.InnerHtml);
            }

            Console.WriteLine($"{cardName} has {faqs.Count} faqs");
        }
    }
}
