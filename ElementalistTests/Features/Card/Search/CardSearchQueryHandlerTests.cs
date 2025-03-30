using static Elementalist.DiscordUi.CardDisplay;
using SorceryBotTests.Features.Card;
using Xunit;
using Elementalist.DiscordUi;

namespace Elementalist.Features.Card.Search.Tests;

public class CardSearchQueryHandlerTests
{
    [Fact]
    public async Task HandleTestAsync()
    {
        var query = new GetCardsQuery() { CardNameContains = "Pudge" };
        var handler = new GetCardsQueryHandler(new MockCardRepository());

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Pudge Butcher", result.First().Name);
    }

    [Theory()]
    [InlineData("Airborne Voidwalk", "Minion Beast", "Hounds of Ondaros")]
    [InlineData("dead Beast", null, "Wolpertinger")]
    public async Task HandleMultipleTermsTest(string textContains, string? typeLine, string actualCardName)
    {
        var query = new GetCardsQuery() { TextContains = textContains, TypeContains = typeLine };
        var handler = new GetCardsQueryHandler(new MockCardRepository());

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Contains(result, c => c.Name == actualCardName);
    }

    [Fact]
    public async Task BuildTooManyMatchesUrl()
    {
        var query = new GetCardsQuery() { TextContains = "Airborne Voidwalk", CardNameContains = "onda", TypeContains = "Minion Beast" };

        var url = CardSearchSlashCommand.GetRealmsAppUrl(query);

        Console.WriteLine(url.ToString());
    }
}
