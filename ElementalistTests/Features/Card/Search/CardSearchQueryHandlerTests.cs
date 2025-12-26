using static Elementalist.DiscordUi.CardDisplay;
using Xunit;
using Elementalist.DiscordUi;
using Elementalist.Features.Cards;

namespace ElementalistTests.Features.Card.Search;

public class CardSearchQueryHandlerTests
{
    [Theory]
    [InlineData("Pudge", "Pudge Butcher")]
    [InlineData("Dream Quest", "Dream-Quest")]
    public async Task HandleTestAsync(string searchText, string actual)
    {
        var query = new GetCardsQuery() { CardNameContains = searchText };
        var handler = new GetCardsQueryHandler(new MockCardRepository());

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(actual, result.First().Name);
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
    public void BuildTooManyMatchesUrl()
    {
        var query = new GetCardsQuery() { TextContains = "Airborne Voidwalk", CardNameContains = "onda", TypeContains = "Minion Beast" };

        var url = CardSearchSlashCommand.GetRealmsAppUrl(query);

        Console.WriteLine(url.ToString());
    }
}
