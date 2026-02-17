using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using CsvHelper;
using Elementalist.Models;

namespace Elementalist.Infrastructure.DataAccess.Rules;

public class CodexCsvRulesRepository : IRulesRepository
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

        using var reader = new StreamReader(Path.Combine(nameof(Infrastructure), nameof(DataAccess), nameof(Rules), "codex.csv"));
        var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var csvRules = csv.GetRecords<CodexCsv>().ToList();
        var rules = new List<CodexEntry>();

        for (var i = 0; i < csvRules.Count; i++)
        {
            var rule = csvRules[i];
            if (!string.IsNullOrEmpty(rule.title))
            {
                var codex = new CodexEntry() { Content = Regex.Replace(rule.content,"(?<!\")\n", ""), Title = rule.title };

                var j = 1;
                while (csvRules.Count > i + j + 1 && string.IsNullOrEmpty(csvRules[i + j].title)) //in the csv subcodexes have empty titles and use the subcodex field for all their data
                {
                    var subcodex = csvRules[i + j].subcodexes;
                    j++;
                    if (subcodex is null)
                    {
                        continue;
                    }
                    var colonSeperator = subcodex.IndexOf(':');

                    var content = subcodex.Substring(colonSeperator + 1).Trim();
                    var title = subcodex.Substring(0, colonSeperator).Trim();
                    codex.Subcodexes.Add(new CodexEntry() { Content = content, Title = title });
                }
                rules.Add(codex);
                continue;
            }
        }
        _rules = rules;

        return _rules;
    }
}

public interface IRulesRepository
{
    Task<IEnumerable<CodexEntry>> GetRules(CancellationToken cancellationToken);

    Task<IEnumerable<string>> GetKeywords(CancellationToken cancellationToken);
}
