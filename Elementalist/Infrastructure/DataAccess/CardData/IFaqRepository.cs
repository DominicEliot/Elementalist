using System.Data;
using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Octokit;
using Serilog;

namespace Elementalist.Infrastructure.DataAccess.CardData;

public interface IFaqRepository
{
    Task<Dictionary<string, IEnumerable<CardFaq>>> GetFaqs(CancellationToken ct);
}

public class GithubFaqRepository(IMemoryCache _cache, IOptions<DataRefreshOptions> _options, HttpClient _httpClient) : IFaqRepository
{
    private static readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

    public async Task<Dictionary<string, IEnumerable<CardFaq>>> GetFaqs(CancellationToken ct)
    {
        if (_cache.TryGetValue("FaqEntries", out Dictionary<string, IEnumerable<CardFaq>>? faqs) && faqs?.Count > 0)
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

    private async Task<Dictionary<string, IEnumerable<CardFaq>>> DownloadFaqs(CancellationToken ct)
    {
        Log.Information("Fetching faq data from GitHub.");

        var responsePayload = await GetFaqJson(ct);
        if (responsePayload == null)
        {
            if (_cache.TryGetValue("FaqEntries", out Dictionary<string, IEnumerable<CardFaq>>? faqs) && faqs?.Count > 0)
            {
                return faqs;
            }
            return [];
        }

        var dict = JsonSerializer.Deserialize<Dictionary<string, IEnumerable<MarkdownFaqEntry>>>(responsePayload);

        Log.Information("Loaded {count} faq entries.", dict?.Sum(kvp => kvp.Key.Length) ?? 0);

        return dict?.ToDictionary(kvp =>
            kvp.Key.Replace('_', ' '),
            kvp => kvp.Value.Select(v => new CardFaq()
            {
                AnswerText = v.answer,
                QuestionText = v.question,
                HasTable = false
            }),
            StringComparer.OrdinalIgnoreCase
        ) ?? [];
    }

    private async Task<string?> GetFaqJson(CancellationToken ct)
    {

        var github = new GitHubClient(new Octokit.ProductHeaderValue(nameof(Elementalist)));
        var files = await github.Repository.Content.GetAllContents("DominicEliot", "sorcery-markdown-codex", "generatedFaqs.json");

        if (files.Count == 0)
        {
            return null;
        }

        return files[0].Content;
    }

    private class MarkdownFaqEntry
    {
        public required IEnumerable<string> cards { get; init; } = [];
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
    public required string QuestionText { get; init; }
    public required string AnswerText { get; init; }
    public bool HasTable { get; set; }

    public override string ToString()
    {
        return QuestionText + "\n" + AnswerText;
    }
}
