using ElementalistBot.Models;

namespace ElementalistBot.Infrastructure.DataAccess.Rules;
public class RulesRepository
{
    public Task<IEnumerable<Rule>> GetRules()
    {
        return Task.FromResult(new List<Rule>().AsEnumerable());
    }
}
