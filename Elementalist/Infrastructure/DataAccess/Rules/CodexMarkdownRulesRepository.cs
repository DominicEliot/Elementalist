using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Elementalist.Models;
using Microsoft.Extensions.Caching.Memory;
using Octokit;

namespace Elementalist.Infrastructure.DataAccess.Rules;

public partial class CodexMarkdownRulesRepository(HttpClient httpClient, ILogger<IRulesRepository> logger, IMemoryCache cache) : IRulesRepository
{
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public async Task<IEnumerable<string>> GetKeywords(CancellationToken cancellationToken)
    {
        return await cache.GetOrCreateAsync<IEnumerable<string>>("CodexKeywords", async entry =>
        {
            await _semaphore.WaitAsync();
            try
            {
                if (cache.TryGetValue("CodexKeywords", out IEnumerable<string>? lateEntry) && lateEntry is not null)
                {
                    return lateEntry;
                }
                var rules = await GetRules(cancellationToken);

                var keywords = new List<string>(rules.Select(r => r.Title));
                keywords.AddRange(rules.SelectMany(r => r.Subcodexes.Select(s => s.Title)));

                return keywords;
            }
            finally
            {
                _semaphore.Release();
            }

        }) ?? throw new NullReferenceException("Couldn't get or create the codex keywords cache");
    }

    public async Task<IEnumerable<CodexEntry>> GetRules(CancellationToken cancellationToken)
    {
        if (cache.TryGetValue("CodexEntries", out IEnumerable<CodexEntry>? codexEntries) && codexEntries is not null)
        {
            return codexEntries;
        }

        await _semaphore.WaitAsync();
        try
        {
            var github = new GitHubClient(new ProductHeaderValue(nameof(Elementalist)));
            var markDownFiles = await github.Repository.Content.GetAllContents("DominicEliot", "sorcery-markdown-codex", "markdown");
            var codex = new ConcurrentBag<CodexEntry>();

            var cts = new CancellationTokenSource();

            var options = new ParallelOptions()
            {
                MaxDegreeOfParallelism = 5,
                CancellationToken = cts.Token
            };

            await Parallel.ForEachAsync(markDownFiles, options, async (file, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                var response = await httpClient.GetAsync(file.DownloadUrl, ct);
                if (!response.IsSuccessStatusCode)
                {
                    cts.Cancel();
                }
                var content = await response.Content.ReadAsStringAsync(ct);
                var codexUrlMatch = Regex.Match(content, @"\[Codex Entry\]\((.*?)\)");

                string? codexUrl = null;
                if (codexUrlMatch.Success)
                {
                    codexUrl = codexUrlMatch.Groups[1].Value;
                    content = content.Remove(codexUrlMatch.Index, codexUrlMatch.Length);
                }

                var subCodex = new List<CodexEntry>();
                var textToRemove = new List<Tuple<int, int>>();
                foreach (Match match in SubcodexRegex().Matches(content))
                {
                    subCodex.Add(new CodexEntry
                    {
                        Content = match.Groups[3].Value.Trim(),
                        Title = match.Groups[2].Value.Trim(),
                        Subcodexes = [],
                        Url = codexUrl
                    });

                    textToRemove.Add(new Tuple<int, int>(match.Index, match.Length));
                }

                foreach (var position in textToRemove.OrderByDescending(t => t.Item1))
                {
                    content = content.Remove(position.Item1, position.Item2);
                }

                var title = file.Name.Replace(".md", "");
                codex.Add(new CodexEntry()
                {
                    Content = content.Replace($"# {title}", "").Trim(),
                    Title = file.Name.Replace(".md", ""),
                    Subcodexes = subCodex,
                    Url = codexUrl
                });
            });
            cache.Set("CodexEntries", codex.AsEnumerable(), TimeSpan.FromHours(12));

            return codex;
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning($"Could not download codex files from the upstream repository.");
            return [];
        }
        finally
        {
            _semaphore.Release();
        }
    }

    [GeneratedRegex(@"^(?=# )")]
    private static partial Regex LineStartsWithH1Regex();

    [GeneratedRegex(@"^# (.*)\n(.*)")]
    private static partial Regex GroupedCodexContentRegex();

    [GeneratedRegex(@"##((.*)\n([^#]*))")]
    private static partial Regex SubcodexRegex();
}
