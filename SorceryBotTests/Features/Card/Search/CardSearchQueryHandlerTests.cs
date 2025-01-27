using SorceryBot.Features.Card.Search;
using SorceryBot.Infrastructure.DataAccess.CardData;
using SorceryBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace SorceryBot.Features.Card.Search.Tests;

public class CardSearchQueryHandlerTests
{
    [Fact]
    public async Task HandleTestAsync()
    {
        var query = new GetCardsQuery() {CardNameContains = "Pudge" };
        var handler = new GetCardsQueryHandler(new FileCardRepository());

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Pudge Butcher", result.First().Name);
    }
}
