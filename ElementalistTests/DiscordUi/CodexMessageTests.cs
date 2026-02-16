using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elementalist.DiscordUi.Rules;
using Elementalist.Infrastructure.DataAccess.Rules;
using NetCord.Rest;
using Xunit;

namespace ElementalistTests.DiscordUi;

public class CodexMessageTests
{
    [Fact]
    [Trait("Category", "Integration")]
    public async Task CreateCodexMessageTest()
    {
        var codexRepo = new CodexMarkdownRulesRepository();
        var codexMessageService = new CodexMessageService(codexRepo);

        var message = await codexMessageService.CreateCodexMessageAsync("Attack", CancellationToken.None);

        var description = message?.Embeds?.First().Description ?? message?.Content;

        Assert.NotNull(description);
        Console.WriteLine(description);
        Assert.Equal(-1, description.IndexOf("****"));
        Assert.Equal(description.IndexOf("enemy"), description.LastIndexOf("enemy**"));
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task CheckDuplicatingTest()
    {
        var codexRepo = new CodexMarkdownRulesRepository();
        var codexMessageService = new CodexMessageService(codexRepo);

        var message = await codexMessageService.CreateCodexMessageAsync("Element", CancellationToken.None);

        var count = message?.Embeds?.First().Fields?.Select(f => f.Value).Count(s => s.Contains("dabbling")) ?? 0;
        count += message?.Embeds?.First().Description?.Contains("dabbling") == true ? 1 : 0;

        Assert.Equal(1, count);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task CheckKeywordFormatting()
    {
        var codexRepo = new CodexMarkdownRulesRepository();
        var codexMessageService = new CodexMessageService(codexRepo);

        var message = await codexMessageService.CreateCodexMessageAsync("Airborne", CancellationToken.None);

        var description = message?.Embeds?.First().Description ?? message?.Content;

        Assert.NotNull(description);
        Assert.Equal(-1, description.IndexOf("******"));
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task CreateCodexMessageWithSubcodexTest()
    {
        var codexRepo = new CodexMarkdownRulesRepository();
        var codexMessageService = new CodexMessageService(codexRepo);

        var message = await codexMessageService.CreateCodexMessageAsync("Casting Spells", CancellationToken.None);

        var carriedField = message?.Embeds?.FirstOrDefault()?.Fields?.FirstOrDefault(f => f.Name == "Casting Artifacts");

        Assert.NotNull(carriedField?.Value);
        Assert.Contains("*carriable artifact*", carriedField.Value);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task CreateCodexMessageComponentTest()
    {
        var codexRepo = new CodexMarkdownRulesRepository();
        var codexMessageService = new CodexMessageService(codexRepo);

        var message = await codexMessageService.CreateCodexMessageAsync("Disabled", CancellationToken.None);

        var selectMenu = message?.Components?.FirstOrDefault() as CodexSelectComponent;
        var minionMenuItem = selectMenu?.FirstOrDefault(item => item.Value == "codex:minion");
        var silenceMenuItem = selectMenu?.FirstOrDefault(item => item.Value == "card:Silence");

        Assert.NotNull(minionMenuItem);
        Assert.NotNull(silenceMenuItem);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task CreateCodexMessageLowercaseTest()
    {
        var codexRepo = new CodexMarkdownRulesRepository();
        var codexMessageService = new CodexMessageService(codexRepo);

        var message = await codexMessageService.CreateCodexMessageAsync("minion", CancellationToken.None);
        var selectOptions = message.Components?.FirstOrDefault() as CodexSelectComponent;

        Assert.NotNull(selectOptions);
        Assert.DoesNotContain(selectOptions, o => string.IsNullOrEmpty(o.Label));
    }


    [Fact]
    [Trait("Category", "Integration")]
    public async Task CreateCodexMaxedOutSelectTest()
    {
        var codexRepo = new CodexMarkdownRulesRepository();
        var codexMessageService = new CodexMessageService(codexRepo);

        var message = await codexMessageService.CreateCodexMessageAsync("Casting Spells", CancellationToken.None);
        var selectOptions = message.Components?.FirstOrDefault() as CodexSelectComponent;

        Assert.NotNull(selectOptions);
        Assert.Equal(25, selectOptions.Count());
    }
}
