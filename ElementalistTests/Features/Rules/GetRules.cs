using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElementalistBot.Infrastructure.DataAccess.Rules;
using Xunit;

namespace ElementalistTests.Features.Rules;

public class GetRules
{
    [Fact]
    public async Task LoadCodexEntriesAsync()
    {
        var repo = new CodexCsvRulesRepository();
        var rules = await repo.GetRules();

        Assert.NotEmpty(rules);
    }
}
