using System.Text.Json;
using ElementalistBot.Models;

namespace ElementalistBot.Infrastructure.DataAccess.Rules;

public class RulesRepository
{
    private IEnumerable<Rule> _rules = [];

    public async Task<IEnumerable<Rule>> GetRules()
    {
        if (!_rules.Any())
        {
            var jsonFile = await File.ReadAllTextAsync(Path.Combine("rules.json"));
            _rules = JsonSerializer.Deserialize<IEnumerable<Rule>>(jsonFile) ?? [];
        }

        return _rules;
    }
}
