using NUnit.Framework;
using SorceryBot.Features.Card.Search;
using SorceryBot.Infrastructure.DataAccess.CardData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SorceryBot.Features.Card.Search.Tests;

[TestFixture()]
public class CardSearchQueryHandlerTests
{
    [Test()]
    public async Task HandleTestAsync()
    {
        var query = new CardSearchQuery() {CardQuery = "Pudge" };
        var handler = new CardSearchQueryHandler(new FileCardRepository());

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.That(result, Is.Not.Null);
    }
}
