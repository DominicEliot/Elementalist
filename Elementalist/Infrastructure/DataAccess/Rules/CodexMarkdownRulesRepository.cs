using System.Text.RegularExpressions;
using Elementalist.Models;

namespace Elementalist.Infrastructure.DataAccess.Rules;

public partial class CodexMarkdownRulesRepository : IRulesRepository
{
    private IEnumerable<CodexEntry> _rules = [];
    private IEnumerable<string> _keywords = [];

    public async Task<IEnumerable<string>> GetKeywords(CancellationToken cancellationToken)
    {
        if (_keywords.Any())
        {
            return _keywords;
        }

        if (!_rules.Any())
        {
            await GetRules(cancellationToken);
        }

        var keywords = new List<string>(_rules.Select(r => r.Title));
        keywords.AddRange(_rules.SelectMany(r => r.Subcodexes.Select(s => s.Title)));

        _keywords = keywords;
        return _keywords;
    }

    public async Task<IEnumerable<CodexEntry>> GetRules(CancellationToken cancellationToken)
    {
        if (_rules.Any())
        {
            return _rules;
        }

        var markdownDirectory = new DirectoryInfo(Path.Combine(nameof(Infrastructure), nameof(DataAccess), nameof(Rules), "markdown"));
        var codex = new List<CodexEntry>();

        foreach(var file in markdownDirectory.EnumerateFiles("*.md"))
        {
            var content = await File.ReadAllTextAsync(file.FullName, cancellationToken);
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
                    Content =  match.Groups[3].Value.Trim(),
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
                Title =  file.Name.Replace(".md", ""),
                Subcodexes = subCodex,
                Url = codexUrl
            });
        }

        _rules = codex;
        return _rules;
    }

    [GeneratedRegex(@"^(?=# )")]
    private static partial Regex LineStartsWithH1Regex();

    [GeneratedRegex(@"^# (.*)\n(.*)")]
    private static partial Regex GroupedCodexContentRegex();

    [GeneratedRegex(@"##((.*)\n([^#]*))")]
    private static partial Regex SubcodexRegex();
}
