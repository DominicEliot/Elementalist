using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elementalist.DiscordUi.Rules;
using Elementalist.Infrastructure.DataAccess.Rules;
using Xunit;

namespace ElementalistTests.DiscordUi;

public class CodexMessageTests
{
    [Fact]
    [Trait("Category", "Integration")]
    public async Task CreateCodexMessageTest()
    {
        var codexRepo = new CodexCsvRulesRepository();
        var codexMessageService = new CodexMessageService(codexRepo);

        var message = await codexMessageService.CreateCodexMessageAsync("Attack");

        var description = message?.Embeds?.First().Description;

        Assert.NotNull(description);
        Assert.Equal(description.LastIndexOf("_you_"), description.IndexOf("_you_"));
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task CreateCodexMessageWithSubcodexTest()
    {
        var codexRepo = new CodexCsvRulesRepository();
        var codexMessageService = new CodexMessageService(codexRepo);

        var message = await codexMessageService.CreateCodexMessageAsync("Casting Spells");

        var carriedField = message?.Embeds?.FirstOrDefault()?.Fields?.FirstOrDefault(f => f.Name == "Casting Artifacts");

        Assert.NotNull(carriedField?.Value);
        Assert.Contains("_carriable artifact_", carriedField.Value);
    }
}
