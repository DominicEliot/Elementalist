using System.Globalization;
using System.Net.Http.Headers;
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
        var responsePayload = await GetFaqJson(ct);
        return [];
    }

    private static async Task<string> GetFaqJson(CancellationToken ct)
    {
        return await File.ReadAllTextAsync(Path.Combine("Infrastructure", "DataAccess", "CardData", "rawfaq.json"), ct);

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var request = new HttpRequestMessage(HttpMethod.Get, "https://sorcerytcg.com/api/trpc/cms.faqs?batch=1");
        request.Headers.Add("Referer", "https://sorcerytcg.com/cards");
        // request.Headers.Add("trpc-accept", "application/jsonl");
        request.Headers.Add("x-trpc-source", "nextjs-react");
        request.Headers.Add("Sec-GPC", "1");
        request.Headers.Add("Sec-Fetch-Dest", "document");
        request.Headers.Add("Sec-Fetch-Site", "none");

        var postResults = await client.SendAsync(request, ct);
        if (!postResults.IsSuccessStatusCode)
        {
            return null;
        }

        var responsePayload = await postResults.Content.ReadAsStringAsync(ct);
        // await File.WriteAllTextAsync(Path.Combine("Infrastructure", "DataAccess", "CardData", "rawfaq.json"), responsePayload, ct);
        return responsePayload;
    }

    private class FaqCsvEntry
    {
        [Name("card name")] public required string card_name { get; init; }
        public required string question { get; init; }
        public required string answer { get; init; }
    }

    private class Root
    {
        public required Result result { get; set; }
    }

    private class Result
    {
        public required Data data { get; set; }
    }

    private class Data
    {
        public required string[] json { get; set; }
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
