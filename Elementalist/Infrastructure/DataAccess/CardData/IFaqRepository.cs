using System.Globalization;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Elementalist.Infrastructure.DataAccess.CardData;

public interface IFaqRepository
{
    Task<Dictionary<string, List<CardFaq>>> GetFaqs(CancellationToken ct);
}

public class CsvFaqRepository(IMemoryCache _cache, IOptions<DataRefreshOptions> _options) : IFaqRepository
{
    private static readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

    public async Task<Dictionary<string, List<CardFaq>>> GetFaqs(CancellationToken ct)
    {
        if (_cache.TryGetValue("FaqEntries", out Dictionary<string, List<CardFaq>>? faqs) && faqs?.Count > 0)
        {
            return faqs;
        }

        await _lock.WaitAsync(ct);
        try
        {
            if (_cache.TryGetValue("FaqEntries", out faqs) && faqs?.Count > 0)
            {
                return faqs;
            }

            faqs = await DownloadFaqs(ct);
            _cache.Set("FaqEntries", faqs, TimeSpan.FromHours(_options.Value.Hours));
            return faqs;
        }
        finally
        {
            _lock.Release();
        }
    }

    private static async Task<Dictionary<string, List<CardFaq>>> DownloadFaqs(CancellationToken ct)
    {
        using var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, "https://curiosa.io/api/trpc/cms.downloadFaqs?batch=1");
        request.Headers.Add("Host", "curiosa.io");
        request.Headers.Add("Origin", "https://curiosa.io");

        var httpContent = JsonContent.Create("{}");

        request.Content = httpContent;

        var postResults = await client.SendAsync(request, ct);
        if (!postResults.IsSuccessStatusCode)
        {
            return [];
        }

        var responsePayload = await postResults.Content.ReadAsStringAsync(ct);
        var jObject = JsonSerializer.Deserialize<Root[]>(responsePayload);
        var internalByteArray = jObject?[0].result.data.json.Select(x => (byte)x).ToArray() ?? [];

        using var reader = new StreamReader(new MemoryStream(internalByteArray), Encoding.UTF8);
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

        return compiledList;
    }

    private class FaqCsvEntry
    {
        [Name("card name")]
        public required string card_name { get; init; }
        public required string question { get; init; }
        public required string answer { get; init; }
    }

    private class Root { public required Result result { get; set; } }

    private class Result { public required Data data { get; set; } }

    private class Data { public required int[] json { get; set; } public required Meta meta { get; set; } }

    private class Meta { public required string[][] values { get; set; } }

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
