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
}
