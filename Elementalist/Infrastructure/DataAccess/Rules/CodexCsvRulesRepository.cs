using System.Globalization;
using System.Text.Json;
using CsvHelper;
using ElementalistBot.Models;

namespace ElementalistBot.Infrastructure.DataAccess.Rules;

public class CodexCsvRulesRepository : IRulesRepository
{
    private IEnumerable<CodexEntry> _rules = [];

    public async Task<IEnumerable<CodexEntry>> GetRules()
    {
        if (_rules.Any())
        {
            return _rules;
        }

        //Todo: will `blob:https://curiosa.io/2e27b065-73b7-4088-9b02-f266058d9eaa` work forever? check next codex update?
        //Also will `blob:https://curiosa.io/7ce0e074-ef59-404f-99ee-6eeaa8cb880d` work for FAQs?
        using var reader = new StreamReader(Path.Combine(nameof(Infrastructure), nameof(DataAccess), nameof(Rules), "codex.csv"));
        var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var csvRules = csv.GetRecords<CodexCsv>().ToList();
        var rules = new List<CodexEntry>();

        for (var i = 0; i < csvRules.Count; i++)
        {
            var rule = csvRules[i];
            if (!string.IsNullOrEmpty(rule.title))
            {
                var codex = new CodexEntry() { Content = rule.content, Title = rule.title };

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
    Task<IEnumerable<CodexEntry>> GetRules();
}
