using SorceryBotTests.Features.Card;
using Xunit;

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
    [InlineData("Minion Beast Airborne Voidwalk", "Hounds of Ondaros")]
    [InlineData("dead Beast", "Wolpertinger")]
    public async Task HandleMultipleTermsTest(string contains, string actualCardName)
    {
        var query = new GetCardsQuery() { TextContains = contains };
        var handler = new GetCardsQueryHandler(new MockCardRepository());

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Contains(result, c => c.Name == actualCardName);
    }
}
