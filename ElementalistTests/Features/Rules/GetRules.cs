using Elementalist.Infrastructure.DataAccess.Rules;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Xunit;

namespace ElementalistTests.Features.Rules;

public class GetRules
{
    [Fact]
    public async Task LoadCodexEntriesAsync()
    {
        var cacheOptions = Options.Create(new MemoryCacheOptions());
        var repo = new CodexMarkdownRulesRepository(new HttpClient(), null!, new MemoryCache(cacheOptions));
        var rules = await repo.GetRules(CancellationToken.None);
        var keywords = await repo.GetKeywords(CancellationToken.None);
        Assert.NotEmpty(rules);
    }
}
